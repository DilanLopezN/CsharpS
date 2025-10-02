define(["app/Widgets/Grids/Grid/grid", "app/Widgets/Buttons/Button/button", "app/Widgets/DropDowns/DropDownMenu/dropDownMenu",
        "app/Widgets/DropDowns/CheckedMultiSelect/checkedMultiSelect"],
    function (Grid, Button, DropDownMenu, CheckedMultiSelect) {

        var factoryComponentes = {
            elementosCriados: []
        }

        /****Controls createComponentes*********/
        var controls = {
            button: function (options) {
                var el = Button.createButton(options);
                return el;
            },
            grid: function (options) {
                var el = Grid.createGrid(options);
                return el;
            },
            dropdownmenu: function (options) {
                var el = DropDownMenu.createDropDownMenu(options);
                return el;
            },
            checkedmultiselect: function(options) {
                var el = CheckedMultiSelect.createCheckedMultiSelect(options);
                return el;
            }
        };
        /****Fim Controls createComponentes*********/

        function CoreException(options, message) {
            this.message = options.type + message;
        }

        return {
            create: function (options) {
                var type = options.type ? options.type.toLowerCase() : undefined;

                if (!type || !controls[type]) {
                    throw new CoreException(options, " não é suportado pelo sistema." );
                }

                return controls[type](options);
            },

            createAll: function (options) {
                if (hasValue(options)) {
                    for (var i = 0; i < options.componentes.length; i++) {
                        factoryComponentes.elementosCriados[options.componentes[i].name] =
                            this.create(options.componentes[i]);
                    }
                }
            }

        };
    }
);