using System.Collections.Generic;

namespace IntelligenceQueryEngine.Model.Dto
{
    public class ApiResponseV2<T>
    {
        public string Status { get; set; } = "success";
        public int Version { get; set; } = 2;
        public ResponseData<T> Data { get; set; } = new();
    }

    public class ResponseData<T>
    {
        public List<T> Items { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int? NextPage { get; set; }
        public int? PrevPage { get; set; }
        public int FirstPage { get; set; } = 1;
        public int LastPage { get; set; }
    }
}
