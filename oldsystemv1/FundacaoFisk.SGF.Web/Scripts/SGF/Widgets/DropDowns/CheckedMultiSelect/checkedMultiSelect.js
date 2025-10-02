define(
    [
        "dojo/data/ItemFileReadStore",
        "dojo/on",
        "dojo/dom",
        "dojo/store/Memory",
        "dojo/store/Observable",
        "dojo/data/ObjectStore",
        "dojox/form/CheckedMultiSelect",
        "dojo/domReady!"
    ],

    function (ItemFileReadStore, on, dom, Memory, Observable, DataStore, CheckedMultiSelect) {

        function createCheckedMultiSelect(options) {
            var el = { elementCreate: {}, elementTemplateCreate: {} }
            el.elementCreate = createDomCheckedMultiSelect(options);

            return el;
        }



        function functions(fs) {
            var self = {};
            fs.forEach(function (f) {
                self[f.length] = f;
            });
            return function () {
                self[arguments.length].apply(this, arguments);
            };
        }



        function MensagemMultiSelect() {
            this.setarMensagemCheckedMultiSelect = functions([
                function (label, name, abreviation) {
                    try {
                        new DescricaoMultiSelect().configDescCheckedMultiSelect("Escolher " + label + "(" + abreviation + ").", "{num} " + label + "(" + abreviation + ") selecionado(" + abreviation + ")", dijit.byId(name));
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (label, name, abreviation, mostrarlabel, size) {
                    try {
                        new DescricaoMultiSelect().configDescCheckedMultiSelect("Escolher " + label + "(" + abreviation + ").", "{num} " + label + "(" + abreviation + ") selecionado(" + abreviation + ")", dijit.byId(name), mostrarlabel, size);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (label, name, abreviation, mostrarlabel, size, opcao) {
                    try {
                        new DescricaoMultiSelect().configDescCheckedMultiSelect("Escolher " + label + "(" + abreviation + ").", "{num} " + label + "(" + abreviation + ") selecionado(" + abreviation + ")", dijit.byId(name), mostrarlabel, size, opcao);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            ]);
        }

        function DescricaoMultiSelect() {
            this.configDescCheckedMultiSelect = functions([
                function (descricaoEscolher, descricaoNroRegistros, componente) {
                    try {
                        if (componente.value.length > 0) {
                            componente._nlsResources.multiSelectLabelText = descricaoNroRegistros;
                        }
                        else
                            componente._nlsResources.multiSelectLabelText = descricaoEscolher;
                        componente._updateSelection();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (descricaoEscolher, descricaoNroRegistros, componente, mostrarlabel, size) {
                    try {
                        if (componente.value.length > 0) {
                            if (mostrarlabel == true) {
                                componente._nlsResources.multiSelectLabelText = '';
                                for (var i = 1; i < componente.options.length; i++) {
                                    if (componente.options[i].selected == true) {
                                        componente._nlsResources.multiSelectLabelText = componente._nlsResources.multiSelectLabelText +
                                            componente.options[i].label +
                                            ',';
                                    }
                                }

                                if (componente._nlsResources.multiSelectLabelText.length > size) {
                                    componente._nlsResources.multiSelectLabelText =
                                        componente._nlsResources.multiSelectLabelText.substring(0, size) + "...";

                                } else {
                                    componente._nlsResources.multiSelectLabelText =
                                        componente._nlsResources.multiSelectLabelText.slice(0, -1);
                                }

                            } else
                                componente._nlsResources.multiSelectLabelText = descricaoNroRegistros;
                        } else
                            componente._nlsResources.multiSelectLabelText = descricaoEscolher;
                        componente._updateSelection();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (descricaoEscolher, descricaoNroRegistros, componente, mostrarlabel, size, Opcao) {
                    try {
                        if (componente.value.length > 0) {
                            if (mostrarlabel == true) {
                                componente._nlsResources.multiSelectLabelText = '';
                                for (var i = Opcao; i < componente.options.length; i++) {
                                    if (componente.options[i].selected == true) {
                                        if (i < componente.value.length && i < componente.options.length - 1)
                                            componente._nlsResources.multiSelectLabelText = componente._nlsResources.multiSelectLabelText + componente.options[i].label + ',';
                                        else
                                            componente._nlsResources.multiSelectLabelText = componente._nlsResources.multiSelectLabelText + componente.options[i].label;
                                    }
                                }
                            }
                            else
                                componente._nlsResources.multiSelectLabelText = descricaoNroRegistros;
                        }
                        else
                            componente._nlsResources.multiSelectLabelText = descricaoEscolher;
                        componente._updateSelection();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            ]);
        }



        /**
         * createDropDownMenu     
         * @param {(JSON)options} options 
         * @returns {(createDropDownMenu)el} 
         */
        function createDomCheckedMultiSelect(options) {
            var store = new dojo.data.ItemFileReadStore({
                data: {
                    identifier: options.storeIdentifier,
                    label: options.storeLabel,
                    items: options.store
                }
            });
            var el = new CheckedMultiSelect({
                dropDown: options.dropDown || true,
                multiple: options.multiple || true,
                label: options.label || "",
                required: options.require ||"required",
                onAfterAddOptionItem: function (item, option) {
                    
                    try {
                        require(["dojo/on"], function (on) {

                            if (option.value) {
                                on(item, "click", function (evt) {
                                   
                                    var checkedMultiSelect = dijit.byId(options.name);

                                    var contSelected = 0;
                                    for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                                        if (checkedMultiSelect.options[i].selected == true) {
                                            contSelected++;
                                        }
                                    }

                                    if (option.value == (options.valueTodos || "0")) {
                                        if (option.selected == true) {
                                            for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                                                checkedMultiSelect.options[i].selected = true;
                                            }
                                        } else {
                                            for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                                                checkedMultiSelect.options[i].selected = false;
                                            }
                                        }
                                    } else {
                                        if (checkedMultiSelect.options.length - 1 == contSelected &&
                                            checkedMultiSelect.options[0].selected == false) {
                                            for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                                                checkedMultiSelect.options[i].selected = true;
                                            }
                                        } else {
                                            checkedMultiSelect.options[0].selected = false;
                                        }
                                    }

                                });
                            }
                        });

                    } catch (e) {

                        postGerarLog(e);
                    }
                }
            }, options.name);

            el.startup();
            el.setStore(store, []);



           

            /*
            var MensageMulti = function () {
                this.strategy = "";
            };

            MensageMulti.prototype = {
                setStrategy: function (strategy) {
                    this.strategy = strategy;
                },

                doAction: function (package) {
                    return this.strategy.doAction(package);
                }
            };

            var Strategy1 = function () {
                this.doAction = function (package) {
                    
                }
            };

            var Strategy2 = function () {
                this.doAction = function (package) {
                    // calculations...
                    console.log("$39.40");
                }
            };

            var Strategy3 = function () {
                this.doAction = function (package) {
                    // calculations...
                    console.log("$29.95");
                }
            };
            
            

            // the 3 strategies
            var strategias = [
                { item: new Strategy1() },
                { item: new Strategy2() },
                { item: new Strategy3() }
            ];

            var getStrategia = function(position) {
                return strategias[position].item;
            }

            var mensageMulti = new MensageMulti();

            mensageMulti.setStrategy(getStrategia(options.strategyMensage));
            mensageMulti.doAction(package);
            
            */
            new MensagemMultiSelect().setarMensagemCheckedMultiSelect(options.configMensage.label, options.name, options.configMensage.abreviation);
            dijit.byId(options.name).on("change",
                function (e) {
                    new MensagemMultiSelect().setarMensagemCheckedMultiSelect(options.configMensage.label, options.name, options.configMensage.abreviation, true, 20);
            });

           // setarMensagemMultiSelect(options.label, options.name, options.abbreviation);
            //dijit.byId(field).on("change",
            //    function (e) {
            //        setarMensagemMultiSelect(PRODUTO, field, true, 20);
            //    });


            return el;
        }

        return {
            createCheckedMultiSelect: createCheckedMultiSelect
        };

    }
);