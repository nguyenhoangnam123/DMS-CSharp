using Common;
using DMS.Entities;
using DMS.Services.MImage;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.image
{
    public class ImageRoute : Root
    {
        public const string Master = Module + "/image/image-master";
        public const string Detail = Module + "/image/image-detail";
        private const string Default = Rpc + Module + "/image";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ImageFilter.Id), FieldType.ID },
            { nameof(ImageFilter.Name), FieldType.STRING },
            { nameof(ImageFilter.Url), FieldType.STRING },
        };
    }

    public class ImageController : RpcController
    {
        private IImageService ImageService;
        private ICurrentContext CurrentContext;
        public ImageController(
            IImageService ImageService,
            ICurrentContext CurrentContext
        )
        {
            this.ImageService = ImageService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ImageRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO);
            ImageFilter = ImageService.ToFilter(ImageFilter);
            int count = await ImageService.Count(ImageFilter);
            return count;
        }

        [Route(ImageRoute.List), HttpPost]
        public async Task<ActionResult<List<Image_ImageDTO>>> List([FromBody] Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO);
            ImageFilter = ImageService.ToFilter(ImageFilter);
            List<Image> Images = await ImageService.List(ImageFilter);
            List<Image_ImageDTO> Image_ImageDTOs = Images
                .Select(c => new Image_ImageDTO(c)).ToList();
            return Image_ImageDTOs;
        }

        [Route(ImageRoute.Get), HttpPost]
        public async Task<ActionResult<Image_ImageDTO>> Get([FromBody]Image_ImageDTO Image_ImageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Image_ImageDTO.Id))
                return Forbid();

            Image Image = await ImageService.Get(Image_ImageDTO.Id);
            return new Image_ImageDTO(Image);
        }

        [Route(ImageRoute.Create), HttpPost]
        public async Task<ActionResult<Image_ImageDTO>> Create([FromBody] Image_ImageDTO Image_ImageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Image_ImageDTO.Id))
                return Forbid();

            Image Image = ConvertDTOToEntity(Image_ImageDTO);
            Image = await ImageService.Create(Image);
            Image_ImageDTO = new Image_ImageDTO(Image);
            if (Image.IsValidated)
                return Image_ImageDTO;
            else
                return BadRequest(Image_ImageDTO);
        }

        [Route(ImageRoute.Update), HttpPost]
        public async Task<ActionResult<Image_ImageDTO>> Update([FromBody] Image_ImageDTO Image_ImageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Image_ImageDTO.Id))
                return Forbid();

            Image Image = ConvertDTOToEntity(Image_ImageDTO);
            Image = await ImageService.Update(Image);
            Image_ImageDTO = new Image_ImageDTO(Image);
            if (Image.IsValidated)
                return Image_ImageDTO;
            else
                return BadRequest(Image_ImageDTO);
        }

        [Route(ImageRoute.Delete), HttpPost]
        public async Task<ActionResult<Image_ImageDTO>> Delete([FromBody] Image_ImageDTO Image_ImageDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Image_ImageDTO.Id))
                return Forbid();

            Image Image = ConvertDTOToEntity(Image_ImageDTO);
            Image = await ImageService.Delete(Image);
            Image_ImageDTO = new Image_ImageDTO(Image);
            if (Image.IsValidated)
                return Image_ImageDTO;
            else
                return BadRequest(Image_ImageDTO);
        }

        [Route(ImageRoute.Import), HttpPost]
        public async Task<ActionResult<List<Image_ImageDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };

            List<Image> Images = await ImageService.Import(DataFile);
            List<Image_ImageDTO> Image_ImageDTOs = Images
                .Select(c => new Image_ImageDTO(c)).ToList();
            return Image_ImageDTOs;
        }

        [Route(ImageRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO);
            ImageFilter = ImageService.ToFilter(ImageFilter);
            DataFile DataFile = await ImageService.Export(ImageFilter);
            return new FileStreamResult(DataFile.Content, StaticParams.ExcelFileType)
            {
                FileDownloadName = DataFile.Name ?? "File export.xlsx",
            };
        }

        [Route(ImageRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Id = new IdFilter { In = Ids };
            ImageFilter.Selects = ImageSelect.Id;
            ImageFilter.Skip = 0;
            ImageFilter.Take = int.MaxValue;

            List<Image> Images = await ImageService.List(ImageFilter);
            Images = await ImageService.BulkDelete(Images);
            return true;
        }

        private async Task<bool> HasPermission(long Id)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter = ImageService.ToFilter(ImageFilter);
            if (Id == 0)
            {

            }
            else
            {
                ImageFilter.Id = new IdFilter { Equal = Id };
                int count = await ImageService.Count(ImageFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Image ConvertDTOToEntity(Image_ImageDTO Image_ImageDTO)
        {
            Image Image = new Image();
            Image.Id = Image_ImageDTO.Id;
            Image.Name = Image_ImageDTO.Name;
            Image.Url = Image_ImageDTO.Url;
            Image.BaseLanguage = CurrentContext.Language;
            return Image;
        }

        private ImageFilter ConvertFilterDTOToFilterEntity(Image_ImageFilterDTO Image_ImageFilterDTO)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Selects = ImageSelect.ALL;
            ImageFilter.Skip = Image_ImageFilterDTO.Skip;
            ImageFilter.Take = Image_ImageFilterDTO.Take;
            ImageFilter.OrderBy = Image_ImageFilterDTO.OrderBy;
            ImageFilter.OrderType = Image_ImageFilterDTO.OrderType;

            ImageFilter.Id = Image_ImageFilterDTO.Id;
            ImageFilter.Name = Image_ImageFilterDTO.Name;
            ImageFilter.Url = Image_ImageFilterDTO.Url;
            return ImageFilter;
        }


    }
}

