﻿using System.Text.Json;

namespace StorApp.Dtos
{
    public class PaginationMetaData
    {
        public int TotalItemCount { get; set; }
        public int TotalPageCount { get; set; }
        public int PageSize { get; set; }
        public int  CurrentPage { get; set; }

        public PaginationMetaData( int totalItemCount, int pageSize , int currentPage )
        {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPage = currentPage;

            TotalPageCount =(int)Math.Ceiling(totalItemCount / (double)pageSize);
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}