define(["dojo/dom-construct", "dojo/_base/window", "dijit/form/Button",
       "dojo/dom",
       "dojo/_base/xhr",
       "dijit/registry",
       "dojox/grid/EnhancedGrid",
       "dojox/grid/DataGrid",
       "dojox/grid/enhanced/plugins/Pagination",
       "dojox/grid/enhanced/plugins/IndirectSelection",
       "dojo/store/JsonRest",
       "dojo/data/ObjectStore",
       "dojo/store/Cache",
       "dojo/store/Memory",
],
   function (domConstruct, window, Button, dom,
       xhr, registry, EnhancedGrid, DataGrid, Pagination, IndirectSelection, JsonRest, ObjectStore, Cache, Memory
   ) {

       function createGrid(options) {
           var el = { elementCreate: {}, elementTemplateCreate: {} }

           el.elementCreate = createDomGrid(options);

           return el;
       }

       function ucfirst(str) {
           return str.substr(0, 1).toUpperCase() + str.substr(1);
       }



       /**
        * createGrid     
        * @param {(JSON)options} options 
        * @returns {(Grid)el} 
        */
       function createDomGrid(options) {



           if (hasValue(options.structure)) {
               options.structure.unshift(
                   {
                       name: "<input id='selecionaTodos" + ucfirst(options.name) + "' style='display:none'/>",
                       field: "selecionado" + ucfirst(options.name),
                       width: "25px",
                       styles: "text-align:center; min-width:15px; max-width:20px;",
                       formatter: formatCheckBox
                   }
               );
           }

           if (hasValue(options.idProperty)) {
               myStore = Cache(
                   JsonRest({
                       target: options.target,
                       handleAs: "json",
                       preventCache: true,
                       headers: {
                           "Accept": "application/json",
                           "Content-Type": "application/json",
                           "Authorization": options.token
                       },
                       idProperty: options.idProperty
                   }),
                   Memory({ idProperty: options.idProperty }));

           } else {
               myStore = Cache(
                   JsonRest({
                       target: options.target,
                       handleAs: "json",
                       preventCache: true,
                       headers: {
                           "Accept": "application/json",
                           "Content-Type": "application/json",
                           "Authorization": options.token
                       }
                   }),
                   Memory({}));
           }

           if (hasValue(options.query)) {
                myStore.query(options.query);
           }


           var el = new EnhancedGrid({
                   
               store: options.storeData ? new ObjectStore({ objectStore: new Memory({ data: null }) }) : ObjectStore({ objectStore: myStore }),
               structure: options.structure,
               noDataMessage: options.noDataMessage,
               canSort: options.canSort || true,
               selectionMode: "single",
               plugins: {
                   pagination: {
                       pageSizes: options.pageSizes || ["17", "34", "68", "100", "All"],
                       description: options.description || true,
                       sizeSwitch: options.sizeSwitch || true,
                       pageStepper: options.pageStepper || true,
                       defaultPageSize: options.defaultPageSize || "17",
                       gotoButton: options.gotoButton || true,
                       /*page step to be displayed*/
                       maxPageStep: options.maxPageStep || 5,
                       /*position of the pagination bar*/
                       position: options.position || "button"
                   }
               }
           }, "grid" + ucfirst(options.name));

           el.pagination.plugin._paginator.plugin.connect(el.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
               verificaMostrarTodos(evt, el, (options.cd_property || 'cd_' + options.name), 'selecionaTodos' + ucfirst(options.name));
           });

           require(["dojo/aspect"], function (aspect) {
               aspect.after(el, "_onFetchComplete", function () {
                   // Configura o check de todos:
                   if (dojo.byId('selecionaTodos' + ucfirst(options.name)).type == 'text')
                       setTimeout("configuraCheckBox(false, '" + (options.cd_property || 'cd_' + options.name) + "', 'selecionado" + ucfirst(options.name) + "', -1, 'selecionaTodos" + ucfirst(options.name) + "', 'selecionaTodos" + ucfirst(options.name) + "', '" + "grid" + ucfirst(options.name) + "')", el.rowsPerPage * 3);
               });
           });

           el.canSort = function (col) { return Math.abs(col) == 0; };
           el.on("RowDblClick", function (evt) {
               try {
                   options.RowDblClick(evt);
               } catch (e) {
                   postGerarLog(e);
               }
           }, true);
           el.startup();

           function formatCheckBox(value, rowIndex, obj) {
               try {
                   var gridName = "grid" + ucfirst(options.name);
                   var grid = dijit.byId(gridName);
                   var icon;
                   var id = obj.field + '_Selected_' + rowIndex;
                   var todos = dojo.byId('selecionaTodos' + ucfirst(options.name));

                   if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
                       var indice = binaryObjSearch(grid.itensSelecionados, (options.cd_property || 'cd_' + options.name), grid._by_idx[rowIndex].item[(options.cd_property || 'cd_' + options.name)]);

                       value = value || indice != null; // Item está selecionado.
                   }
                   if (rowIndex != -1)
                       icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

                   setTimeout("configuraCheckBox(" + value + ", '" + (options.cd_property || 'cd_' + options.name) + "', 'selecionado" + ucfirst(options.name) + "', " + rowIndex + ", '" + id + "', 'selecionaTodos" + ucfirst(options.name) + "', '" + gridName + "')", 2);

                   return icon;
               }
               catch (e) {
                   postGerarLog(e);
               }
           };

           return el;
       }

       return {
           createGrid: createGrid
       };

   }
);