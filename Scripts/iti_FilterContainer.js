var filterCategoryID_def = '';
function load_filterCategoryID_g(filterCategoryID) {
    filterCategoryID_def = filterCategoryID;
}

function iti_FilterParameter(ParameterID, ParameterValue, ParameterField, Removable) {
    this.ParameterID = ParameterID;
    this.ParameterValue = ParameterValue;
    this.ParameterField = ParameterField;
    
    if (Removable == false) {
        this.Removable = false;
    }
    else {
        this.Removable = true;
    }
}

function iti_Filter(filterName, filterModule, filterAffiliation, filterAffiliationName, filterRole, filterRoleName, filterCategoryID) {
    
    this.Name = filterName;
    this.Module = filterModule;
    this.CategoryID = filterCategoryID;
    this.Affiliation = filterAffiliation;
    this.AffiliationName = filterAffiliationName;
    this.Role = filterRole;
    this.RoleName = filterRoleName;
    this.Parameters = [];
    this.ParameterCount = 0;
    this.AddParameter = function (ParameterID, ParameterValue, ParameterField, Removable, filterCategoryID) { this.Parameters[this.ParameterCount] = new iti_FilterParameter(ParameterID, ParameterValue, ParameterField, Removable, filterCategoryID); this.ParameterCount++; };
    this.GetParameter = function (ParameterID, ParamterferField) {
        for (i = 0; i <= this.ParameterCount - 1; i++) {
            if (this.Parameters[i].ParameterID == ParameterID && this.Parameters[i].ParameterField == ParamterferField) {
                return this.Parameters[i];
            }
        }
        return null;
    };
}

