var TODOS = 0;
var NORMAL = 1, PPT = 3;
var SITUACAO = 3;
var _FILES = [];
var _DATA_GRID_ANEXO = [];

function montarCadastroMailMarketing(funcao) {
    //Criação da Grade
    require([
        "dojo/_base/xhr",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/query",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dijit/MenuBar",
        "dijit/PopupMenuBarItem",
        "dijit/MenuItem",
        "dijit/DropDownMenu",
        "dojo/data/ItemFileReadStore",
        "dijit/registry",
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
        "dojox/editor/plugins/CollapsibleToolbar",
        "dojo/window",
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom,
            MenuBar, PopupMenuBarItem, MenuItem, DropDownMenu, ItemFileReadStore, registry, Editor, FontChoice, TextColor, LinkDialog, ViewSource, ShowBlockNodes, Preview, Blockquote, NewPage, FindReplace, ToolbarLineBreak, CollapsibleToolbar, windowUtils) {
        ready(function () {
            try {

                // Configura o rodapé da mensagem:
                dojo.xhr.get({
                    url: Endereco() + "/api/escola/getRodapeSysApp",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    var mensagem = jQuery.parseJSON(data).retorno;
                    dojo.byId('rodape_fixo_msg').innerHTML = mensagem;
                });


                loadPesqSexo(Memory, dijit.byId("nm_sexo"));
                loadMesNascimento(Memory, dijit.byId('nm_mes_nascimento'));
                loadPeriodo(Memory,ItemFileReadStore, registry.byId('cbPeriodo'));

                //Criação dos botões:
                new Button({
                    label: "Enviar", iconClass: 'dijitEditorIcon dijitEditorIconEmail', onClick: function () {
                        enviarComporMensagem(windowUtils);
                    }
                }, "btEnviar");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        limparComporMensagem();
                    }
                }, "btCancelar");
                new Button({
                    label: "Inserir Imagens", iconClass: 'dijitEditorIcon dijitEditorIconUpload', onClick: function () {
                        inserirImagem();
                    }
                }, "btInserirImagem");                

                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { dijit.byId("dlgInserirImagem").hide(); } }, "fechar");

                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        incluirImagemEditor();
                    }
                }, "incluir");

                new Button({
                    label: "Anexar", iconClass: 'dijitEditorIcon dijitEditorIconUpload', onClick: function () {
                        apresentaMensagem('apresentadorMensagemDocumentoAnexo', "");                       

                        dijit.byId("dlgAnexarDocumento").show();
                        var dados = [];

                        for (var i = 0; i < _DATA_GRID_ANEXO.length; i++) {
                            dados.push(_DATA_GRID_ANEXO[i]);
                        }

                        dijit.byId("gridDocumentoAnexo").setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));
                    }
                }, "btnAnexarDocumento");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        dijit.byId("upIncluirDocumento")._files = null;
                        dijit.byId("dlgAnexarDocumento").hide();
                    }
                }, "fecharDialog");

                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        var newArrFile = [];
                        if (!hasValue(dijit.byId("gridDocumentoAnexo").store.objectStore.data.length))
                            _FILES = [];
                        else {
                            for (var i = 0; i < dijit.byId("gridDocumentoAnexo").store.objectStore.data.length; i++) {
                                newArrFile.push(dijit.byId("gridDocumentoAnexo").store.objectStore.data[i]._file);
                            }
                            _FILES = newArrFile;
                        }

                        _DATA_GRID_ANEXO = dijit.byId("gridDocumentoAnexo").store.objectStore.data;
                        dijit.byId("dlgAnexarDocumento").hide();
                    }
                }, "btnIncluirDocumento");

                dijit.byId('upIncluirDocumento').on("change", function (evt) {
                    try {
                        var mensagensWeb = new Array();
                        var files = dijit.byId("upIncluirDocumento")._files;
                        apresentaMensagem("apresentadorMensagemDocumentoAnexo", null);

                        if (hasValue(files) && files.length > 0) {

                            for (var i = 0; i < files.length; i++) {

                                if ((hasValue(dijit.byId("gridDocumentoAnexo")) && hasValue(dijit.byId("gridDocumentoAnexo").store.objectStore.data) &&
                                        dijit.byId("gridDocumentoAnexo").store.objectStore.data.length == 5) || dijit.byId("upIncluirDocumento")._files.length > 5) {

                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroLimiteMaxAnexoAtingido);
                                    apresentaMensagem("apresentadorMensagemDocumentoAnexo", mensagensWeb);

                                    if (dijit.byId("upIncluirDocumento")._files.length > 5)
                                        dijit.byId("upIncluirDocumento")._files = null;
                                    return false;
                                }

                                if (hasValue(files[i]) && files[i].size > 500000) {
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorAnexoExcedeuTamanhoMailMarketing);
                                    apresentaMensagem("apresentadorMensagemDocumentoAnexo", mensagensWeb);
                                    return false;
                                }
                                if (!verificarExtensaoArquivo(files[i].name, true)) {
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtesaoErradoAnexo);
                                    apresentaMensagem("apresentadorMensagemDocumentoAnexo", mensagensWeb);
                                    return false;
                                }
                                if (window.FormData !== undefined) {

                                    if (hasValue(dijit.byId("gridDocumentoAnexo"))) {
                                        var gridDocumentoAnexo = dijit.byId("gridDocumentoAnexo");
                                        var id_array = geradorIdDocumentoAnexo(dijit.byId("gridDocumentoAnexo"));
                                        files[i].id = id_array;

                                        gridDocumentoAnexo.store.objectStore.data.push({
                                            id: id_array,
                                            name: files[i].name,
                                            type: files[i].type,
                                            size: (Math.round(files[i].size * 0.001) + " Kb"),
                                            _file: files[i]
                                        });                                     
                                        
                                        gridDocumentoAnexo.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridDocumentoAnexo.store.objectStore.data }) }));
                                        gridDocumentoAnexo.startup();
                                    }
                                }
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                var gridDocumentoAnexo = new EnhancedGrid({
                    store: dojo.data.ObjectStore({ objectStore: new Memory({ data: [] }) }),
                    structure:
                    [
                        { name: "Tipo Arquivo", field: "type", width: "15%" },
                        { name: "Nome", field: "name", width: "45%", styles: "text-align: center;" },
                        { name: "Tamanho", field: "size", width: "12%", styles: "text-align: center;" },
                        {
                            name: "Excluir",
                            field: "_item",
                            width: '100px',
                            styles: "text-align: center;",
                            formatter: function (item) {
                                var btn = new dijit.form.Button({
                                    label: "Excluir",
                                    onClick: function () {
                                        try {
                                            var gridDocumentoAnexo = dijit.byId("gridDocumentoAnexo");
                                            removeObjSort(gridDocumentoAnexo.store.objectStore.data, "id", item.id);

                                            dijit.byId("upIncluirDocumento")._files = null;
                                            var dataStore = new ObjectStore({ objectStore: new Memory({ data: gridDocumentoAnexo.store.objectStore.data }) });
                                            gridDocumentoAnexo.setStore(dataStore);
                                            gridDocumentoAnexo.update();

                                        } catch (e) {

                                        }
                                    }
                                });
                                return btn;
                            }
                        }
                    ]               
                }, "gridDocumentoAnexo");
                gridDocumentoAnexo.startup();


                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                            montarGridPesquisaTurmaFK(function () {
                                abrirTurmaFK(xhr, ObjectStore, Memory, Cache, JsonRest);
                            });
                        else
                            abrirTurmaFK(xhr, ObjectStore, Memory, Cache, JsonRest);
                    }
                }, "pesProTurmaFK");
                btnPesquisar(dojo.byId("pesProTurmaFK"));

                dijit.byId("cbTipoTurma").on("change", function (tipo) {
                    try {
                        if (!hasValue(tipo, true) || tipo < 0)
                            dijit.byId("cbTipoTurma").set("value", TODOS);
                        else {
                            var pesSituacao = dijit.byId("cbSituacaoTurma");
                            if (tipo == PPT) {
                                dojo.byId("trTurmaFilha").style.display = "";
                                dojo.byId("lblTurmaFilhas").style.display = "";
                                dojo.byId("divPesTurmasFilhas").style.display = "";
                                loadSituacaoTurmaPPT();
                                pesSituacao.set("value", 1);
                                pesSituacao.set("disabled", false);
                            } else {
                                dojo.byId("trTurmaFilha").style.display = "none";
                                dojo.byId("lblTurmaFilhas").style.display = "none";
                                dojo.byId("divPesTurmasFilhas").style.display = "none";
                                dijit.byId("pesTurmasFilhas").set("checked", false);
                                loadSituacaoTurma(Memory);
                                pesSituacao.set("value", 1);
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("pesTurmasFilhas").on("click", function (e) {
                    try {
                        var pesSituacao = dijit.byId("cbSituacaoTurma");
                        if (dijit.byId("cbTipoTurma").displayedValue == "Personalizada" && this.checked) {
                            loadSituacaoTurmaPPT();
                            loadSituacaoTurma(Memory);
                            pesSituacao.set("value", 1);
                        } else if (dijit.byId("cbTipoTurma").displayedValue == "Personalizada" && !this.checked) {
                            loadSituacaoTurma(dojo.store.Memory);
                            loadSituacaoTurmaPPT();
                            pesSituacao.set("value", 1);
                            pesSituacao.set("disabled", false);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                //Configura que quando o usuario selecionar o tipo PPT marcara automaticamente turmas filhas.
                dijit.byId("tipoTurmaFK").on("change", function (e) {
                    try {
                        dijit.byId("pesTurmasFilhasFK").set("checked", false);
                        dijit.byId('pesTurmasFilhasFK').set('disabled', false);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                new Button({
                    label: "Limpar", iconClass: '', disabled: true, type: "reset", onClick: function () {
                        dojo.byId("cd_turma_pesquisa").value = 0;
                        dojo.byId("cbTurma").value = "";
                        dijit.byId('limparProTurmaFK').set("disabled", true);

                        dijit.byId('cbTipoTurma').set("disabled", false);
                        dijit.byId('cbSituacaoTurma').set("disabled", false);

                        apresentaMensagem('apresentadorMensagem', null);
                    }
                }, "limparProTurmaFK");

                if (hasValue(document.getElementById("limparProTurmaFK"))) {
                    document.getElementById("limparProTurmaFK").parentNode.style.minWidth = '40px';
                    document.getElementById("limparProTurmaFK").parentNode.style.width = '40px';
                }

                dijit.byId('uploader').on("change", function (evt) {
                    try {
                        var mensagensWeb = new Array();
                        var files = dijit.byId("uploader")._files;
                        apresentaMensagem("apresentadorMensagemInserirImagem", null);
                        if (hasValue(files) && files.length > 0) {
                            if (hasValue(files[0]) && files[0].size > 1000000) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoExcedeuTamanhoMailMarketing);
                                apresentaMensagem("apresentadorMensagemInserirImagem", mensagensWeb);
                                dojo.byId('url_imagem_name').innerHTML = "<div style=\"font-weight:bold; color: #393939\">Imagem Selecionada: Nenhuma.</div>";
                                dojo.byId('url_imagem').innerHTML = "";
                                dijit.byId("uploader").reset();
                                return false;
                            }
                            if (!verificarExtensaoArquivo(files[0].name)) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtesaoErradaArquivo);
                                apresentaMensagem("apresentadorMensagemInserirImagem", mensagensWeb);
                                dojo.byId('url_imagem_name').innerHTML = "<div style=\"font-weight:bold; color: #393939\">Imagem Selecionada: Nenhuma.</div>";
                                dojo.byId('url_imagem').innerHTML = "";
                                dijit.byId("uploader").reset();
                                return false;
                            }
                            if (window.FormData !== undefined) {
                                var data = new FormData();
                                data.append("UploadedImage", files[0]);
                                $.ajax({
                                    type: "POST",
                                    url: Endereco() + "/api/emailMarketing/uploadImage",
                                    ansy: false,
                                    headers: { Authorization: Token() },
                                    contentType: false,
                                    processData: false,
                                    data: data,
                                    success: function (results) {
                                        results = jQuery.parseJSON(results);
                                        dojo.byId('url_imagem').innerHTML = EnderecoAbsoluto() + "/api/emailMarketing/getPhoto?nome=" + results;
                                        dojo.byId('url_imagem_name').innerHTML = "<div style=\"font-weight:bold; color: #393939\">Imagem Selecionada: </div>" + results;
                                    },
                                    error: function (error) {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Impossível fazer upload de foto. Verifique se o tamanho dela é abaixo de 1 MB.");
                                        apresentaMensagem("apresentadorMensagemInserirImagem", mensagensWeb);
                                        dojo.byId('url_imagem_name').innerHTML = "<div style=\"font-weight:bold; color: #393939\">Imagem Selecionada: Nenhuma.</div>";
                                        dojo.byId('url_imagem').innerHTML = "";
                                        dijit.byId("uploader").reset();
                                    }
                                });
                            } else {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Impossível fazer upload de foto. Verifique se o tamanho dela é abaixo de 1 MB.");
                                apresentaMensagem("apresentadorMensagemInserirImagem", mensagensWeb);
                                dojo.byId('url_imagem_name').innerHTML = "<div style=\"font-weight:bold; color: #393939\">Imagem Selecionada: Nenhuma.</div>";
                                dojo.byId('url_imagem').innerHTML = "";
                                dijit.byId("uploader").reset();
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoCamposMesclagemNome = new MenuItem({
                    label: "#nomecompleto#",
                    onClick: function () { incluiTextoEditor('mensagemComporMsg', "#nomecompleto#"); }
                });
                menu.addChild(acaoCamposMesclagemNome);
                var acaoCamposMesclagemSobrenome = new MenuItem({
                    label: "#primeironome#",
                    onClick: function () { incluiTextoEditor('mensagemComporMsg', "#primeironome#"); }
                });
                menu.addChild(acaoCamposMesclagemSobrenome);

                var button = new DropDownButton({
                    label: "Campos de Mesclagem",
                    name: "acoesCamposMesclagem",
                    dropDown: menu,
                    id: "acoesCamposMesclagem"
                });
                dojo.byId("linkCamposMesclagem").appendChild(button.domNode);                

                //Montagem do editor de html:
                var editor = new Editor({
                    height: '315px',
                    plugins: ['collapsibletoolbar', 'undo', 'redo', '|', 'cut', 'copy', 'paste', '|', 'bold', 'italic', 'underline', 'strikethrough', '|', 'insertOrderedList', 'insertUnorderedList', 'indent', 'outdent',
                                              '|', 'justifyLeft', 'justifyRight', 'justifyCenter', 'justifyFull', '|', 'foreColor', 'hiliteColor', '|', 'createLink', 'insertImage', '|',
                                              'newpage', /*'save',*/'showBlockNodes', 'preview', 'findreplace',
                                              { name: 'viewSource', stripScripts: true, stripComments: true }, 'selectAll', '|', 'fontName', 'fontSize', { name: 'formatBlock', plainText: true }]
                }, "mensagemComporMsg");
                editor.startup();
                dijit.byId("mensagemComporMsg").set("required", true);

                //Redimensiona os botões do editor de texto:
                var botoes_editor = dojo.byId('mensagemComporMsg').children[0].children[0];
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


                // Declare my own so I can pass in my plugins list without resoring to globals.
                dojo.declare("custom.Editor", [dijit.Editor], {
                    constructor: function () {
                        this.plugins = plugins;
                        this.height = "100%";
                        this.inherited(arguments);
                    }
                });
                dijit.byId("dialogListagemEnderecos").on("Show", function (e) { dijit.byId("gridListagemEnderecoFK").update(); });
                if (hasValue(funcao))
                    funcao.call();
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function incluirImagemEditor() {
    var campo_selecao = dijit.byId('mensagemComporMsg').selection.getParentElement();
    if (hasValue(campo_selecao)) {
        campo_selecao.innerHTML += "<img src='" + dojo.byId('url_imagem').innerHTML + "'></img>";
        dijit.byId('mensagemComporMsg').value = dijit.byId('mensagemComporMsg').editNode.innerHTML;
    }
        //
}

function incluiTextoEditor(idEditor, texto) {
    var campo_selecao = dijit.byId(idEditor).selection.getParentElement();
    if (hasValue(campo_selecao))
        campo_selecao.innerHTML += texto;
    else
        caixaDialogo(DIALOGO_AVISO, msgErroApontamentoEditor, null);
}

function montaTemasProntos(DropDownMenu, MenuItem, DropDownButton) {
    dojo.xhr.get({
        url: Endereco() + "/api/emailMarketing/getTemasProntos",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            dados = jQuery.parseJSON(dados);
            if (hasValue(dados) && hasValue(dados.retorno)) {
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoTemasProntos;

                for (var i = 0; i < dados.retorno.length; i++) {
                    acaoTemasProntos = new MenuItem({
                        id: dados.retorno[i].id,
                        label: dados.retorno[i].name,
                        onClick: function () { aplicaTemaPronto(this.id) }
                    });
                    menu.addChild(acaoTemasProntos);
                }
                var button = new DropDownButton({
                    label: "Temas Prontos",
                    name: "acoesTemasProntos",
                    dropDown: menu,
                    id: "acoesTemasProntos"
                });
                dojo.byId("linkTemasProntos").appendChild(button.domNode);
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    }, function (error) {
        apresentaMensagem('apresentadorMensagem', error.response.data);
    });
}

function aplicaTemaPronto(tema) {
    dojo.xhr.get({
        url: Endereco() + "/api/emailMarketing/getTemaPronto?tema=" + tema,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            apresentaMensagem('apresentadorMensagem', dados);
            dados = jQuery.parseJSON(dados);
            if (hasValue(dados) && hasValue(dados.retorno))
                dijit.byId('mensagemComporMsg').set('value', dados.retorno);
        }
        catch (e) {
            postGerarLog(e);
        }
    }, function (error) {
        apresentaMensagem('apresentadorMensagem', error.response.data);
    });
}

function verificarExtensaoArquivo(nomeArquivo, isPdf) {
    var valido = true;
    var achou = false;
    var TamanhoString = nomeArquivo.length;
    var extensao = nomeArquivo.substr(TamanhoString - 4, TamanhoString);

    if (hasValue(isPdf) && isPdf)
        var ext = new Array('.PDF');
    else
        var ext = new Array('.JPG', 'JPEG', '.GIF', '.PNG', '.BMP');

    for (var i = 0; i < ext.length; i++)
        if (extensao.toUpperCase() == ext[i]) {
            achou = true;
            break;
        }

    if (!achou)
        valido = false;
    return valido;
}

function loadMesNascimento(Memory, id) {
    try {
        var stateStore = new Memory({
            data: [
                    { name: "Selecione", id: 0 },
                    { name: "Janeiro", id: 1 },
                    { name: "Fevereiro", id: 2 },
                    { name: "Março", id: 3 },
                    { name: "Abril", id: 4 },
                    { name: "Maio", id: 5 },
                    { name: "Junho", id: 6 },
                    { name: "Julho", id: 7 },
                    { name: "Agosto", id: 8 },
                    { name: "Setembro", id: 9 },
                    { name: "Outubro", id: 10 },
                    { name: "Novembro", id: 11 },
                    { name: "Dezembro", id: 12 }
            ]
        });
        id.store = stateStore;
        id.set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadProdutoAtual() {
    require([
       "dojo/_base/xhr",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore"
    ], function (xhr, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                xhr.get({
                    url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=0&cd_produto=null",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    var produtos = jQuery.parseJSON(data).retorno;
                    var w = registry.byId("cbProdutoAtual");
                    var produtoCb = [];
                    if (produtos != null || produtos.length > 0)
                        $.each(produtos, function (index, val) {
                            produtoCb.push({
                                "cd_produto": val.cd_produto + "",
                                "no_produto": val.no_produto
                            });
                        });
                    var store = new ItemFileReadStore({
                        data: {
                            identifier: "cd_produto",
                            label: "no_produto",
                            items: produtoCb
                        }
                    });
                    w.setStore(store, []);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadCursoAtual() {
    require([
        "dojo/_base/xhr",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore"
    ], function (xhr, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var w = registry.byId("cbCursoAtual");
                var produtoCb = [];
                var store = new ItemFileReadStore({
                    data: {
                        identifier: "cd_curso",
                        label: "no_curso",
                        items: produtoCb
                    }
                });
                w.setStore(store, []);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadPeriodo(Memory, ItemFileReadStore, w) {
    try {
        var stateStore = [
                   { name: "Manhã", id: "1" },
                   { name: "Tarde", id: "2" },
                   { name: "Noite", id: "3" }
        ];
        var store = new ItemFileReadStore({
            data: {
                identifier: "id",
                label: "name",
                items: stateStore
            }
        });
        w.setStore(store, []);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mostraPeriodoAluno(obj) {

    if (dijit.byId('ckAluno').checked)
    {
        setMenssageMultiSelect(DIA, 'cbDiasMailMarketing');
        setMenssageMultiSelect(SITUACAO, 'cbSituacaoAluno');
        loadDiasSemanaMailMarketing();
        loadSituacaoAluno();
        loadTipoTurma();
        loadSituacaoTurma();
        if (!hasValue(dijit.byId("cbComoConheceu").store.data))
            loadMidia();

        showP('trMatricula', true);
        showP('trTurma', true);
        
    }
    else {
        showP('trMatricula', false);
        showP('trTurma', false);
    }

    if (dijit.byId('ckAluno').checked || dijit.byId('ckPessoaRelacionada').checked) {
	    setMenssageMultiSelect(SITUACAO, 'cbSituacaoAluno');
	    loadSituacaoAluno();
        showP('trSituacao', true);
    }
    else {
	    showP('trSituacao', false);
    }

    if (dijit.byId('ckPendentes').checked || dijit.byId('ckAluno').checked) {
        showP('trProdutoAtual', true);
        showP('trComoConheceu', true);
    }
    else {
        showP('trProdutoAtual', false);
        showP('trComoConheceu', false);
    }
    show('tdCursoAtual1');
    show('tdCursoAtual2');
    show('tdPeriodoAluno1');
    show('tdPeriodoAluno2');
}

function mostraPeriodoProspect(obj) {

    if (dijit.byId('ckPendentes').checked) {
        if (!hasValue(dijit.byId("cbComoConheceu").store.data))
            loadMidia();

        if (!hasValue(dijit.byId("cbFaixaEtaria").store.data))
            loadFaixaEtaria();

        showP('trComoConheceu', true);
        showP('trFaixaEtaria', true);
        showP('trProspectAtivo', true);
    }
    else {
        showP('trComoConheceu', false);
        showP('trFaixaEtaria', false);
        showP('trProspectAtivo', false);
        dijit.byId("cbComoConheceu").set("value", "");
    }

    if (dijit.byId('ckPendentes').checked || dijit.byId('ckAluno').checked) {
        showP('trProdutoAtual', true);
        showP('trComoConheceu', true);
    }
    else {
        showP('trProdutoAtual', false);
        showP('trComoConheceu', false);
    }

    show('tdPeriodoProspect1');
    show('tdPeriodoProspect2');
}

function mostraProfissao(obj) {
    require([
       "dojo/store/Memory",
        "dojo/ready",
    ], function (Memory, ready) {
        ready(function () {
            if (!obj.checked) {
                showP('tdTipoProfissao1', false);
                showP('tdTipoProfissao2', false);
            } else {
                show('tdTipoProfissao1');
                show('tdTipoProfissao2');
                loadProfissao(Memory, dijit.byId('tdTipoProfissao'));
            }
        });
    });
}

function loadProfissao(Memory, id) {
    try {
        var stateStore = new Memory({
            data: [
                    { name: "Todos", id: 0 },
                    { name: "Colaborador Cyber", id: 1 },
                    { name: "Coordenador", id: 2 },
                    { name: "Professor", id: 3 }
            ]
        });
        id.store = stateStore;
        id.set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mostraCamposPessoaRelacionada(obj)
{
    if (!obj.checked) {
	    showP('trRelacao', false);
    }
    else {
        showP('trRelacao', true);
        if (!hasValue(dijit.byId("cbGrauParentesco").store))
            loadQualifRelacionamento();
        if (!hasValue(dijit.byId("cbRelacao").store))
            loadTipoPapel();

    }

    if (dijit.byId('ckAluno').checked || dijit.byId('ckPessoaRelacionada').checked) {
	    setMenssageMultiSelect(SITUACAO, 'cbSituacaoAluno');
	    loadSituacaoAluno();
	    showP('trSituacao', true);
    }
    else {
	    showP('trSituacao', false);
    }

    
}

function loadQualifRelacionamento() {
    require([
        "dojo/_base/xhr",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
    ], function (xhr, ready, registry, ItemFileReadStore) {
        ready(function () {
            xhr.get({
                url: Endereco() + "/api/pessoa/getQualifRelacionamento",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (response) {
                try {
                    dados = jQuery.parseJSON(response);
                    var domGrauParent = registry.byId("cbGrauParentesco");
                    var arrGrauParent = [];

                    if (hasValue(dados)) {
                        $.each(dados.retorno.qualifRelacionamentos, function (index, val) {
                            arrGrauParent.push({
                                "cd_qualif_relacionamento": val.cd_qualif_relacionamento,
                                "no_qualif_relacionamento": val.no_qualif_relacionamento
                            });
                        });
                    }
                    var store = new ItemFileReadStore({
                        data: {
                            identifier: "cd_qualif_relacionamento",
                            label: "no_qualif_relacionamento",
                            items: arrGrauParent
                        }
                    });
                    domGrauParent.setStore(store, []);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
        });
    });
}

function loadTipoPapel() {
    require([
        "dojo/_base/xhr",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
    ], function (xhr, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var EDUCADORESFORNECEDORES = 1;
                var PAISEMPRESAS = 2;
                var papeis = new Array(2);
                papeis[0] = EDUCADORESFORNECEDORES;
                papeis[1] = PAISEMPRESAS;
                require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
                    xhr.post({
                        url: Endereco() + "/api/pessoa/getPapelByTipo?tipo=" + papeis,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        postData: ref.toJson(papeis)
                    }).then(function (data) {
                        try {
                            var domRelacao = registry.byId("cbRelacao");
                            var arrRelacao = [];

                            if (hasValue(data)) {
                                $.each(data.retorno, function (index, val) {
                                    arrRelacao.push({
                                        "cd_papel": val.cd_papel,
                                        "no_papel": val.no_papel
                                    });
                                });
                            }
                            var store = new ItemFileReadStore({
                                data: {
                                    identifier: "cd_papel",
                                    label: "no_papel",
                                    items: arrRelacao
                                }
                            });
                            domRelacao.setStore(store, []);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });       
}

var STATUS_MIDIA_TODOS = 0;
function loadMidia() {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getMidia?status=" + STATUS_MIDIA_TODOS,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cbComoConheceu', 'cd_midia', 'no_midia');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function loadDiasSemanaMailMarketing() {
    require([
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
    ], function (ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var w = registry.byId("cbDiasMailMarketing");
                var retorno = [];
                retorno.push({ value_dia: 1 + "", no_dia: "Domingo" });
                retorno.push({ value_dia: 2 + "", no_dia: "Segunda" });
                retorno.push({ value_dia: 3 + "", no_dia: "Terça" });
                retorno.push({ value_dia: 4 + "", no_dia: "Quarta" });
                retorno.push({ value_dia: 5 + "", no_dia: "Quinta" });
                retorno.push({ value_dia: 6 + "", no_dia: "Sexta" });
                retorno.push({ value_dia: 7 + "", no_dia: "Sabado" });
                var store = new ItemFileReadStore({
                    data: {
                        identifier: "value_dia",
                        label: "no_dia",
                        items: retorno
                    }
                });
                w.setStore(store, []);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadSituacaoAluno() {
    require([
       "dojo/ready",
       "dijit/registry",
       "dojo/data/ItemFileReadStore",
    ], function (ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var w = registry.byId('cbSituacaoAluno');
                var dados = [
                             { name: "Aguardando Matrícula", id: "9" },
                             { name: "Matriculado", id: "1" },
                             { name: "Rematriculado", id: "8" },
                             { name: "Desistente", id: "2" },
                             { name: "Encerrado", id: "4" },
                             { name: "Movido", id: "0" },
                             { name: "Inativo", id: "20" },
                ]
                var store = new ItemFileReadStore({
                    data: {
                        identifier: "id",
                        label: "name",
                        items: dados
                    }
                });
                w.setStore(store, []);
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadTipoTurma() {
    require([
        "dojo/store/Memory",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
    ], function (Memory, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var statusStoreTipo = new Memory({
                    data: [
                    { name: "Todas", id: 0 },
                    { name: "Regular", id: 1 },
                    { name: "Personalizada", id: 3 }
                    ]
                });
                dijit.byId("cbTipoTurma").store = statusStoreTipo;
                dijit.byId("cbTipoTurma").set("value", 0);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadSituacaoTurma() {
    require([
        "dojo/store/Memory",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
    ], function (Memory, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var statusStore = new Memory({
                    data: [
                    { name: "Todas", id: 0 },
                    { name: "Turmas em Andamento", id: 1 },
                    { name: "Turmas em Formação", id: 3 },
                    { name: "Turmas Encerradas", id: 2 }
                    ]
                });

                dijit.byId("cbSituacaoTurma").store = statusStore;
                dijit.byId("cbSituacaoTurma").set("value", 1);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadSituacaoTurmaPPT() {
    require([
           "dojo/store/Memory",
           "dojo/ready",
           "dijit/registry",
           "dojo/data/ItemFileReadStore",
    ], function (Memory, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var statusStore = new Memory({
                    data: [
                    { name: "Turmas Ativas", id: 1 },
                    { name: "Turmas Inativas", id: 2 }
                    ]
                });

                dijit.byId("cbSituacaoTurma").store = statusStore;
                dijit.byId("cbSituacaoTurma").set("value", 1);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadFaixaEtaria() {
    dojo.xhr.get({
        url: Endereco() + "/api/Coordenacao/getModalidades?criterios=&cd_modalidade=",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cbFaixaEtaria', 'cd_modalidade', 'no_modalidade');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function inserirImagem() {
    var offSetX = 20;
    var offSetY = 20;
    var dlgInserirImagem = dijit.byId("dlgInserirImagem");
    var campo_selecao = dijit.byId('mensagemComporMsg').selection.getParentElement();
    if (!hasValue(campo_selecao))
        caixaDialogo(DIALOGO_AVISO, msgErroApontamentoEditor, null);
    else {
        $("#" + "dlgInserirImagem").css("top", dlgInserirImagem.pageX + offSetX).css("left", dlgInserirImagem.pageY + offSetY);
        dijit.byId("dlgInserirImagem").show();
        dojo.byId("dlgInserirImagem").style.display = "block";
        dojo.byId("url_imagem").innerHTML = "";
        dijit.byId("uploader")._files = [];
        dijit.byId("uploader").reset();
        dojo.byId('url_imagem_name').innerHTML = "<div style=\"font-weight:bold; color: #393939\">Imagem Selecionada: Nenhuma.</div>";
    }
}

function limparComporMensagem() {
    clearForm("formComporMensagem");
    dijit.byId('mensagemComporMsg').set('value', "");
    if (hasValue(dijit.byId('cbPeriodo')) && hasValue(dijit.byId('cbPeriodo').store))
        dijit.byId('cbPeriodo').setStore(dijit.byId('cbPeriodo').store, [0]);
    if (hasValue(dijit.byId('cbProdutoAtual')) && hasValue(dijit.byId('cbProdutoAtual').store))
        dijit.byId('cbProdutoAtual').setStore(dijit.byId('cbProdutoAtual').store, [0]);
    if (hasValue(dijit.byId('cbCursoAtual')) && hasValue(dijit.byId('cbCursoAtual').store))
        dijit.byId('cbCursoAtual').setStore(dijit.byId('cbCursoAtual').store, [0]);
}

function atualizaCursosAtuais(objProdutoAtual) {
    require([
        "dojo/dom",
        "dojo/dom-attr",
        "dojo/_base/xhr",
        "dojox/json/ref",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
        "dijit/layout/TabContainer",
        "dijit/layout/ContentPane",
        "dojox/form/CheckedMultiSelect"
    ], function (dom, domAttr, xhr, ref, ready, registry, ItemFileReadStore) {
        ready(function () {
            var cd_produtos = "";
            if (hasValue(dijit.byId('cbProdutoAtual').value))
                cd_produtos = dijit.byId('cbProdutoAtual').value.join();
            xhr.get({
                url: Endereco() + "/api/curso/getCursosProdutos?cd_produtos=" + cd_produtos,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                var cursos = jQuery.parseJSON(data).retorno;
                var w = registry.byId("cbCursoAtual");
                var cursosCb = [];
                var valores_antigos = dijit.byId("cbCursoAtual").value;

                apresentaMensagem("apresentadorMensagem", data);
                if (cursos != null || cursos.length > 0)
                    $.each(cursos, function (index, val) {
                        cursosCb.push({
                            "cd_curso": val.cd_curso + "",
                            "no_curso": val.no_curso
                        });
                    });
                var store = new ItemFileReadStore({
                    data: {
                        identifier: "cd_curso",
                        label: "no_curso",
                        items: cursosCb
                    }
                });
                w.setStore(store, []);
                dijit.byId("cbCursoAtual").set('value', valores_antigos);
            });
        });
    });
}

function montarMensagem() {
    var cd_produtos = new Array();
    if (hasValue(dijit.byId('cbProdutoAtual').value))
        for (var i = 0; i < dijit.byId('cbProdutoAtual').value.length; i++)
            cd_produtos.push({ cd_produto: dijit.byId('cbProdutoAtual').value[i] });

    var cd_cursos = new Array();
    if (hasValue(dijit.byId('cbCursoAtual').value))
        for (var i = 0; i < dijit.byId('cbCursoAtual').value.length; i++)
            cd_cursos.push({ cd_curso: dijit.byId('cbCursoAtual').value[i] });

    var cd_periodos_prospects = new Array();
    if (hasValue(dijit.byId('cbPeriodo').value))
        for (var i = 0; i < dijit.byId('cbPeriodo').value.length; i++)
            cd_periodos_prospects.push({ id_periodo: parseInt(dijit.byId('cbPeriodo').value[i]) });

    var malasDiretaCadastro = new Array();

    if (dijit.byId('ckPendentes').checked)
        malasDiretaCadastro.push({ id_cadastro: 1 });
    if (dijit.byId('ckAluno').checked)
        malasDiretaCadastro.push({ id_cadastro: 2 });
    if (dijit.byId('ckCliente').checked)
        malasDiretaCadastro.push({ id_cadastro: 3 });
    if (dijit.byId('ckPessoaRelacionada').checked)
        malasDiretaCadastro.push({ id_cadastro: 4 });
    if (dijit.byId('ckFuncionarioProfessor').checked)
        malasDiretaCadastro.push({ id_cadastro: 5 });

    var dadosRetorno = {
        MalasDiretaCadastro: malasDiretaCadastro,
        id_sexo: dijit.byId('nm_sexo').value == 0 ? "" : dijit.byId('nm_sexo').value,
        nm_mes_nascimento: dijit.byId('nm_mes_nascimento').value == 0 ? "" : dijit.byId('nm_mes_nascimento').value,
        MalasDiretaCurso: cd_cursos,
        MalasDiretaProduto: cd_produtos,
        MalasDiretaPeriodo: cd_periodos_prospects,
        dc_hr_inicial: dojo.byId('timeIni').value,
        dc_hr_fim: dojo.byId('timeFim').value,
        dc_assunto: dojo.byId('dc_assunto').value,
        tx_msg_body: !hasValue(dijit.byId('mensagemComporMsg').value) ? dijit.byId('mensagemComporMsg').selection.getParentElement().innerHTML : dijit.byId('mensagemComporMsg').value,
        tx_msg_footer: dojo.byId('rodape_fixo_msg').innerHTML,
        id_tipo_profissao: hasValue(dijit.byId('tdTipoProfissao').value) ? dijit.byId('tdTipoProfissao').value : 0,

        cd_tipo_papel: hasValue(dijit.byId("cbRelacao").value) ? convertArrInt(dijit.byId("cbRelacao").value) : [],
        cd_midia: hasValue(dijit.byId("cbComoConheceu").value) ? dijit.byId("cbComoConheceu").value : 0,
        dt_matricula: hasValue(dijit.byId("dtaMatricula").value) ? dijit.byId("dtaMatricula").value : null,
        dt_inicio_turma: hasValue(dijit.byId("dtaInicialTurm").value) ? dijit.byId("dtaInicialTurm").value : null,
        cd_tipo_turma: hasValue(dijit.byId("cbTipoTurma").value) ? dijit.byId("cbTipoTurma").value : 0,
        existe_turmas_filhas: document.getElementById("pesTurmasFilhas").checked,
        cd_situacao_turma: hasValue(dijit.byId("cbSituacaoTurma").value) ? dijit.byId("cbSituacaoTurma").value : 0,
        cd_situacao_aluno: hasValue(dijit.byId("cbSituacaoAluno").value) ? convertArrInt(dijit.byId("cbSituacaoAluno").value) : [],
        cd_faixa_etaria: hasValue(dijit.byId("cbFaixaEtaria").value) ? dijit.byId("cbFaixaEtaria").value : 0,
        cd_turma: hasValue(dojo.byId("cd_turma_pesquisa").value) ? parseInt(dojo.byId("cd_turma_pesquisa").value) : 0,
        cd_dia_semana: hasValue(dojo.byId("cbDiasMailMarketing").value) ? convertArrInt(dojo.byId("cbDiasMailMarketing").value) : [],
        cd_grau_parentesco: hasValue(dijit.byId("cbGrauParentesco").value) ? convertArrInt(dijit.byId("cbGrauParentesco").value) : [],
        id_prospect_inativo: hasValue(dijit.byId("ckProspectAtivo")) ? dijit.byId("ckProspectAtivo").checked : false,
    }
    return dadosRetorno;
}

function convertArrInt(situacao) {
    var arrInt = [];

    if (!hasValue(situacao))
        return arrInt;

    for (var i = 0; i < situacao.length; i++) {
        arrInt.push(parseInt(situacao[i]));
    }
    return arrInt;
}

function enviarComporMensagem(windowUtils) {
    try {
        apresentaMensagem('apresentadorMensagem', "");
        //Valida os campos:
        if ((!dijit.byId('ckPendentes').checked && !dijit.byId('ckAluno').checked && !dijit.byId('ckCliente').checked && !dijit.byId('ckPessoaRelacionada').checked && !dijit.byId('ckFuncionarioProfessor').checked)
                | !mostrarMensagemCampoValidado(windowUtils, dijit.byId('dc_assunto'))
				|| !hasValue(dijit.byId('mensagemComporMsg').selection.getParentElement()) || dijit.byId('mensagemComporMsg').selection.getParentElement().innerHTML == "&nbsp;") {
            dijit.byId('tagDestinatario').set('open', true);
            var mensagensWeb = new Array();

            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroSelecionarDestinatario);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            return false;
        }
        if (hasValue(dojo.byId('timeIni').value) || hasValue(dojo.byId('timeFim').value))
            if (dijit.byId('timeIni').value > dijit.byId('timeFim').value || !hasValue(dojo.byId('timeIni').value) || !hasValue(dojo.byId('timeFim').value)) {
                dijit.byId('tagDestinatario').set('open', true);
                var mensagensWeb = new Array();

                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroHoraInicialMaiorFinal);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }

        var mala_direta = new montarMensagem();

        showCarregando();
        require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post({
                url: Endereco() + "/api/emailMarketing/postComporMensagem",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(mala_direta)
            }).then(function (data) {
                try {
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data);
                    if (hasValue(data) && hasValue(data.retorno) && hasValue(data.retorno.ListasEnderecoMala,true))
                        eventoListarEnderecos(data.retorno);

                }
                catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function enviarArquivoAnexo() {
    try {
        var files = _FILES;
        if (hasValue(files) && files.length > 0) {
            if (window.FormData !== undefined) {
                showCarregando();

                var data = new FormData();
                for (i = 0; i < files.length; i++) {
                    data.append("anexo" + i, files[i]);
                }

                $.ajax({
                    type: "POST",
                    url: Endereco() + "/api/emailMarketing/uploadArquivoAnexo",
                    ansy: false,
                    headers: { Authorization: Token() },
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (nome_arquivos_anexo) {
                        try {
                            showCarregando();
                            enviarMensagens(nome_arquivos_anexo);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    error: function (error) {
                        showCarregando();
                        apresentaMensagem('apresentadorMensagem', error);
                        return false;
                    }
                });
            } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Impossível fazer upload de arquivo.");
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
            }
        } else { enviarMensagens(null); }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function enviarMensagens(nome_arquivos_anexo) {
    try {
        apresentaMensagem('apresentadorMensagemListagemEnderecos', "");
        var malaDireta = dijit.byId('gridListagemEnderecoFK').malaDireta;
        malaDireta.json_nome_arquivos = nome_arquivos_anexo;

        malaDireta.ListasEnderecoMalaComNaoInscritos = cloneArray(malaDireta.ListasEnderecoMala);
        var itensSelecionados = malaDireta.ListasEnderecoMala = dijit.byId('gridListagemEnderecoFK').itensSelecionados;
        malaDireta.ListasEnderecoMalaView = cloneArray(malaDireta.ListasEnderecoMala);
        malaDireta.ListasEnderecoMala = cloneArray(malaDireta.ListasEnderecoMala);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return;
        }

        showCarregando();
        require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post({
                url: Endereco() + "/mailMarketing/postComporMensagemEnviar",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(malaDireta)
            }).then(function (data) {
                try {
                    apresentaMensagem('apresentadorMensagemListagemEnderecos', data);
                    showCarregando();
                }
                catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemListagemEnderecos', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarNovaMensagem() {
    try {
        limparComporMensagem();
        loadProdutoAtual();
        loadCursoAtual();
        //Metodos para criação do link
        montaTemasProntos(dijit.DropDownMenu, dijit.MenuItem, dijit.form.DropDownButton);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxInscritoFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridListagemEnderecoFK'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEnderecosFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_cadastro", grid._by_idx[rowIndex].item.cd_cadastro);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_cadastro', 'enderecoSelecionadoFK', -1, 'selecionaTodosEnderecosFK', 'selecionaTodosEnderecosFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_cadastro', 'enderecoSelecionadoFK', " + rowIndex + ", '" + id + "', 'selecionaTodosEnderecosFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarEAtualizarFKListagemEnderecos(funcao, data) {
    try {
        var gridListagemEnderecoFK = new dojox.grid.EnhancedGrid({
            store: dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
            structure:
            [
                { name: "<input id='selecionaTodosEnderecosFK' style='display:none'/>", field: "enderecoSelecionadoFK", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxInscritoFK },
                { name: "Nome", field: "no_pessoa", width: "35%", styles: "min-width:70px;" },
                { name: "E-Mail", field: "dc_email_cadastro", width: "35%", styles: "min-width:70px;" },
                { name: "Cadastro", field: "tipo_cadastro", width: "25%", styles: "min-width:70px;" },
                { name: "cd_cadastro", field: "cd_cadastro", width: "0%", styles: "min-width:70px;display:none;" }
            ],
            canSort: true,
            selectionMode: "single",
            noDataMessage: "Nenhum registro encontrado.",
            plugins: {
                pagination: {
                    pageSizes: ["22", "44", "88", "100", "All"],
                    description: true,
                    sizeSwitch: true,
                    pageStepper: true,
                    defaultPageSize: "22",
                    gotoButton: true,
                    maxPageStep: 5,
                    position: "button"
                }
            }
        }, "gridListagemEnderecoFK");
        gridListagemEnderecoFK.startup();
        gridListagemEnderecoFK.on("StyleRow", function (row) {
            try {
                var item = gridListagemEnderecoFK.getItem(row.index);
                if (hasValue(item) && hasValue(item.cd_lista_endereco_mala, true)) {
                    var inscrito = false;
                    var itens = cloneArray(gridListagemEnderecoFK.itensSelecionados);
                    quickSortObj(itens, 'cd_cadastro');
                    if (hasValue(itens && itens.length > 0) &&
                        row.node.children[0].children[0].children[0].children[4].innerHTML != '...' && eval(row.node.children[0].children[0].children[0].children[4].innerHTML)) {
                        var posicao = binaryObjSearch(itens, 'cd_cadastro', parseInt(row.node.children[0].children[0].children[0].children[4].innerHTML));
                        if (hasValue(posicao, true))
                            inscrito = true;
                    }
                    if (inscrito)
                        row.customClasses += " GreenRow"
                    else
                        row.customClasses += " RedRow"
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        new dijit.form.Button({
            label: "Enviar", iconClass: 'dijitEditorIcon dijitEditorIconEmail', onClick: function () {
                enviarArquivoAnexo();
            }
        }, "btEnviarFK");
        montarLegendaListagemEnderecos("chartListagemFK", "legendListagemFK");
        if (hasValue(funcao))
            funcao.call();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarLegendaListagemEnderecos(idChar, idLegend) {
    dojo.ready(function () {
        try {
            var chart = new dojox.charting.Chart(idChar);
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();
            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, idLegend);
            chart.addSeries("Não Inscrito", [1], { fill: "#F06167" });
            chart.addSeries("Inscrito", [1], { fill: "#98FB98" }); //GreenRow
            chart.render();
            dijit.byId(idLegend).refresh();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirTurmaFK(xhr, ObjectStore, Memory, Cache, JsonRest) {
    try{
        dojo.byId("trAluno").style.display = "none";
        dojo.byId("idOrigemCadastro").value = MAILMARKETING;

        dijit.byId("proTurmaFK").show();
        limparFiltrosTurmaFK();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            //if (hasValue(dijit.byId("pesProfessorFK").value)) {
            //    dijit.byId('pesProfessorFK').set('disabled', true);
            //} else {
            //    dijit.byId('pesProfessorFK').set('disabled', false);
            //}
        }
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        pesquisarTurmaFK();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFK(fieldID, fieldNome) {
    try {
        var valido = true;
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("cd_turma_pesquisa").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
            dojo.byId("cbTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
            dijit.byId('limparProTurmaFK').set("disabled", false);


            dijit.byId("cbTipoTurma").set("value", 0);
            dijit.byId('cbSituacaoTurma').set("value", 1);
            dijit.byId('cbTipoTurma').set("disabled", true);
            dijit.byId('cbSituacaoTurma').set("disabled", true);
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function geradorIdDocumentoAnexo(gridDocumentoAnexo) {
    try {
        if (hasValue(gridDocumentoAnexo)) {
            var id = gridDocumentoAnexo.store.objectStore.data.length;
            var itensArray = gridDocumentoAnexo.store.objectStore.data.sort(function byOrdem(a, b) { return a.id - b.id; });
            if (id == 0)
                id = 1;
            else if (id > 0)
                id = itensArray[id - 1].id + 1;
            return id;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
