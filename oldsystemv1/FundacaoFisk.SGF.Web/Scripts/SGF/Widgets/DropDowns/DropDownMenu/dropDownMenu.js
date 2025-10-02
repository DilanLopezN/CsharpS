define(
    [ 
        "dijit/DropDownMenu", 
        "dijit/form/DropDownButton",
        "dijit/MenuItem", 
        "dijit/MenuSeparator", 
        "dijit/PopupMenuItem",
        "dojo/dom",
        "dojo/_base/array",
    ],

    function ( DropDownMenu, DropDownButton, MenuItem, MenuSeparator, PopupMenuItem, dom, array) {

        function createDropDownMenu(options) {
            var el = { elementCreate: {}, elementTemplateCreate: {} }
            el.elementCreate = createDomDropDownMenu(options);

            return el;
        }


        /**
         * createDropDownMenu     
         * @param {(JSON)options} options 
         * @returns {(createDropDownMenu)el} 
         */
        function createDomDropDownMenu(options) {
            var menu = new DropDownMenu({ style: options.style });

            array.forEach(options.menusItem, function (item, i) {
                var acao = new MenuItem({
                    label: item.label,
                    onClick: function (evt) {
                        item.onClick(evt);
                    }
                });
                menu.addChild(acao);
                
            });
            

            var el = new DropDownButton({
                label: options.label,
                name: options.name,
                dropDown: menu,
                id: options.id
            });

            dom.byId(options.linksAcoesId).appendChild(el.domNode);

            return el;
        }

        return {
            createDropDownMenu: createDropDownMenu
        };

    }
);