function iti_FilterContainer() {
    this.Filters = [];
    this.FilterCount = 0;
    this.AddFilter = function (FilterName, FilterModule, FilterAffiliation, FilterAffiliationName, FilterRole, FilterRoleName, filterCategoryID) { this.Filters[this.FilterCount] = new iti_Filter(FilterName, FilterModule, FilterAffiliation, FilterAffiliationName, FilterRole, FilterRoleName, filterCategoryID); this.FilterCount++; };
    this.AddParameter = function (FilterName, FilterModule, FilterAffiliation, FilterAffiliationName, FilterRole, FilterRoleName, ParameterID, ParameterValue, ParameterField, Removable, filterCategoryID) {
        var existingFilter = this.GetFilter(FilterName, FilterModule, FilterAffiliation, FilterRole);
        if (!existingFilter) {
            this.AddFilter(FilterName, FilterModule, FilterAffiliation, FilterAffiliationName, FilterRole, FilterRoleName, filterCategoryID);
            existingFilter = this.GetFilter(FilterName, FilterModule, FilterAffiliation, FilterRole, filterCategoryID);
        }

        var existingParameter = existingFilter.GetParameter(ParameterID, ParameterField);
        if (!existingParameter) {
            existingFilter.AddParameter(ParameterID, ParameterValue, ParameterField, Removable);
            existingParameter = existingFilter.GetParameter(ParameterID, ParameterField);
        }
        else {
            existingParameter.ParameterValue = ParameterValue;
        }
    };
    this.RemoveFilter = function (FilterName, FilterModule, FilterAffiliation, FilterRole, filterCategoryID) {
        var newFilter = new iti_FilterContainer();
        for (var i = 0; i <= this.Filters.length - 1; i++) {
            if (this.Filters[i].Name != FilterName || this.Filters[i].Module != FilterModule || this.Filters[i].CategoryID != filterCategoryID) {
                newFilter.Filters[newFilter.FilterCount] = this.Filters[i];
                newFilter.FilterCount++;
            }
        }

        this.Filters.length = 0;
        this.FilterCount = 0;
        this.Filters = [];

        for (var i = 0; i <= newFilter.FilterCount - 1; i++) {
            this.Filters[this.FilterCount] = newFilter.Filters[i];
            this.FilterCount++;
        }
    };
    this.RemoveParameter = function (FilterName, FilterModule, FilterAffiliation, FilterRole, ParameterID, ParameterValue, filterCategoryID) {
    	var selectedFilter = this.GetFilter(FilterName, FilterModule, FilterAffiliation, FilterRole);
    	var newParamIDs = [];
        var newParamValues = [];
        var newParamField = [];
        var newParamRemovable = [];
        for (var i = 0; i <= selectedFilter.ParameterCount - 1; i++) {
            if (selectedFilter.Parameters[i].ParameterID == ParameterID && selectedFilter.Parameters[i].ParameterValue == ParameterValue) {
                selectedFilter.Parameters[i] = "";
            }
        }

        for (var i = 0; i <= selectedFilter.ParameterCount - 1; i++) {
            if (selectedFilter.Parameters[i] != "") {
                newParamIDs[newParamIDs.length] = selectedFilter.Parameters[i].ParameterID;
                newParamValues[newParamValues.length] = selectedFilter.Parameters[i].ParameterValue;
                newParamField[newParamField.length] = selectedFilter.Parameters[i].ParameterField;
                newParamRemovable[newParamRemovable.length] = selectedFilter.Parameters[i].Removable;
            }
        }
        selectedFilter.Parameters.length = 0;
        selectedFilter.ParameterCount = 0;

        for (var i = 0; i <= newParamIDs.length - 1; i++) {
            selectedFilter.AddParameter(newParamIDs[i], newParamValues[i], newParamField[i], newParamRemovable[i]);
        }

        if (selectedFilter.ParameterCount == 0) {
            this.RemoveFilter(FilterName, FilterModule, FilterAffiliation, filterCategoryID);
            return 1;
        }
        else {
            return 0;
        }
    };
    this.GetFilter = function (FilterName, FilterModule, FilterAffiliation, FilterRole, filterCategoryID) {
        for (i = 0; i <= this.FilterCount - 1; i++) {
            if (this.Filters[i].Name == FilterName && this.Filters[i].Module == FilterModule && this.Filters[i].Affiliation == FilterAffiliation && this.Filters[i].Role == FilterRole && this.Filters[i].CategoryID == filterCategoryID) {
                return this.Filters[i];
            }
        }
        return null;
    };

    this.ClearFilters = function (ModuleSection, filterCategoryID) {
        for (i = 0; i <= this.Filters.length - 1; i++) {
            if (this.Filters[i].Module == ModuleSection && this.Filters[i].CategoryID == filterCategoryID) {
                this.RemoveFilter(this.Filters[i].Name, this.Filters[i].Module, this.Filters[i].Affiliation, this.Filters[i].Role, filterCategoryID);
                i--;
            }
        }
    };
}

function CloneFilterContainer(filterContainer) {
    var nFilterContainer = new iti_FilterContainer;
    nFilterContainer.Filters = [];
    for (var i = 0; i <= filterContainer.FilterCount - 1; i++) {
        var nFilter = new iti_Filter(filterContainer.Filters[i].Name, filterContainer.Filters[i].Module, filterContainer.Filters[i].Affiliation, filterContainer.Filters[i].AffiliationName, filterContainer.Filters[i].Role, filterContainer.Filters[i].RoleName);

        for (var y = 0; y <= filterContainer.Filters[i].Parameters.length - 1; y++) {
            var nParameter = new iti_FilterParameter(filterContainer.Filters[i].Parameters[y].ParameterID, filterContainer.Filters[i].Parameters[y].ParameterValue, filterContainer.Filters[i].Parameters[y].ParameterField, filterContainer.Filters[i].Parameters[y].Removable);
            nFilter.Parameters[nFilter.Parameters.length] = nParameter;
            nFilter.ParameterCount++;
        }
        nFilterContainer.Filters[nFilterContainer.Filters.length] = nFilter;
        nFilterContainer.FilterCount++;
    }
    return nFilterContainer;
}
  