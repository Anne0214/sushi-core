-- @bucket: RedisKey（會被映射到 KEYS），token 數量的 key
-- 其餘為一般參數（映射到 ARGV）
-- 參數：
-- @maxCapacity = token max capacity
-- @refillIntervalMs = token refillIntervalMs
-- @refillTokens = refillTokens (每個 refill 週期補充多少 token)
-- @nowMs = nowMs (optional; '' 表示用 redis TIME)

local function now_ms()
  if @nowMs ~= nil and @nowMs ~= '' then
    return tonumber(@nowMs)
  end
  local t = redis.call('TIME')
  -- t[1] 秒, t[2] 微秒（不是奈秒）
  return tonumber(t[1]) * 1000 + math.floor(tonumber(t[2]) / 1000)
end
local capacity          = tonumber(@maxCapacity)
local refillIntervalMs  = tonumber(@refillIntervalMs)
local refillTokens      = tonumber(@refillTokens)

-- 取得 Redis 伺服器當前毫秒時間
local ms = now_ms()

-- 讀取現況
local raw = redis.call('GET', @bucket)
local tokenData

if not raw then
  tokenData = {
    token = capacity,
    lastRefillTime = ms
  }
else
  tokenData = cjson.decode(raw)
  if not tokenData then
    tokenData = { token = capacity, lastRefillTime = ms }
  end
end

-- 計算補充
local elapsed = ms - (tokenData.lastRefillTime or ms)
if elapsed < 0 then
  elapsed = 0
end

-- 「允許小數 token」：用浮點補充更精準
-- 若你要「只補整數 token」，把下一行改成：
local tokensToAdd = (elapsed / refillIntervalMs) * refillTokens

local afterRefill = (tokenData.token or 0) + tokensToAdd
local available   = math.min(afterRefill, capacity)

-- 判斷是否可扣 1 個 token
if available >= 1 then
  available = available - 1
  tokenData.token = available
  tokenData.lastRefillTime = ms
  redis.call('SET', @bucket, cjson.encode(tokenData))
  return 1
else
  -- 不能扣款也要把「補完後的狀態」寫回（不扣）
  tokenData.token = available
  tokenData.lastRefillTime = ms
  redis.call('SET', @bucket, cjson.encode(tokenData))
  return 0
end
