"use strict";
(function (window, $) {

    /// Your code goes below
    ///////////////////////////////////////////////////////////////////////////////////

    $(function () {
        document.addEventListener("WvPbManager_Design_Loaded", function (event) {
            if (event && event.payload && event.payload.component_name === "Reomi.Erp.Plugins.Sale.Components.HierarchicalOptionList"){
                console.log("Reomi.Erp.Plugins.Sale.Components.HierarchicalOptionList Design loaded");
            }
        });
    });

    $(function () {
        document.addEventListener("WvPbManager_Design_Unloaded", function (event) {
            if (event && event.payload && event.payload.component_name === "Reomi.Erp.Plugins.Sale.Components.HierarchicalOptionList"){
                console.log("Reomi.Erp.Plugins.Sale.Components.HierarchicalOptionList Design unloaded");
            }
        });
    });


    $(function () {
        document.addEventListener("WvPbManager_Options_Loaded", function (event) {
            if (event && event.payload && event.payload.component_name === "Reomi.Erp.Plugins.Sale.Components.HierarchicalOptionList") {
                console.log("Reomi.Erp.Plugins.Sale.Components.HierarchicalOptionList Options loaded");
            }
        });
    });

    $(function () {
        document.addEventListener("WvPbManager_Options_Unloaded", function (event) {
            if (event && event.payload && event.payload.component_name === "Reomi.Erp.Plugins.Sale.Components.HierarchicalOptionList"){
                console.log("Reomi.Erp.Plugins.Sale.Components.HierarchicalOptionList Options unloaded");
            }
        });
    });

    //////////////////////////////////////////////////////////////////////////////////
    /// You code is above

})(window, jQuery);