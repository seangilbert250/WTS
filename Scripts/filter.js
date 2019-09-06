﻿//filter.js
function filterContainer(el) {
    var fc = this;

    if (typeof (el) == 'string') {
        this.containerElement = document.getElementById(el);
    }
    else {
        this.containerElement = el;
    }

    this.argsForTable = {};
    this.tableGrouping = [];
    this.editableDisplay = true;

    //setup filters
    this.filters = [];
    this.filters.indexOf = function (object) {
        for (var i = 0; i <= this.length - 1; i++) {
            if (this[i] == object) {
                return i;
            }
        }
        return null;
    }
    this.filters.add = function (args) {
        var existing = fc.filters.find(args);
        if (existing.length > 0) {
            return existing[0];
        }

        var nFilter = new filter(args);
        fc.filters.push(nFilter);
        return nFilter;
    }
    this.filters.insert = function (index, args) {
        var existing = fc.filters.find(args);
        if (existing.length > 0) {
            return existing[0];
        }
        var nFilter = new filter(args);
        fc.filters.splice(index, 0, nFilter);
        return nFilter;
    }
    this.filters.remove = function (filter) {
        var i = fc.filters.indexOf(filter);

        if (fc.filters[i]["removable"] != undefined) {
            if (fc.filters[i]["removable"] == false) {
            }
            else {
                fc.filters.splice(i, 1);
            }
        }
        else {
            fc.filters.splice(i, 1);
        }
    }
    this.filters.removeAt = function (index) {
        if (fc.filters[i]["removable"] != undefined) {
            if (fc.filters[i]["removable"] == false) {
            }
            else {
                fc.filters.splice(index, 1);
            }
        }
        else {
            fc.filters.splice(index, 1);
        }
    }
    this.filters.find = function (args) {
        return findFilter(fc.filters, args);
    }
    this.filters.findWithGroup = function (groups) {
        return findWithGroup(fc.filters, groups);
    }
    this.filters.removable = function (value) {
        filtersRemovable(this, value);
    }
    this.filters.clear = function () {
        clearFilters(fc.filters);
    }
    this.toJSON = function (args, groupName) {
        var filtersToExport = this.filters.find(args);
        try {
            if (filtersToExport && filtersToExport.length > 0) {
                var outFilters = {};
                if (groupName) {
                    var gValues = groupValues(filtersToExport, groupName);
                    for (var g = 0; g <= gValues.length - 1; g++) {
                        var gFilter = {};
                        var gData = {};
                        gData[groupName] = gValues[g];

                        var filtersFromGroupVal = filtersToExport.find({ groups: gData });
                        for (var f = 0; f <= filtersFromGroupVal.length - 1; f++) {
                            var gFilterData = {};

                            var ntext = filtersFromGroupVal[f].parameters.toString(',', true);
                            var nvalue = filtersFromGroupVal[f].parameters.toString(',', false);
                            var nParameter = { value: nvalue, text: ntext };
                            //gFilter[filtersFromGroupVal[f].name] = filtersFromGroupVal[f].parameters.toString(',', true);
                            //gFilter[filtersFromGroupVal[f].name + "ID"] = filtersFromGroupVal[f].parameters.toString(',', false);

                            gFilter[filtersFromGroupVal[f].name] = nParameter;
                        }
                        outFilters[gValues[g]] = gFilter;
                    }
                    var jsStr = toJSONString(outFilters);

                    if (jsStr == undefined) {
                        jsStr = '{null:{null:null}}';
                    }
                }
                else {
                    for (var i = 0; i <= filtersToExport.length - 1; i++) {
                        var gFilterData = {};
                        var ntext = filtersToExport[i].parameters.toString(',', true);
                        var nvalue = filtersToExport[i].parameters.toString(',', false);
                        var nParameter = { value: nvalue, text: ntext };
                        outFilters[filtersToExport[i].name] = nParameter;
                    }
                    var jsStr = toJSONString(outFilters);

                    if (jsStr == undefined) {
                        jsStr = '{null:null}';
                    }
                }


                return jsStr;
            }

            if (groupName) {
                return '{null:{null:null}}';
            }
            else {
                return '{null:null}';
            }
        }
        catch (e) {
            throw (e);
        }
    }
    this.toTable = function (args, groups) {
        try{
            this.argsForTable = args;
            this.tableGrouping = groups;

            var filtersToShow = [];

            if (!args) {
                filtersToShow = fc.filters;
            }
            else {
                filtersToShow = fc.filters.find(args);
            }

            var cTable = document.createElement('table');
            cTable.cellPadding = 0;
            cTable.cellSpacing = 0;
            cTable.style.cursor = 'default';

            var groupFilters = groupedFilters(filtersToShow, groups,'', true);

            var gItem = groupFilters.item;
            if (gItem) {
                for (var i = 0; i <= gItem.values.length - 1; i++) {
                    var groupItem = gItem.values[i];
                    var nRow = cTable.insertRow(cTable.rows.length);
                    var nCell = nRow.insertCell(nRow.cells.length);

                    if (groupItem.subGroup) {
                        var sTable = document.createElement('table');
                        sTable.cellPadding = 0;
                        sTable.cellSpacing = 0;
                        sTable.style.marginBottom = "10px";
                        var sRow = sTable.insertRow(sTable.rows.length);
                        var sCell = sRow.insertCell(sRow.cells.length);

                        nCell.appendChild(sTable);
                        var sDiv = document.createElement('div');
                        sDiv.innerHTML = '- ' + groupItem.value.toUpperCase() + ' -';
                        sCell.appendChild(sDiv);
                        subGroupRow(groupItem, sTable);
                    }
                    else {
                        var sDiv = document.createElement('div');
                        sDiv.innerHTML = '- ' + groupItem.value.toUpperCase() + ' -';
                        nCell.appendChild(sDiv);
                        var nRow2 = cTable.insertRow(cTable.rows.length);
                        var nCell2 = nRow2.insertCell(nRow2.cells.length);
                        filterRow(groupItem, cTable);
                        nCell2.style.paddingBottom = '10px';
                    }
                }
            }

            this.containerElement.innerHTML = '';

            if (cTable.rows.length > 0) {
                this.containerElement.appendChild(cTable);
            }

            if (this.containerElement.innerHTML == '') {
                this.containerElement.style.borderTop = '1px solid grey';
            } else {
                this.containerElement.style.borderTop = 'none';
            }

            return cTable;
        }
        catch (e) {
            throw (e);
        }
    }
    this.clone = function (el) {
        try{
            var cloneFilterContainer = new filterContainer(el);
            //clone filters
            for (var f = 0; f <= this.filters.length - 1; f++) {
                var args = { name: this.filters[f].name, groups: this.filters[f].groups };
                var nFilter = cloneFilterContainer.filters.add(args);
                for (var key in this.filters[f]) {
                    if (key != 'parameters' && key != 'remove' && key != 'removeAt' && key != 'find' && key != 'findWithGroup' && key != 'add' && key != 'insert' && key != 'indexOf') {
                        nFilter[key] = this.filters[f][key];
                    }
                }
                //clone parameters
                for (var p = 0; p <= this.filters[f].parameters.length - 1; p++) {
                    nFilter.parameters.add(this.filters[f].parameters[p].value, this.filters[f].parameters[p].text);
                }
            }
            return cloneFilterContainer;
        }
        catch (e) {
            throw (e);
        }
    }

    var subGroupRow = function (gItem, cTable) {
        var gItem = gItem.subGroup.item;
        if (gItem) {
            for (var i = 0; i <= gItem.values.length - 1; i++) {
                var groupItem = gItem.values[i];
                var gRow = cTable.insertRow(cTable.rows.length);
                var gCell = gRow.insertCell(gRow.cells.length);

                gCell.style.paddingLeft = '10px';

                var nTable = document.createElement('table');
                nTable.cellPadding = 0;
                nTable.cellSpacing = 0;

                var nRow = nTable.insertRow(nTable.rows.length);
                var nCell = nRow.insertCell(nRow.cells.length);
                nCell.innerHTML = groupItem.value.toUpperCase();

                nCell.style.fontWeight = 'bold';
                gCell.appendChild(nTable);

                if (groupItem.subGroup) {

                    subGroupRow(groupItem, nTable);
                }
                else {
                    filterRow(groupItem, nTable);
                }
            }
        }
    }
    var filterRow = function (groupItem, nTable) {
        for (var f = 0; f <= groupItem.filters.length - 1; f++) {
            var filter = groupItem.filters[f];
            if (filter.display) {
                var nRow = nTable.insertRow(nTable.rows.length);
                var nCell = nRow.insertCell(nRow.cells.length);
                var fTable = document.createElement('table');
                fTable.cellPadding = 0;
                fTable.cellSpacing = 0;
                var fRowTitle = fTable.insertRow(fTable.rows.length);
                var fCellTitle = fRowTitle.insertCell(fRowTitle.cells.length);
                fCellTitle.style.fontWeight = 'bold';
                nCell.appendChild(fTable);
                fCellTitle.style.paddingLeft = "10px";

                var titleTable = document.createElement('table');
                titleTable.style.width = 'auto';
                titleTable.cellPadding = 0;
                titleTable.cellSpacing = 0;
                titleTable.style.textAlign = 'left';
                var tTableRow = titleTable.insertRow(titleTable.rows.length);
                var tTableCell1 = tTableRow.insertCell(tTableRow.cells.length);
                var tTableCell2 = tTableRow.insertCell(tTableRow.cells.length);
                tTableCell1.innerHTML = filter.name.toUpperCase();
                var fButton = document.createElement('img');
                tTableCell2.style.width = '12px';
                tTableCell2.style.paddingLeft = '2px';
                tTableCell2.style.textAlign = 'left';
                tTableCell2.appendChild(fButton);
                fButton.src = 'images/icons/cross.png';
                fButton.alt = 'Remove';
                fButton.title = 'Remove';
                fButton.style.display = 'none';

                fCellTitle.appendChild(titleTable);

                if (fc.editableDisplay && filter.removable != false) {
                    fRowTitle.onmouseover = function () {
                        var fButton = this.getElementsByTagName('img')[0];
                        if (fButton) {
                            fButton.style.display = 'block';
                        }
                    }
                    fRowTitle.onmouseout = function () {
                        var fButton = this.getElementsByTagName('img')[0];
                        if (fButton) {
                            fButton.style.display = 'none';
                        }
                    }

                    fButton.filter = filter;
                    fButton.onclick = function () {
                        this.filter.remove();
                        fc.toTable(fc.argsForTable, fc.tableGrouping);
                    }
                }

                var fRow = fTable.insertRow(fTable.rows.length);
                var fCell = fRow.insertCell(fRow.cells.length);
                var pTable = document.createElement('table');
                pTable.cellPadding = 0;
                pTable.cellSpacing = 0;
                fCell.style.paddingLeft = "10px";

                //add parameters
                var parameters = filter.parameters;
                if (parameters.length > 0) {
                    fCell.appendChild(pTable);
                }
                for (var p = 0; p <= parameters.length - 1; p++) {
                    var pRow = pTable.insertRow(pTable.rows.length);
                    var pCellButton = pRow.insertCell(pRow.cells.length);
                    var pCell = pRow.insertCell(pRow.cells.length);
                    var pButton = document.createElement('img');
                    pCellButton.style.width = '12px';
                    pCellButton.style.paddingRight = '2px';
                    pCellButton.appendChild(pButton);
                    pButton.src = 'Images/Icons/cross.png';
                    pButton.alt = 'Remove';
                    pButton.title = 'Remove';
                    pButton.style.display = 'none';
                    pCell.innerHTML = parameters[p].text;

                    if (fc.editableDisplay && filter.removable != false) {
                        pRow.onmouseover = function () {
                            var pButton = this.getElementsByTagName('img')[0];
                            if (pButton) {
                                pButton.style.display = 'block';
                            }
                        }
                        pRow.onmouseout = function () {
                            var pButton = this.getElementsByTagName('img')[0];
                            if (pButton) {
                                pButton.style.display = 'none';
                            }
                        }

                        pButton.parameter = parameters[p];
                        pButton.onclick = function () {
                            this.parameter.remove();
                            fc.toTable(fc.argsForTable, fc.tableGrouping);
                        }
                    }
                }
            }
        }
    }

    var filter = function (args) {
        var mf = this;
        this.groups = {};
        this.name = '';

        //run through args and setup filter
        for (var key in args) {
            if (args.hasOwnProperty(key)) {
                this[key] = args[key];
            }
        }

        this.display = true;

        this.remove = function () {
            fc.filters.remove(this);
        }

        //setup parameters
        this.parameters = [];
        this.parameters.indexOf = function (object) {
            for (var i = 0; i <= this.length - 1; i++) {
                if (this[i] == object) {
                    return i;
                }
            }
            return null;
        }
        this.parameters.add = function (value, text) {
            var nParameter = { value: value, text: text };
            nParameter.remove = function () {
                mf.parameters.remove(this);
            }
            mf.parameters.push(nParameter);
            return nParameter;
        }
        this.parameters.insert = function (value, text, index) {
            var nParameter = { value: value, text: text };
            mf.parameters.splice(i, 0);
            return nParameter;
        }
        this.parameters.remove = function (parameter) {
            var i = mf.parameters.indexOf(parameter);
            if (mf["removable"] != undefined) {
                if (mf["removable"] == false) {
                }
                else {
                    mf.parameters.splice(i, 1);
                }
            }
            else {
                mf.parameters.splice(i, 1);
            }
            if (mf.parameters.length == 0) {
                mf.remove();
            }
        }
        this.parameters.removeAt = function (index) {
            if (fc.filters[index]["removable"] != undefined) {
                if (fc.filters[index]["removable"] == false) {
                }
                else {
                    mf.parameters.splice(index, 1);
                }
            }
            else {
                mf.parameters.splice(index, 1);
            }
        }
        this.parameters.findByValue = function (value) {
            for (var i = 0; i <= mf.parameters.length - 1; i++) {
                if (mf.parameters[i].value == value) {
                    return mf.parameters[i];
                }
            }
            return null;
        }
        this.parameters.findByText = function (text) {
            for (var i = 0; i <= mf.parameters.length - 1; i++) {
                if (mf.parameters[i].text == text) {
                    return mf.parameters[i];
                }
            }
            return null;
        }
        this.parameters.toString = function (delimeter, blnText) {
            try {
                var outPut = [];
                for (var i = 0; i <= this.length - 1; i++) {
                    if (blnText) {
                        outPut.push(this[i].text);
                    } else {
                        outPut.push(this[i].value);
                    }
                }
                outPut = outPut.join(delimeter);

                return outPut;
            }
            catch (e) {
                throw (e);
            }
        }
        this.toJSON = function () {
            try {
                var jsStr = toJSONString(mf);

                if (jsStr == undefined) {
                    jsStr = '{null:null}';
                }
                return jsStr;
            }
            catch (e) {
                throw (e);
            }
        }
        //-------------------
    }
    var findFilter = function (filterList, args) {
        //loop through filters and check for a match

        var filtersFoundByName = [];
        var filtersFoundByGroup = [];
        var filtersFound = [];

        for (var i = 0; i <= filterList.length - 1; i++) {
            if (args.name) {
                if (args.name == filterList[i].name) {
                    filtersFoundByName.push(filterList[i]);
                }
            }
            if (args.groups) {
                var hasGroupsInSearch = false;
                for (var groupName in args.groups) {
                    if (filterList[i].groups[groupName]) {
                        if (filterList[i].groups[groupName] == args.groups[groupName]) {
                            hasGroupsInSearch = true;
                        }
                        else {
                            hasGroupsInSearch = false;
                            break;
                        }
                    }
                }
                if (hasGroupsInSearch) {
                    filtersFoundByGroup.push(filterList[i]);
                }
            }
        }

        if (args.name && args.groups) {
            var filtersFound = [];
            for (var i = 0; i <= filtersFoundByName.length - 1; i++) {
                for (var y = 0; y <= filtersFoundByGroup.length - 1; y++) {
                    if (filtersFoundByName[i] == filtersFoundByGroup[y]) {
                        filtersFound.push(filtersFoundByName[i]);
                    }
                }
            }
        }
        else {
            if (args.name) {
                filtersFound = filtersFoundByName;
            }
            if (args.groups) {
                filtersFound = filtersFoundByGroup;
            }
        }

        filtersFound.find = function (args) {
            return findFilter(filtersFound, args);
        }
        filtersFound.findWithGroup = function (groups) {
            return findWithGroup(filtersFound, groups);
        }
        filtersFound.removable = function (value) {
            filtersRemovable(this, value);
        }
        filtersFound.clear = function () {
            clearFilters(fc.filters);
        }

        return filtersFound;
    }
    var findWithGroup = function (filterList, groups) {
        var foundFilters = [];
        for (var i = 0; i <= filterList.length - 1; i++) {
            var hasAllGroups = true;
            for (var g = 0; g <= groups.length - 1; g++) {
                if (filterList[i].groups[groups[g]] == undefined) {
                    hasAllGroups = false;
                    break;
                }
            }
            if (hasAllGroups) {
                foundFilters.push(filterList[i]);
            }
        }

        foundFilters.find = function (args) {
            return findFilter(foundFilters, args);
        }
        foundFilters.findWithGroup = function (groups) {
            return findWithGroup(foundFilters, groups);
        }
        foundFilters.removable = function (value) {
            filtersRemovable(this, value);
        }
        foundFilters.clear = function () {
            clearFilters(fc.filters);
        }

        return foundFilters;
    }
    var filtersRemovable = function (filters, value) {
        for (var i = 0; i <= filters.length - 1; i++) {
            filters[i].removable = value;
        }
    }
    var clearFilters = function (filters) {
        for (var i = 0; i <= filters.length - 1; i++) {
            if (filters[i].removable !== false) {
                filters.splice(i, 1);
                i--;
            }
        }
    }
    var toJSONString = function (obj) {
        var t = typeof (obj);
        if (t != "object" || obj === null) {
            //simple data type
            if (t == "string") obj = '"' + obj + '"';
            return String(obj);
        }
        else {
            //recurse array or object
            var n, v, json = [], arr = (obj && obj.constructor == Array);
            for (n in obj) {
                v = obj[n]; t = typeof (v);

                if (t == "string") v = '"' + v + '"';
                else if (t == "object" && v !== null) v = toJSONString(v);

                if (t != "function") {
                    json.push((arr ? "" : '"' + n + '":') + String(v));
                }
            }
            return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");
        }
    }
    var groupedFilters = function (filtersToShow, groups, gF, displayed) {
        if (!gF) {
            var gF = {};
        }
        if (typeof (groups) == 'string') {
            groups = groups.split(',');
        }

        var gValues = groupValues(filtersToShow, groups[0], displayed);
        gF["item"] = {};
        gF["item"]['values'] = [];
        gF["item"]['name'] = groups[0];
        for (var g = 0; g <= gValues.length - 1; g++) {
            gF["item"]['values'].push({ value: gValues[g] });
        }

        var subGroups = [];
        for (var i = 1; i <= groups.length - 1; i++) {
            subGroups.push(groups[i]);
        }

        for (var i = 0; i <= gF["item"].values.length - 1; i++) {

            var gFilter = {};

            gFilter[groups[0]] = gF["item"].values[i].value;

            var nFiltersToShow = filtersToShow.find({ groups: gFilter });
            gF["item"]['values'][i]["filters"] = nFiltersToShow;

            if (subGroups.length > 0) {
                nFiltersToShow = nFiltersToShow.findWithGroup(subGroups);
                if (nFiltersToShow.length > 0) {
                    gF["item"]['values'][i]["subGroup"] = groupedFilters(nFiltersToShow, subGroups, gF["item"]['values'][i]["subGroup"]);
                }
            }
        }
        return gF;
    }
    var groupValues = function (filtersToShow, groupName, displayed) {
        var groupValues = [];
        for (var i = 0; i <= filtersToShow.length - 1; i++) {
            if (filtersToShow[i].groups[groupName] != undefined) {
                var groupValue = filtersToShow[i].groups[groupName];
                var exists = false;
                for (var y = 0; y <= groupValues.length - 1; y++) {
                    if (groupValues[y] == groupValue) {
                        exists = true;
                        break;
                    }
                }
                if (!exists) {
                    if (filtersToShow[i].display == displayed || displayed == undefined) {
                        groupValues.push(groupValue);
                    }
                }
            }
        }

        groupValues.sort();
        groupValues.reverse();

        return groupValues;
    }

    var filterInOtherGroupings = function (filtersToShow, args, groupName) {
        var filtersInGroupings = filtersToShow.find(args);
        for (var i = 0; i <= filterInOtherGroupings.length - 1; i++) {

        }
    }
}