// (c) Copyright Cleverous 2022. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Cleverous.VaultDashboard;
using Cleverous.VaultSystem;
using UnityEngine;

public static class VaultListFilter
{
    // *** PROPERTY SELECTION ***
    // Property Name

    // *** TYPES ***
    // (#) int
    // (#) float
    // (A) string
    // (A) enum

    // *** OPERATIONS ***
    // Includes / Exact (default, no operator)
    // Greater Than >
    // Less Than <
    // Between - (require lhs/rhs vals, only numerical properties)
    
    public enum FilterType { String, Float, Int }
    public enum FilterOp
    {
        Contains, 
        GreaterThan, 
        LessThan
    }
    public static List<string> FilterOpSymbols = new List<string>
    {
        "=",
        ">",
        "<"
    };

    public static List<string> GetFilterablePropertyNames()
    {
        Type targetType = VaultDashboard.CurrentSelectedGroup.SourceType;
        List<string> results = (from field in targetType.GetFields() where Attribute.IsDefined(field, typeof(VaultFilterableAttribute)) select field.Name).ToList();
        results.AddRange(from prop in targetType.GetProperties() where Attribute.IsDefined(prop, typeof(VaultFilterableAttribute)) select prop.Name);
        return results;
    }

    /// <summary>
    /// Filters the current Group based on class field/property criteria.
    /// </summary>
    /// <returns>A list of DataEntity assets from the selected Group that meet the filter criteria.</returns>
    public static List<DataEntity> FilterList()
    {
        string input = VaultDashboard.Instance.GetAssetFilterPropertyValue();

        // if the list is too small, just return the list.
        if (VaultDashboard.CurrentSelectedGroup.Content.Count < 2 || string.IsNullOrEmpty(input)) return VaultDashboard.CurrentSelectedGroup.Content;

        List<DataEntity> filteredListResult = new List<DataEntity>();
        Type targetType = VaultDashboard.CurrentSelectedGroup.SourceType;
        FilterOp operation = VaultDashboard.Instance.GetAssetFilterOperation();
        FilterType filterType = VaultDashboard.Instance.GetAssetFilterPropertyType();

        // ********* FIGURE OUT THE OPERATOR ********* //
        if (filterType == FilterType.Float)
        {
            foreach (DataEntity asset in VaultDashboard.CurrentSelectedGroup.Content)
            {
                float assetValue = Convert.ToSingle(targetType.GetField(VaultDashboard.Instance.GetAssetFilterPropertyName()).GetValue(asset));
                float targetValue = VaultDashboard.Instance.AssetFilterValueFloat;

                switch (operation)
                {
                    case FilterOp.Contains:
                        if (Mathf.Approximately(assetValue, targetValue)) filteredListResult.Add(asset);
                        break;
                    case FilterOp.GreaterThan:
                        if (assetValue > targetValue) filteredListResult.Add(asset);
                        break;
                    case FilterOp.LessThan:
                        if (assetValue < targetValue) filteredListResult.Add(asset);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        else if (filterType == FilterType.Int)
        {
            foreach (DataEntity asset in VaultDashboard.CurrentSelectedGroup.Content)
            {
                int assetValue = Convert.ToInt32(targetType.GetField(VaultDashboard.Instance.GetAssetFilterPropertyName()).GetValue(asset));
                int targetValue = VaultDashboard.Instance.AssetFilterValueInt;

                switch (operation)
                {
                    case FilterOp.Contains:
                        if (assetValue == targetValue) filteredListResult.Add(asset);
                        break;
                    case FilterOp.GreaterThan:
                        if (assetValue > targetValue) filteredListResult.Add(asset);
                        break;
                    case FilterOp.LessThan:
                        if (assetValue < targetValue) filteredListResult.Add(asset);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        else if (filterType == FilterType.String)
        {
            // string search (could also support enum here later?)
            filteredListResult = VaultDashboard.CurrentSelectedGroup.Content.FindAll(x => x.Title.ToLower().Contains(input.ToLower()));
        }

        return filteredListResult;
    }
}