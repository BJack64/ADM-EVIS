using System;

namespace eFakturADM.Logic.Utilities
{
    public class Filter
    {
        /// <summary>
        /// Items per page number
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Sorting column name.
        /// </summary>
        public string SortColumnName { get; set; }

        /// <summary>
        /// Sorting column number.
        /// </summary>
        public int SortColumn { get; set; }

        /// <summary>
        /// Order direction.
        /// </summary>
        public bool SortOrderAsc { get; set; }

        /// <summary>
        /// Current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Set of words for searching.
        /// </summary>
        public String Search { get; set; }

        /// <summary>
        /// Use Paging or not
        /// </summary>
        public bool EnablePage { get; set; }
    }
}
