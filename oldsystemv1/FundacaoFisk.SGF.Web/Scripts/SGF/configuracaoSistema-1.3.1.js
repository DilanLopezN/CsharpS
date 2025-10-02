function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridCadRodape';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nm_script", grid._by_idx[rowIndex].item.nm_script);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  style='height: 16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nm_script', 'rodapeSelecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'nm_script', 'rodapeSelecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarCadastroRodape() {
    require([
    "dojo/_base/xhr",
    "dojo/dom",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/query",
    "dojo/dom-attr",
    "dijit/form/Button",
    "dojo/ready",
    "dojo/on",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dijit/form/FilteringSelect",
    "dojo/_base/array",
    "dojox/json/ref",
    "dijit/Editor",
    "dijit/_editor/plugins/FontChoice",
    "dijit/_editor/plugins/TextColor",
    "dijit/_editor/plugins/LinkDialog",
    "dijit/_editor/plugins/ViewSource",
    "dojox/editor/plugins/ShowBlockNodes",
    "dojox/editor/plugins/Preview",
    "dojox/editor/plugins/Blockquote",
    "dijit/_editor/plugins/NewPage",
    "dojox/editor/plugins/FindReplace",
    "dojox/editor/plugins/ToolbarLineBreak",
    "dojox/editor/plugins/CollapsibleToolbar"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, domAttr, Button, ready, on, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, array, ref,
        Editor, FontChoice, TextColor, LinkDialog, ViewSource, ShowBlockNodes, Preview, Blockquote, NewPage, FindReplace, ToolbarLineBreak, CollapsibleToolbar) {
        ready(function () {
            try {
                var editor = new Editor({
                    height: '450px',
                    plugins: ['collapsibletoolbar', 'undo', 'redo', '|', 'cut', 'copy', 'paste', '|', 'bold', 'italic', 'underline', 'strikethrough', '|', 'insertOrderedList', 'insertUnorderedList',
                        'indent', 'outdent',
                                              '|', 'justifyLeft', 'justifyRight', 'justifyCenter', 'justifyFull', '|', 'foreColor', 'hiliteColor', '|', 'createLink', 'insertImage', '|',
                                              'newpage', /*'save',*/'showBlockNodes', 'preview', 'findreplace',
                                              { name: 'viewSource', stripScripts: true, stripComments: true }, 'selectAll', '|', 'fontName', 'fontSize', { name: 'formatBlock', plainText: true }]
                }, "mensagemRodape");
                editor.startup();
                dijit.byId("mensagemRodape").set("required", true);
                //Redimensiona os botões do editor de texto:
                var botoes_editor = dojo.byId('mensagemRodape').children[0].children[0];
                configurarLayoutEditor(botoes_editor);
                var editorVerso = new Editor({
                    height: '420px',
                    plugins: ['collapsibletoolbar', 'undo', 'redo', '|', 'cut', 'copy', 'paste', '|', 'bold', 'italic', 'underline', 'strikethrough', '|', 'insertOrderedList', 'insertUnorderedList',
                        'indent', 'outdent',
                                              '|', 'justifyLeft', 'justifyRight', 'justifyCenter', 'justifyFull', '|', 'foreColor', 'hiliteColor', '|', 'createLink', 'insertImage', '|',
                                              'newpage', /*'save',*/'showBlockNodes', 'preview', 'findreplace',
                                              { name: 'viewSource', stripScripts: true, stripComments: true }, 'selectAll', '|', 'fontName', 'fontSize', { name: 'formatBlock', plainText: true }]
                }, "mensagemVersoCartaoPostal");
                editorVerso.startup();
                dijit.byId("mensagemVersoCartaoPostal").set("required", true);
                var botoes_editor_verso = dojo.byId('mensagemVersoCartaoPostal').children[0].children[0];
                configurarLayoutEditor(botoes_editor_verso);
                keepValues();
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        try {
                            keepValues();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarEmailMarketing");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        alterarCamposEmailMarketing();
                    }
                }, "alterarEmailMarketing");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}


function keepValues() {
    try {
        showCarregando();
        apresentaMensagem('apresentadorMensagem', null);
        showEditConfigEmailMarketing();
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function alterarCamposEmailMarketing() {
    try {
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/escola/postAlterarConfigEmailMarketing",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson({
                tx_msg_email: dijit.byId("mensagemRodape").get("value"),
                tx_verso_cartao_postal: dijit.byId("mensagemVersoCartaoPostal").get("value")
            })
        }).then(function (data) {
            try {
                if (!hasValue(data.erro)) {
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    dijit.byId("mensagemRodape").set("value", itemAlterado.tx_msg_email);
                }
                apresentaMensagem('apresentadorMensagem', data);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function showEditConfigEmailMarketing() {
    require(["dojo/request/xhr", "dojox/json/ref", "dojo/ready"], function (xhr, ref, ready) {
        ready(function () {
            try {
                xhr.get(Endereco() + "/api/escola/getConfigEmailMarketingSysApp", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json"
                }).then(function (data) {
                    try {
                        var retorno = jQuery.parseJSON(data).retorno;
                        dijit.byId("mensagemRodape").set("value", retorno.tx_msg_email);
                        dijit.byId("mensagemVersoCartaoPostal").set("value", retorno.tx_verso_cartao_postal);
                        showCarregando();
                    }
                    catch (e) {
                        showCarregando();
                        postGerarLog(e);
                    }
                }, function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagem', error.response.data);
                });
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        });
    });
}

function configurarLayoutEditor(botoes_editor) {
    for (var i = 0; i < botoes_editor.children[0].children[0].children[2].children[0].children[0].children.length; i++)
        if (hasValue(botoes_editor.children[0].children) && hasValue(botoes_editor.children[0].children[0])
                && hasValue(botoes_editor.children[0].children[0].children) && hasValue(botoes_editor.children[0].children[0].children[2])
                && hasValue(botoes_editor.children[0].children[0].children[2].children) && hasValue(botoes_editor.children[0].children[0].children[2].children[0])
                && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children) && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0])
                && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children)
                && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i])
                && botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].localName != "div") {
            botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].style.minWidth = '32px';
            if (hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].children)
                    && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].children[0]))
                botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].children[0].style.minWidth = '32px';
        }
}