﻿using System;

using System.ComponentModel.DataAnnotations;
using BExIS.Search.Model;

namespace BExIS.Web.Shell.Areas.Search.Models
{
    public class SearchAttributeViewModel
    {
        public int id { get; set; }
        //names
        [Display(Name = "Display Name")]
        [Required(ErrorMessage = "Please enter a Display Name.")]
        public String displayName { get; set; }

        [Display(Name = "Source Name")]
        [Required(ErrorMessage = "Please enter a Source Name.")]
        public String sourceName { get; set; }

        [Display(Name = "Metadata Node")]
        [Required(ErrorMessage = "Please select a Metadata link.")]
        public String metadataName { get; set; }

        //Type
        [Display(Name = "Search Component Type")]
        [Required(ErrorMessage = "Please select a Search Component")]
        public string searchType { get; set; }

        [Display(Name = "Data Type")]
        [Required(ErrorMessage = "Please select a Data Type")]
        public string dataType { get; set; }

        // parameter for index
        [Display(Name = "Store")]
        public bool store { get; set; }

        [Display(Name = "Multi Value")]
        public bool multiValue { get; set; }

        [Display(Name = "Analyzed")]
        public bool analysed { get; set; }

        [Display(Name = "Norm")]
        public bool norm { get; set; }

        [Display(Name = "Boost")]
        [Required(ErrorMessage = "Please select a Number greater then 0")]
        public double boost { get; set; }

        // ResultView
        [Display(Name = "Header Item")]
        public bool headerItem { get; set; }

        [Display(Name = "Default Header Item")]
        public bool defaultHeaderItem { get; set; }

        // properties
        [Display(Name = "Sort Direction")]
        [Required(ErrorMessage = "Please select a Direction")]
        public string direction { get; set; }

        [Display(Name = "UI Component Type")]
        [Required(ErrorMessage = "Please select a UI Component")]
        public string uiComponent { get; set; }

        [Display(Name = "Selection Type")]
        [Required(ErrorMessage = "Please select a Selection Type")]
        public string aggregationType { get; set; }

        [Display(Name = "Date Format")]
        public string dateFormat = "bgc:format";


        public static SearchAttribute GetSearchAttribute(SearchAttributeViewModel searchAttributeViewModel)
        {
            SearchAttribute sa = new SearchAttribute();

            //names
            sa.displayName = searchAttributeViewModel.displayName;
            sa.sourceName = searchAttributeViewModel.sourceName;
            sa.metadataName = searchAttributeViewModel.metadataName;

            //types
            sa.dataType = SearchAttribute.GetDataType(searchAttributeViewModel.dataType);
            sa.searchType = SearchAttribute.GetSearchType(searchAttributeViewModel.searchType);

            // parameter for index
            sa.store = searchAttributeViewModel.store;
            sa.multiValue = searchAttributeViewModel.multiValue;
            sa.analysed = searchAttributeViewModel.analysed;
            sa.norm = searchAttributeViewModel.norm;
            sa.boost = searchAttributeViewModel.boost;

            // resultview
            sa.headerItem = searchAttributeViewModel.headerItem;
            sa.defaultHeaderItem = searchAttributeViewModel.defaultHeaderItem;

            // properties
            sa.direction = SearchAttribute.GetDirection(searchAttributeViewModel.direction);
            sa.uiComponent = SearchAttribute.GetUIComponent(searchAttributeViewModel.uiComponent);
            sa.aggregationType = SearchAttribute.GetAggregationType(searchAttributeViewModel.aggregationType);
            sa.dateFormat = searchAttributeViewModel.dateFormat;

            return sa;
        }

        public static SearchAttributeViewModel GetSearchAttributeViewModel(SearchAttribute searchAttribute)
        {
            SearchAttributeViewModel sa = new SearchAttributeViewModel();

            sa.id = searchAttribute.id;
            //names
            sa.displayName = searchAttribute.displayName;
            sa.sourceName = searchAttribute.sourceName;
            sa.metadataName = searchAttribute.metadataName;

            //types
            sa.dataType = SearchAttribute.GetDataTypeAsDisplayString(searchAttribute.dataType);
            sa.searchType = SearchAttribute.GetSearchTypeAsDisplayString(searchAttribute.searchType);

            // parameter for index
            sa.store = searchAttribute.store;
            sa.multiValue = searchAttribute.multiValue;
            sa.analysed = searchAttribute.analysed;
            sa.norm = searchAttribute.norm;
            sa.boost = searchAttribute.boost;

            // resultview
            sa.headerItem = searchAttribute.headerItem;
            sa.defaultHeaderItem = searchAttribute.defaultHeaderItem;

            // properties
            sa.direction = SearchAttribute.GetDirectionAsString(searchAttribute.direction);
            sa.uiComponent = SearchAttribute.GetUIComponentAsString(searchAttribute.uiComponent);
            sa.aggregationType = SearchAttribute.GetAggregationTypeAsString(searchAttribute.aggregationType);
            sa.dateFormat = searchAttribute.dateFormat;

            return sa;
        }



    }
}