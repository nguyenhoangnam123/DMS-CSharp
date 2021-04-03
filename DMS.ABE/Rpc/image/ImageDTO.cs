using DMS.ABE.Common;
using DMS.ABE.Entities;
using System;
using System.Collections.Generic;

namespace DMS.ABE.Rpc.image
{
    public class ImageDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
        public byte[] Content { get; set; }

        public ImageDTO()
        {
        }
    }
}
