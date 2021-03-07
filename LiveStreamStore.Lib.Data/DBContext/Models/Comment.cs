using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiveStreamStore.Lib.Data.DBContext.Models
{
    public partial class Comment
    {
        [Key]
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? LiveStreamId { get; set; }
        [StringLength(100)]
        public string FaceBookId { get; set; }
        [StringLength(100)]
        public string CommentFaceBookId { get; set; }
        [StringLength(200)]
        public string FaceBookName { get; set; }
        [StringLength(1000)]
        public string CommentContent { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDateUtc { get; set; }

        [ForeignKey(nameof(CustomerId))]
        [InverseProperty("Comment")]
        public virtual Customer Customer { get; set; }
        [ForeignKey(nameof(LiveStreamId))]
        [InverseProperty("Comment")]
        public virtual LiveStream LiveStream { get; set; }
    }
}
