using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiGateway.Infrastructure.Persistence.EFCore.Abstracts;

namespace ApiGateway.Infrastructure.Persistence.EFCore.Models
{
    public class RefreshToken : IHasAudit
    {
        /// <summary>
        /// 不透明ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 用戶ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 已雜湊的Token
        /// </summary>
        public string HashedToken { get; set; }

        /// <summary>
        /// Token家族
        /// </summary>
        public Guid FamilyId { get; set; }

        /// <summary>
        /// 上一顆RefreshToken的ID
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// 發布時間
        /// </summary>
        public DateTime IssuedAt { get; set; }

        /// <summary>
        /// 過期時間
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// 狀態  1:有效 2:已撤校 3: 已過期
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 撤銷原因
        /// </summary>
        public string RevokedReason { get; set; }

        /// <summary>
        /// 版本號 每次更新時+1
        /// </summary>
        public long Version { get; private set; } = 0;

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
