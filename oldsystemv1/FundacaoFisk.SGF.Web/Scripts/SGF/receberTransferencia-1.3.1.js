var PESQUISAALUNOFILTRO = 3;

var EnumsTipoStatusTransferenciaAluno = {
    CADASTRADA: 0,
    SOLICITADA: 1,
    APROVADA: 2,
    EFETUADA: 3,
    RECUSADA: 9
}

function formatCheckBoxReceberTransferencia() {

}

function formatCheckBoxReceberTransferencia(value, rowIndex, obj) {
    try {
        var gridName = 'gridReceberTransferencia';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosReceberTransferencia');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_transferencia_aluno", grid._by_idx[rowIndex].item.cd_transferencia_aluno);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  Id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_transferencia_aluno', 'selecionadoReceberTransferencia', -1, 'selecionaTodosReceberTransferencia', 'selecionaTodosReceberTransferencia', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_transferencia_aluno', 'selecionadoReceberTransferencia', " + rowIndex + ", '" + id + "', 'selecionaTodosReceberTransferencia', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAluno(value, rowIndex, obj) {
    var gridName = 'gridAluno'
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosAluno');

    if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
        var indice = binaryObjSearch(grid.itensSelecionados, "Id", grid._by_idx[rowIndex].item.Id);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  Id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'Id', 'selecionadoAluno', -1, 'selecionaTodosAluno', 'selecionaTodosAluno', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'Id', 'selecionadoAluno', " + rowIndex + ", '" + id + "', 'selecionaTodosAluno', '" + gridName + "')", 2);

    return icon;
}


function verificaoInicial() {
    require([
        "dijit/registry",
        "dojo/ready",
        "dojo/on",
        "dojo/require",
        "dojo/domReady!"
    ], function (registry, ready, on) {
        ready(function () {
            try {
                $("#cpf").mask("999.999.999-99");
                $("#raf").mask("a999-999");
                $("#rafCad").mask("9999-999");
                //if (hasValue(dijit.byId("cpf")))
                //    dijit.byId("cpf").on("blur", function (evt) {
                //        try {
                //            //if (trim(dojo.byId("cpf").value) != "" && dojo.byId("cpf").value != "___.___.___-__")
                //            //    if (validarCPF())
                //            //        //validarCPF() ?
                //            //        verificarPessoCPF();
                //        }
                //        catch (e) {
                //            postGerarLog(e);
                //        }
                //    });


            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function montarCadastroReceberTransferencia() {
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
        "dijit/form/FilteringSelect",
        "dojo/_base/array",
        "dojo/promise/all",
        "dojo/Deferred",
        'dojo/_base/lang',
        'dojox/grid/cells/dijit',
        "dijit/Tooltip",
        "dijit/Dialog"
    ],
        function (xhr,
            EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, FilteringSelect, array, all, Deferred, lang, cells, Tooltip) {
            ready(function () {
                try {

                    /*
                     * Loaders
                     */
                    montarStatusTransferencia("cbStatusTransferencia", "1");
                    montarStatusTransferencia("cbRecStatusTransferenciaCad", "1");
                    criarOuCarregarCompFiltering("cbMotivoTransferenciaCad", [], "", null, ready, Memory, FilteringSelect, 'cd_motivo_transferencia_aluno', 'dc_motivo_transferencia_aluno');
                    /*Fim Loaders*/

                    /**
                     * Ações Relacionadas
                     */
                    var menu = new DropDownMenu({ style: "height: 25px" });
                    var acaoEditar = new MenuItem({
                        label: "Editar",
                        onClick: function () {

                            limparFormReceberTransferencias();
                            showCarregando();
                            eventoEditarReceberTransferencia(dijit.byId("gridReceberTransferencia").itensSelecionados, xhr);

                            //limparGrid();
                            hideCarregando();



                        }
                    });
                    menu.addChild(acaoEditar);

                    
                    var acaoEnviarEmail = new MenuItem({
                        label: "Enviar Email",
                        onClick: function () {
                            eventoEnviarEmail(dijit.byId("gridReceberTransferencia").itensSelecionados, xhr);
                        }
                    });
                    menu.addChild(acaoEnviarEmail);


                    var button = new DropDownButton({
                        label: "Ações Relacionadas",
                        name: "acoesRelacionadasReceberTransferencia",
                        dropDown: menu,
                        id: "acoesRelacionadasReceberTransferencia"
                    });
                    dom.byId("linkAcoesReceberTransferencia").appendChild(button.domNode);

                    // Adiciona link de selecionados:
                    menu = new DropDownMenu({ style: "height: 25px" });
                    var menuTodosItens = new MenuItem({
                        label: "Todos Itens",
                        onClick: function () {
                            buscarTodosItens(gridReceberTransferencia,
                                'todosReceberTransferencia',
                                ['pesquisarReceberTransferencia', 'relatorioReceberTransferencia']);
                            PesquisarReceberTransferencia(false);
                        }
                    });
                    menu.addChild(menuTodosItens);

                    var menuItensSelecionados = new MenuItem({
                        label: "Itens Selecionados",
                        onClick: function () {
                            buscarItensSelecionados('gridReceberTransferencia',
                                'selecionadoReceberTransferencia',
                                'cd_transferencia_aluno',
                                'selecionaTodosReceberTransferencia',
                                ['pesquisarReceberTransferencia', 'relatorioReceberTransferencia'],
                                'todosItensReceberTransferencia');
                        }
                    });
                    menu.addChild(menuItensSelecionados);

                    var button = new DropDownButton({
                        label: "Todos Itens",
                        name: "todosItensReceberTransferencia",
                        dropDown: menu,
                        id: "todosItensReceberTransferencia"
                    });
                    dom.byId("linkSelecionadosReceberTransferencia").appendChild(button.domNode);
                    /*-------------Fim Ações relacionadas -----------*/




                    /*
                     * Botões
                     */

                    new Button({
                        label: "Salvar",
                        iconClass: 'dijitEditorIcon dijitEditorIconSave',
                        onClick: function () {
                            IncluirReceberTransferencia();
                        }
                    },
                        "incluirReceberTransferencia");
                    new Button({
                        label: "Salvar",
                        iconClass: 'dijitEditorIcon dijitEditorIconSave',
                        onClick: function () {
                            AlterarReceberTransferencia();
                        }
                    },
                        "alterarReceberTransferencia");
                    

                    new Button({
                        label: "Limpar",
                        iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                        onClick: function () {
                            limparFormReceberTransferencias();
                            //dojo.byId("atendente").value = document.getElementById('nomeUsuario').innerText;
                        }
                    },
                        "limparReceberTransferencia");

                    new Button({
                        label: "Cancelar",
                        iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                        onClick: function () {
                            showCarregando();
                            keepValues(null, null, dijit.byId("gridReceberTransferencia"), null);
                            showCarregando();
                        }
                    },
                        "cancelarReceberTransferencia");
                    new Button({
                        label: "Fechar",
                        iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                        onClick: function () {
                            dijit.byId("cadReceberTransferencia").hide();
                        }
                    },
                        "fecharReceberTransferencia");



                    new Button({
                        label: "",
                        iconClass: 'dijitEditorIconSearchSGF',
                        onClick: function () {
                            apresentaMensagem('apresentadorMensagem', null);
                            PesquisarReceberTransferencia(true);
                        }
                    }, "pesquisarReceberTransferencia");

                    //new Button({
                    //    label: "Novo",
                    //    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    //    onClick: function () {
                    //        try {


                    //            getComponentesReceberTransferencia(ready, Memory, FilteringSelect);

                    //        } catch (e) {
                    //            postGerarLog(e);
                    //            showCarregando();
                    //        }
                    //    }
                    //},
                    //    "novoReceberTransferencia");

                    new Button({
                        label: getNomeLabelRelatorio(),
                        iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                        onClick: function () {
                            var ReceberTransferencia = montarParametrosUrlReceberTransferencia();
                            require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"],
                                function (dom, domAttr, xhr, ref) {
                                    xhr.get({
                                        url: Endereco() +
                                            "/api/Secretaria/GetUrlRelatorioReceberTransferencia?" +
                                            getStrGridParameters('gridReceberTransferencia') +
                                            "&cd_unidade_origem=" + (ReceberTransferencia.cd_turma || 0) + "&no_aluno=" + (ReceberTransferencia.no_aluno || '') + "&nm_raf=" + (ReceberTransferencia.nm_raf || '') + "&cpf=" + (ReceberTransferencia.cpf || '') + "&status_transferencia=" + (ReceberTransferencia.statusTransferencia || '0') + "&dataIni=" + (ReceberTransferencia.dataIni || "") + "&dataFim=" + (ReceberTransferencia.dataFim || ""),
                                        preventCache: true,
                                        handleAs: "json",
                                        headers: {
                                            "Accept": "application/json",
                                            "Content-Type": "application/json",
                                            "Authorization": Token()
                                        }
                                    }).then(function (data) {
                                        abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data,
                                            '1000px',
                                            '750px',
                                            'popRelatorio');
                                    },
                                        function (error) {
                                            apresentaMensagem('apresentadorMensagem', error);
                                        });
                                });
                        }
                    },
                        "relatorioReceberTransferencia");


                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    dojo.query("#_nomePessoaFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisarEscolasFK();
                                    });
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisarEscolasFK();
                                    });
                                    abrirPessoaFK(true);
                                });
                            else
                                abrirPessoaFK(true);


                        }
                    }, "pesUnidade");

                    var pesUnidade = document.getElementById('pesUnidade');
                    if (hasValue(pesUnidade)) {
                        pesUnidade.parentNode.style.minWidth = '18px';
                        pesUnidade.parentNode.style.width = '18px';
                    }

                    new Button({
                        label: "Limpar", iconClass: '', type: "reset", disabled: true,
                        onClick: function () {
                            dojo.byId("no_unidade").value = "";
                            dojo.byId("cdUnidadePesUnidade").value = 0;
                            dijit.byId("limparUnidadePes").set('disabled', true);
                            apresentaMensagem('apresentadorMensagem', null);
                        }
                    }, "limparUnidadePes");
                    if (hasValue(document.getElementById("limparUnidadePes"))) {
                        document.getElementById("limparUnidadePes").parentNode.style.minWidth = '40px';
                        document.getElementById("limparUnidadePes").parentNode.style.width = '40px';
                    }



                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                            try {
                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                    montarGridPesquisaAluno(false, function () {
                                        abrirAlunoFKCad();
                                    });
                                }
                                else
                                    abrirAlunoFKCad();
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }
                    }, "pesAlunoCad");

                    var pesAlunoCad = document.getElementById('pesAlunoCad');
                    if (hasValue(pesAlunoCad)) {
                        pesAlunoCad.parentNode.style.minWidth = '18px';
                        pesAlunoCad.parentNode.style.width = '18px';
                    }

                    //criação do botões pesquisa principal
                    new Button({
                        label: "Limpar", iconClass: '', type: "reset", disabled: true,
                        onClick: function () {
                            dojo.byId("cdAlunoPesCad").value = 0;
                            dojo.byId("noAlunoCad").value = null;
                            dijit.byId("noAlunoCad").value = null;
                            dojo.byId("rafCad").value = "";
                            apresentaMensagem('apresentadorMensagem', null);
                            dijit.byId("limparAlunoPesCad").set('disabled', true);
                        }
                    }, "limparAlunoPesCad");

                    if (hasValue(document.getElementById("limparAlunoPesCad"))) {
                        document.getElementById("limparAlunoPesCad").parentNode.style.minWidth = '40px';
                        document.getElementById("limparAlunoPesCad").parentNode.style.width = '40px';
                    }

                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    dojo.query("#_nomePessoaFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisarEscolasFK();
                                    });
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisarEscolasFK();
                                    });
                                    abrirPessoaFKCad(true);
                                });
                            else
                                abrirPessoaFKCad(true);


                        }
                    }, "pesUnidadeCadOrigem");

                    var pesUnidadeCadOrigem = document.getElementById('pesUnidadeCadOrigem');
                    if (hasValue(pesUnidadeCadOrigem)) {
                        pesUnidadeCadOrigem.parentNode.style.minWidth = '18px';
                        pesUnidadeCadOrigem.parentNode.style.width = '18px';
                    }

                    new Button({
                        label: "Limpar", iconClass: '', type: "reset", disabled: true,
                        onClick: function () {
                            dojo.byId("no_unidade_cad_origem").value = "";
                            dojo.byId("cdUnidadePesUnidadeCadOrigem").value = 0;
                            dijit.byId("limparUnidadePesCadOrigem").set('disabled', true);
                            dojo.byId("emailRecCadDestino").value = "";
                            apresentaMensagem('apresentadorMensagem', null);
                        }
                    }, "limparUnidadePesCadOrigem");
                    if (hasValue(document.getElementById("limparUnidadePesCadOrigem"))) {
                        document.getElementById("limparUnidadePesCadOrigem").parentNode.style.minWidth = '40px';
                        document.getElementById("limparUnidadePesCadOrigem").parentNode.style.width = '40px';
                    }

                    new Button({
                        label: "",
                        iconClass: 'dijitEditorIcon dijitEditorIconDownload',
                        onClick: function () {
                            downloadHistorico();
                        }
                    }, "btnDownHistoricoAlunoCad");

                    new Button({
                        label: "",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                        onClick: function () { visualizarHistorico(); }
                    }, "btnShowHistoricoAlunoCad");

                    var buttonFkArray = ['uploaderHistoricoAluno', 'btnDownHistoricoAlunoCad', 'btnShowHistoricoAlunoCad'];
                    for (var p = 0; p < buttonFkArray.length; p++) {
                        var buttonFk = document.getElementById(buttonFkArray[p]);

                        if (hasValue(buttonFk)) {
                            buttonFk.parentNode.style.minWidth = '18px';
                            buttonFk.parentNode.style.width = '18px';
                        }
                    }

                    new Tooltip({
                        connectId: ["uploaderHistoricoAluno"],
                        label: "Upload",
                        position: ['above']
                    });
                    new Tooltip({
                        connectId: ["btnDownHistoricoAlunoCad"],
                        label: "Download",
                        position: ['above']
                    });
                    new Tooltip({
                        connectId: ["btnShowHistoricoAlunoCad"],
                        label: "Visualizar",
                        position: ['above']
                    });

                    dijit.byId('uploaderHistoricoAluno').on("change", function (evt) {
                        try {
                            var mensagensWeb = new Array();
                            //if (dojo.byId('cd_contrato').value == "0") {
                            //    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgAvisoSalvarContratoparadigitalizar);
                            //    apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
                            //    dijit.byId("uploaderContratoDigitalizado")._files = [];
                            //    return false;
                            //}
                            var files = dijit.byId("uploaderHistoricoAluno")._files;
                            if (hasValue(files) && files.length > 0) {
                                if (dijit.byId("uploaderHistoricoAluno")._files[0].name.length > 128) {
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanho);
                                    apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                    dijit.byId("uploaderHistoricoAluno")._files = [];
                                    return false;
                                } else {
                                    if (hasValue(files[0]) && (files[0].size > 500000)) {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoExcedeuTamanho);
                                        apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                        dijit.byId("uploaderHistoricoAluno")._files = [];
                                        return false;
                                    }
                                    var nomeArquivo = files[0].name;
                                    var extArquivo = nomeArquivo.substr(nomeArquivo.length - 4, nomeArquivo.length);
                                    if (hasValue(extArquivo) && (extArquivo.toLowerCase().indexOf(".pdf") === -1)) {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtensaoFileHistoricoAlunoInvalida);
                                        apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                        dijit.byId("uploaderHistoricoAluno")._files = [];
                                        return false;
                                    }

                                    apresentaMensagem('apresentadorMensagem', null);
                                    dojo.byId("edlayoutHistoricoAlunoCad").value = files[0].name;
                                }
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    /*---------Fim Botões----------------*/


                    /*
                     * Grid ReceberTransferencias
                     */

                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/Secretaria/getReceberTransferenciaAlunoSearch?desc=&cd_unidade_origem=&no_aluno=&nm_raf=&cpf=&status_transferencia=1&dataIni=&dataFim=",
                            handleAs: "json",
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            idProperty: "cd_transferencia_aluno"
                        }),
                        Memory({ idProperty: "cd_transferencia_aluno" }));

                    var gridReceberTransferencia = new EnhancedGrid({
                        store: ObjectStore({ objectStore: myStore }),
                        //store: ObjectStore({ objectStore: new dojo.store.Memory({ data: myStore }) }),
                        structure: [
                            {
                                name: "<input id='selecionaTodosReceberTransferencia' style='display:none'/>",
                                field: "selecionadoReceberTransferencia",
                                width: "5%",
                                styles: "text-align:center; min-width:15px; max-width:20px;",
                                formatter: formatCheckBoxReceberTransferencia
                            },
                            //   { name: "Código", field: "cd_item", width: "5%", styles: "text-align: right; min-width:75px; max-width:75px;" },
                            { name: "Unidade de Origem ", field: "no_unidade_origem", width: "20%", styles: "min-width:80px;" },
                            { name: "Aluno ", field: "no_aluno", width: "30%", styles: "min-width:80px;" },
                            { name: "CPF ", field: "cpf", width: "15%", styles: "min-width:80px;" },
                            { name: "Email Enviado", field: "email_destino_enviado", width: "15%", styles: "min-width:80px;" },
                            { name: "Data", field: "dta_status", width: "15%" },
                            { name: "Motivo", field: "no_motivo_transferencia_aluno", width: "10%", styles: "min-width:80px;" },
                            { name: "RAF", field: "nm_raf", width: "10%", styles: "min-width:80px;" },
                            { name: "Status", field: "no_status", width: "10%", styles: "min-width:80px;" },
                        ],
                        noDataMessage: "Nenhum registro encontrado.",
                        plugins: {
                            pagination: {
                                pageSizes: ["17", "34", "68", "100", "All"],
                                description: true,
                                sizeSwitch: true,
                                pageStepper: true,
                                defaultPageSize: "17",
                                gotoButton: true,
                                /*page step to be displayed*/
                                maxPageStep: 4,
                                /*position of the pagination bar*/
                                position: "button",
                                plugins: { nestedSorting: true }
                            }
                        }
                    },
                        "gridReceberTransferencia"); // make sure you have a target HTML element with this id
                    // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                    gridReceberTransferencia.pagination.plugin._paginator.plugin.connect(gridReceberTransferencia.pagination.plugin._paginator,
                        'onSwitchPageSize',
                        function (evt) {
                            verificaMostrarTodos(evt, gridReceberTransferencia, 'cd_item', 'selecionaTodosReceberTransferencia');
                        });
                    var idGrupoItem = 0;
                    gridReceberTransferencia.startup();
                    gridReceberTransferencia.canSort = function (col) { return Math.abs(col) != 1; };
                    gridReceberTransferencia.on("RowDblClick",
                        function (evt) {
                            try {

                                //limparGridReceberTransferencias();
                                var idx = evt.rowIndex,
                                    item = this.getItem(idx),
                                    store = this.store;

                                limparFormReceberTransferencias();

                                item.cd_transferencia_aluno = item.cd_transferencia_aluno == null ? 0 : item.cd_transferencia_aluno;
                                dojo.byId("cd_transferencia_aluno").value = item.cd_transferencia_aluno;
                                apresentaMensagem('apresentadorMensagem', '');
                                keepValues(null, item, gridReceberTransferencia, false);
                                dijit.byId("cadReceberTransferencia").show();
                                IncluirAlterar(0,
                                    'divAlterarReceberTransferencia',
                                    'divIncluirReceberTransferencia',
                                    null,
                                    'apresentadorMensagemReceberTransferencia',
                                    'divCancelarReceberTransferencia',
                                    'divLimparReceberTransferencia');
                                //limparGrid();



                            } catch (e) {
                                hideCarregando();
                                postGerarLog(e);
                            }
                        }, true);

                    require(["dojo/aspect"],
                        function (aspect) {
                            aspect.after(gridReceberTransferencia,
                                "_onFetchComplete",
                                function () {
                                    // Configura o check de todos:
                                    if (dojo.byId('selecionaTodosReceberTransferencia').type == 'text')
                                        setTimeout(
                                            "configuraCheckBox(false, 'cd_transferencia_aluno', 'selecionadoReceberTransferencia', -1, 'selecionaTodosReceberTransferencia', 'selecionaTodosReceberTransferencia', 'gridReceberTransferencia')",
                                            gridReceberTransferencia.rowsPerPage * 3);
                                });
                        });
                    /*Fim Grid */

                    showCarregando();

                } catch (e) {
                    postGerarLog(e);
                }


            });

        });
}

function downloadHistorico() {
    try {
        var url = Endereco() +
            "/Secretaria/getDownloadArquivoHistoricoTransferenciaAluno?cd_transferencia_aluno=" +
            dojo.byId("cd_transferencia_aluno").value;
        window.open(url);
    } catch (e) {
        postGerarLog(e);
    }
}

function visualizarHistorico() {
    try {
        var url = Endereco() +
            "/Secretaria/getVisualizarArquivoHistoricoTransferenciaAluno?cd_transferencia_aluno=" +
            dojo.byId("cd_transferencia_aluno").value;
        window.open(url);
    } catch (e) {
        postGerarLog(e);
    }
}

function getComponentesReceberTransferencia(ready, Memory, FilteringSelect) {
    showCarregando();
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
        function (dom, xhr, ref) {
            dojo.xhr.get({
                url: Endereco() + "/api/Secretaria/getComponentesReceberTransferenciaCad",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (retorno) {
                try {
                    if (hasValue(retorno)) {
                        console.log(retorno);
                        limparFormReceberTransferencias();
                        dojo.byId('dt_cadastro_transferencia_cad').value = dojo.date.locale.format(new Date(),
                            { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });

                        criarOuCarregarCompFiltering("cbMotivoTransferenciaCad", retorno.MotivosTransferencia, "", null, ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_motivo_transferencia_aluno', 'dc_motivo_transferencia_aluno');
                        dojo.byId("emailRecCadOrigem").value = retorno.emailOrigem;
                        dijit.byId("uploaderHistoricoAluno").set("disabled", true);
                        dijit.byId("btnDownHistoricoAlunoCad").set("disabled", true);
                        dijit.byId("btnShowHistoricoAlunoCad").set("disabled", true);
                        //dojo.byId("atendente").value = document.getElementById('nomeUsuario').innerText;
                        //dojo.byId('dt_atual').value = new Date();
                        dijit.byId("cbRecStatusTransferenciaCad").set("disabled", true);
                        dijit.byId("rafCad").set("disabled", true);
                        dijit.byId("ckRecEmailEnviadoCadOrigem").set("disabled", true);
                        dijit.byId("ckRecEmailEnviadoCadDestino").set("disabled", true);


                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarReceberTransferencia', 'divIncluirReceberTransferencia', null, 'apresentadorMensagemReceberTransferencia', 'divCancelarReceberTransferencia', 'divLimparReceberTransferencia');

                        dijit.byId("cadReceberTransferencia").show();

                        //limparGrid();
                        showCarregando();
                    }



                } catch (e) {
                    showCarregando();
                    postGerarLog(e);

                }
            },
                function (error) {
                    showCarregando();

                });
        });


}


function montarStatusTransferencia(nomElement, value) {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
        function (Memory, filteringSelect) {
            try {
                var statusStore = null;

                statusStore = new Memory({
                    data: [
                        { name: "Aprovada", id: "2" },
                        { name: "Efetuada", id: "3" },
                        { name: "Recusada", id: "9" },
                        { name: "Solicitada", id: "1" },



                    ]
                });
                var status = new filteringSelect({
                    id: nomElement,
                    name: "Status",
                    value: value,
                    store: statusStore,
                    searchAttr: "name",
                    style: "width: 90px;"
                }, nomElement);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
};

function montarMotivoCad(nomElement) {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
        function (Memory, filteringSelect) {
            try {
                var statusStore = null;

                statusStore = new Memory({
                    data: [
                        { name: "Aprovada", id: "2" },
                        { name: "Efetuada", id: "3" },
                        { name: "Recusada", id: "9" },
                        { name: "Solicitada", id: "1" },
                    ]
                });
                var status = new filteringSelect({
                    id: nomElement,
                    name: "Status",
                    value: "0",
                    store: statusStore,
                    searchAttr: "name",
                    style: "width: 90px;"
                }, nomElement);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
};

function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = ReceberTransferencia;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        limparPesquisaAlunoFK();
        pesquisarAlunoFK(true);
        dijit.byId("proAluno").show();
        dijit.byId('gridPesquisaAluno').update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFKCad() {
    try {
        debugger
        dojo.byId('tipoRetornoAlunoFK').value = ReceberTransferenciaCAD;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        limparPesquisaAlunoFK();
        pesquisarAlunoFK(true);
        dijit.byId("proAluno").show();
        dijit.byId('gridPesquisaAluno').update();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function retornarAlunoEnviarTranferencia() {
    try {
        var valido = true;
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);

            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("cdAlunoPes").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAlunoPes').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function retornarAlunoEnviarTranferenciaCad() {
    try {
        var valido = true;
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);

            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            showCarregando();

            require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
                function (dom, xhr, ref) {
                    dojo.xhr.get({
                        url: Endereco() + "/api/Secretaria/getRafByAluno?cdAluno=" + gridPesquisaAluno.itensSelecionados[0].cd_aluno,
                        preventCache: true,
                        handleAs: "json",
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json",
                            "Authorization": Token()
                        }
                    }).then(function (retorno) {
                        try {

                            if (hasValue(retorno)) {
                                dojo.byId("cdAlunoPesCad").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
                                dojo.byId("noAlunoCad").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
                                dijit.byId('limparAlunoPesCad').set("disabled", false);
                                dojo.byId("rafCad").value = retorno;
                            } else {
                                dojo.byId("cdAlunoPesCad").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
                                dojo.byId("noAlunoCad").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
                                dijit.byId('limparAlunoPesCad').set("disabled", false);
                                dojo.byId("rafCad").value = "";
                            }

                            showCarregando();




                        } catch (e) {
                            showCarregando();
                            postGerarLog(e);

                        }
                    },
                        function (error) {
                            showCarregando();

                        });
                });


        }
        if (!valido)
            return false;
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//function riqueridDatas(bool) {
//    dijit.byId("dtInicialComp").set("required", bool);
//    dijit.byId("dtFinalComp").set("required", bool);
//}




function abrirPessoaFK(isPessoaEscola) {
    try {

        if (isPessoaEscola) {
            dijit.byId("proPessoa").set("title", "Pesquisar Escolas");
            dijit.byId('tipoPessoaFK').set('value', 2);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Fantasia:";
            dijit.byId('tipoPessoaFK').set('disabled', true);
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Fantasia";
            dijit.byId("gridPesquisaPessoa").getCell(2).width = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(2).unitWidth = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(3).width = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(3).unitWidth = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(1).width = "25%";
            dijit.byId("gridPesquisaPessoa").getCell(1).unitWidth = "25%";
            dojo.byId("idOrigemPessoaFK").value = CAD_ESCOLA_USUARIO;
            limparPesquisaEscolaFK();
            pesquisarEscolasFK();
            dijit.byId("proPessoa").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        }
        else {
            limparPesquisaPessoaFK();
            dijit.byId("tipoPessoaFK").set("value", 1);
            dijit.byId("tipoPessoaFK").set("disabled", true);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Nome Reduzido:";
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Nome Reduzido";
            dojo.byId("idOrigemPessoaFK").value = "";
            dijit.byId("proPessoa").set("title", "Pesquisar Pessoa");

            pesquisaPessoaFK(true, 0);
            dijit.byId("proPessoa").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPessoaFKCad(isPessoaEscola) {
    try {

        if (isPessoaEscola) {
            dijit.byId("proPessoa").set("title", "Pesquisar Escolas");
            dijit.byId('tipoPessoaFK').set('value', 2);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Fantasia:";
            dijit.byId('tipoPessoaFK').set('disabled', true);
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Fantasia";
            dijit.byId("gridPesquisaPessoa").getCell(2).width = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(2).unitWidth = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(3).width = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(3).unitWidth = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(1).width = "25%";
            dijit.byId("gridPesquisaPessoa").getCell(1).unitWidth = "25%";
            dojo.byId("idOrigemPessoaFK").value = ReceberTransferenciaCAD;
            limparPesquisaEscolaFK();
            pesquisarEscolasFK();
            dijit.byId("proPessoa").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        }
        else {
            limparPesquisaPessoaFK();
            dijit.byId("tipoPessoaFK").set("value", 1);
            dijit.byId("tipoPessoaFK").set("disabled", true);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Nome Reduzido:";
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Nome Reduzido";
            dojo.byId("idOrigemPessoaFK").value = "";
            dijit.byId("proPessoa").set("title", "Pesquisar Pessoa");

            pesquisaPessoaFK(true, 0);
            dijit.byId("proPessoa").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarEscolasFK() {

    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready"],
        function (JsonRest, ObjectStore, Cache, Memory, ready) {
            ready(function () {
                try {

                    var grid = dijit.byId("gridPesquisaPessoa");
                    //var listaEmpresasGrid = getEmpresasGridUsuario();

                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/empresa/findAllEmpresaTransferencia?&cd_usuario=" /*+ dojo.byId("cod_usuario").value*/
                                + "&nome=" + dojo.byId("_nomePessoaFK").value + "&fantasia=" + dojo.byId("_apelido").value + "&cnpj=" + dojo.byId("CnpjCpf").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token(), "Range": "items=0-24" },

                        }), Memory({}));

                    dataStore = new ObjectStore({ objectStore: myStore });
                    grid.setStore(dataStore);
                    grid.layout.setColumnVisibility(5, false);


                }
                catch (e) {
                    postGerarLog(e);
                }
            })
        });
}


function retornarPessoaEscola() {
    try {
        showCarregando();
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0 || gridPessoaSelec.itensSelecionados.length > 1) {
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("no_unidade").value = gridPessoaSelec.itensSelecionados[0].dc_reduzido_pessoa;
            dojo.byId("cdUnidadePesUnidade").value = gridPessoaSelec.itensSelecionados[0].cd_pessoa;
            dijit.byId('limparUnidadePes').set("disabled", false);
        }

        if (!valido) {
            showCarregando();
            return false;

        }


        dijit.byId("proPessoa").hide();
        showCarregando();
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}


function retornarPessoaEscolaCad() {
    try {
        showCarregando();
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPessoaSelec.itensSelecionados) ||
            gridPessoaSelec.itensSelecionados.length <= 0 ||
            gridPessoaSelec.itensSelecionados.length > 1) {
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        } else {

            require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
                function (dom, xhr, ref) {
                    dojo.xhr.get({
                        url: Endereco() + "/api/Secretaria/getEmailUnidade?cdEscola=" + gridPessoaSelec.itensSelecionados[0].cd_pessoa,
                        preventCache: true,
                        handleAs: "json",
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json",
                            "Authorization": Token()
                        }
                    }).then(function (retorno) {
                        try {
                            if ((gridPessoaSelec.itensSelecionados[0].cd_pessoa + "") === dojo.byId("_ES0").value) {
                                dojo.byId("no_unidade_cad_origem").value = "";
                                dojo.byId("cdUnidadePesUnidadeCadOrigem").value = 0;
                                dijit.byId("limparUnidadePesCadOrigem").set('disabled', true);
                                dojo.byId("emailRecCadDestino").value = "";
                                caixaDialogo(DIALOGO_AVISO, "A unidade de destino não pode ser igual a de origem", null);
                            } else
                                if (hasValue(retorno)) {
                                    console.log(retorno);
                                    dojo.byId("no_unidade_cad_origem").value =
                                        gridPessoaSelec.itensSelecionados[0].dc_reduzido_pessoa;
                                    dojo.byId("cdUnidadePesUnidadeCadOrigem").value =
                                        gridPessoaSelec.itensSelecionados[0].cd_pessoa;
                                    dijit.byId('limparUnidadePesCadOrigem').set("disabled", false);
                                    dojo.byId("emailRecCadDestino").value = retorno;
                                } else {
                                    dojo.byId("no_unidade_cad_origem").value =
                                        gridPessoaSelec.itensSelecionados[0].dc_reduzido_pessoa;
                                    dojo.byId("cdUnidadePesUnidadeCadOrigem").value =
                                        gridPessoaSelec.itensSelecionados[0].cd_pessoa;
                                    dijit.byId('limparUnidadePesCadOrigem').set("disabled", false);
                                    dojo.byId("emailRecCadDestino").value = "";
                                }

                            showCarregando();




                        } catch (e) {
                            showCarregando();
                            postGerarLog(e);

                        }
                    },
                        function (error) {
                            showCarregando();

                        });
                });






        }
        if (!valido) {
            showCarregando();
            return false;

        }

        dijit.byId("proPessoa").hide();

    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}


function limparPesquisaEscolaFK() {
    try {
        dojo.byId("_nomePessoaFK").value = "";
        dojo.byId("_apelido").value = "";
        dojo.byId("CnpjCpf").value = "";
        if (hasValue(dijit.byId("gridPesquisaPessoa"))) {
            dijit.byId("gridPesquisaPessoa").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaPessoa").itensSelecionados))
                dijit.byId("gridPesquisaPessoa").itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparFormReceberTransferencias() {
    try {
        showCarregando();
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
            function (dom, xhr, ref) {
                dojo.xhr.get({
                    url: Endereco() + "/api/Secretaria/getComponentesReceberTransferenciaCad",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (retorno) {
                    try {
                        if (hasValue(retorno)) {

                            dojo.byId('dt_cadastro_transferencia_cad').value = dojo.date.locale.format(new Date(),
                                { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });

                            criarOuCarregarCompFiltering("cbMotivoTransferenciaCad", retorno.MotivosTransferencia, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_motivo_transferencia_aluno', 'dc_motivo_transferencia_aluno');
                            dojo.byId("emailRecCadOrigem").value = retorno.emailOrigem;
                            dojo.byId("cd_transferencia_aluno").value = 0;
                            dijit.byId("cbRecStatusTransferenciaCad").set("value", 0);
                            dojo.byId("no_unidade_cad_origem").value = '';
                            dojo.byId("cdUnidadeCadOrigem").value = 0;
                            dojo.byId("emailRecCadDestino").value = '';
                            dojo.byId("noAlunoCad").value = '';
                            dojo.byId("cdAlunoPesCad").value = 0;

                            dojo.byId("cdAlunoDest").value = 0;


                            dojo.byId("rafCad").value = '';
                            dijit.byId("ckRecEmailEnviadoCadOrigem").set("checked", false);
                            dijit.byId("ckRecEmailEnviadoCadDestino").set("checked", false);

                            dojo.byId("edlayoutHistoricoAlunoCad").value = '';


                            dijit.byId("uploaderHistoricoAluno").set("disabled", true);
                            dijit.byId("btnDownHistoricoAlunoCad").set("disabled", true);
                            dijit.byId("btnShowHistoricoAlunoCad").set("disabled", true);


                            dojo.byId("dt_solicitacao_transferencia_cad").value = '';
                            dojo.byId("dt_confirmacao_transferencia_cad").value = '';
                            dojo.byId("dt_transferencia_cad").value = '';

                            dijit.byId("cbRecStatusTransferenciaCad").set("disabled", true);
                            dijit.byId("rafCad").set("disabled", true);
                            dijit.byId("ckRecEmailEnviadoCadOrigem").set("disabled", true);
                            dijit.byId("ckRecEmailEnviadoCadDestino").set("disabled", true);
                            hideCarregando();
                        }
                    }
                    catch (e) {
                        hideCarregando();
                        postGerarLog(e);
                    }

                })
            })



    }
    catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function limparGridReceberTransferencias() {
    try {
        var gridAluno = dijit.byId('gridAluno');
        if (hasValue(gridAluno)) {
            gridAluno.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridAluno.update();
        }

        //var gridReceberTransferencia = dijit.byId('gridReceberTransferencia');
        //if (hasValue(gridReceberTransferencia)) {
        //    gridReceberTransferencia.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        //    gridReceberTransferencia.update();
        //}
    }
    catch (e) {
        postGerarLog(e);
    }
}



function formatterDateHours(data) {

    var data_formatada = dojo.date.locale.format(new Date(data),
        { selector: "date", datePattern: "dd/MM/yyyy HH:mm", formatLength: "short", locale: "pt-br" });
    return data_formatada;
}

function formatterDate(data) {

    var data_formatada = dojo.date.locale.format(new Date(data),
        { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
    return data_formatada;
}


function IncluirReceberTransferencia() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemReceberTransferencia', null);
        var itemCad = new ObjItemCad();


        if (hasValue(itemCad)) {

            require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"],
                function (xhr, ref, windows) {
                    if (!validarCamposReceberTransferencias(windows))
                        return false;

                    showCarregando();

                    xhr.post(Endereco() + "/api/Secretaria/postInsertReceberTransferenciaAluno",
                        {
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            handleAs: "json",
                            data: ref.toJson(itemCad)
                        }).then(function (data) {
                            try {
                                data = jQuery.parseJSON(data);
                                if (!hasValue(data.erro)) {
                                    var itemAlterado = data.retorno;
                                    var gridReceberTransferencia = 'gridReceberTransferencia';
                                    var grid = dijit.byId(gridReceberTransferencia);
                                    apresentaMensagem('apresentadorMensagem', data);
                                    dijit.byId("cadReceberTransferencia").hide();
                                    if (!hasValue(grid.itensSelecionados)) {
                                        grid.itensSelecionados = [];
                                    }
                                    insertObjSort(grid.itensSelecionados,
                                        "cd_transferencia_aluno",
                                        itemAlterado);
                                    buscarItensSelecionados(gridReceberTransferencia,
                                        'selecionadoReceberTransferencia',
                                        'cd_transferencia_aluno',
                                        'selecionaTodosReceberTransferencia',
                                        ['pesquisarReceberTransferencia', 'relatorioReceberTransferencia'],
                                        'todosItensReceberTransferencia');
                                    grid.sortInfo =
                                        2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                    setGridPagination(grid, itemAlterado, "cd_transferencia_aluno");
                                    hideCarregando();

                                } else {
                                    apresentaMensagem('apresentadorMensagemReceberTransferencia', data);
                                    hideCarregando();
                                }
                            } catch (e) {
                                hideCarregando();
                                postGerarLog(e);
                            }
                        },
                            function (error) {
                                apresentaMensagem('apresentadorMensagemReceberTransferencia', error);
                                hideCarregando();
                            });
                });



        } else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "");
            apresentaMensagem("apresentadorMensagemReceberTransferencia", mensagensWeb);
        }
    } catch (e) {
        postGerarLog(e);
    }

}

function validarCamposReceberTransferencias(windowUtils) {
    try {
        var validado = true;
        apresentaMensagem("apresentadorMensagemReceberTransferencia", null);


        if (!hasValue(dojo.byId("no_unidade_cad_origem").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Unidade de destino.");
            apresentaMensagem("apresentadorMensagemReceberTransferencia", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("emailRecCadOrigem").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Email de origem.");
            apresentaMensagem("apresentadorMensagemReceberTransferencia", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("emailRecCadDestino").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Email de destino.");
            apresentaMensagem("apresentadorMensagemReceberTransferencia", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("noAlunoCad").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Aluno.");
            apresentaMensagem("apresentadorMensagemReceberTransferencia", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("noAlunoCad").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Aluno.");
            apresentaMensagem("apresentadorMensagemReceberTransferencia", mensagensWeb);
            return false;
        }

        if (dijit.byId("cbMotivoTransferenciaCad").value == null || dijit.byId("cbMotivoTransferenciaCad").value == undefined || dijit.byId("cbMotivoTransferenciaCad").value <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Motivo.");
            apresentaMensagem("apresentadorMensagemReceberTransferencia", mensagensWeb);
            return false;
        }

        if (!dijit.byId("formReceberTransferencias").validate()) {
            return false;
        }

        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function AlterarReceberTransferencia() {
    try {
        if ((dijit.byId("cbRecStatusTransferenciaCad").value == EnumsTipoStatusTransferenciaAluno.RECUSADA &&
                dijit.byId("ckRecEmailEnviadoCadDestino").checked == true) ||
            (dijit.byId("cbRecStatusTransferenciaCad").value == EnumsTipoStatusTransferenciaAluno.APROVADA &&
                dijit.byId("ckRecEmailEnviadoCadDestino").checked == true)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não é possível alterar o registro após o envio do email já ter sido realizado.");
            apresentaMensagem("apresentadorMensagemReceberTransferencia", mensagensWeb);
            return false;
        }

        if (dijit.byId("cbRecStatusTransferenciaCad").value == EnumsTipoStatusTransferenciaAluno.SOLICITADA) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Transferência com status solicitada deve ser alterada para aprovada ou recusada para que o registro seja salvo.");
            apresentaMensagem("apresentadorMensagemReceberTransferencia", mensagensWeb);
            return false;
        }

        if (dijit.byId("cbRecStatusTransferenciaCad").value == EnumsTipoStatusTransferenciaAluno.EFETUADA) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O status da Transferência só pode ser alterado para aprovada ou recusada.");
            apresentaMensagem("apresentadorMensagemReceberTransferencia", mensagensWeb);
            return false;
        }

        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemReceberTransferencia', null);

        itemEdit = new ObjItemEdit();
        
        
        if (itemEdit != null) {
            require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/window"],
                function (dom, xhr, ref, windows) {

                    if (!validarCamposReceberTransferencias(windows))
                        return false;

                    if (itemEdit.id_status_transferencia == EnumsTipoStatusTransferenciaAluno.EFETUADA) {
                        caixaDialogo(DIALOGO_AVISO, "Transferência efetuada não podem ser editada.", null);
                        return false;
                    }

                    showCarregando();

                    xhr.post(Endereco() + "/api/Secretaria/postEditReceberTransferenciaAluno",
                        {
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            handleAs: "json",
                            data: ref.toJson(itemEdit)
                        }).then(function (data) {
                            try {
                                data = jQuery.parseJSON(data);
                                if (!hasValue(data.erro)) {
                                    var itemAlterado = data.retorno;
                                    var gridName = 'gridReceberTransferencia';
                                    var grid = dijit.byId(gridName);
                                    apresentaMensagem('apresentadorMensagem', data);
                                    dijit.byId("cadReceberTransferencia").hide();
                                    if (!hasValue(grid.itensSelecionados)) {
                                        grid.itensSelecionados = [];
                                    }
                                    removeObjSort(grid.itensSelecionados, "cd_transferencia_aluno", dom.byId("cd_transferencia_aluno").value);
                                    insertObjSort(grid.itensSelecionados, "cd_transferencia_aluno", itemAlterado);
                                    buscarItensSelecionados(gridName,
                                        'selecionadoReceberTransferencia',
                                        'cd_transferencia_aluno',
                                        'selecionaTodosReceberTransferencia',
                                        ['pesquisarReceberTransferencia', 'relatorioReceberTransferencia'],
                                        'todosItensReceberTransferencia');
                                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                    setGridPagination(grid, itemAlterado, "cd_transferencia_aluno");
                                    showCarregando();
                                } else {
                                    apresentaMensagem('apresentadorMensagemReceberTransferencia', data);
                                    showCarregando();
                                }
                            } catch (er) {
                                postGerarLog(er);
                            }
                        },
                            function (error) {
                                showCarregando();
                                apresentaMensagem('apresentadorMensagemReceberTransferencia', error.response.data);
                            });
                });
        }


    }
    catch (e) {
        postGerarLog(e);
    }
}

function ObjItemEdit() {
    try {

        var item = {
            cd_transferencia_aluno: dojo.byId("cd_transferencia_aluno").value > 0 ? dojo.byId("cd_transferencia_aluno").value : 0,
            dc_email_origem: dojo.byId("emailRecCadOrigem").value,
            dc_email_destino: dojo.byId("emailRecCadDestino").value,
            id_status_transferencia: dijit.byId("cbRecStatusTransferenciaCad").value,
        }

        return item;
    }
    catch (e) {
        postGerarLog(e);
    }
}






function eventoEditarReceberTransferencia(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {


            var gridReceberTransferencia = dijit.byId("gridReceberTransferencia");


            dojo.byId("cd_transferencia_aluno").value = itensSelecionados[0].cd_transferencia_aluno;
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(null, itensSelecionados[0], gridReceberTransferencia, false);


        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEnviarEmail(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {



            require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"],
                function (xhr, ref, windows) {


                    if (itensSelecionados[0].id_email_destino == true) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Email já enviado.");
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return false;
                    }

                    if (itensSelecionados[0].id_status_transferencia == EnumsTipoStatusTransferenciaAluno.SOLICITADA) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não é permitido enviar email para transferência com status solicitada.");
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return false;
                    }


                    if (itensSelecionados[0].id_status_transferencia == EnumsTipoStatusTransferenciaAluno.EFETUADA) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não é permitido enviar email de solicitação para transferência com status efetuada.");
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return false;
                    }


                    showCarregando();

                    xhr.post(Endereco() + "/api/Secretaria/PostSendEmailAprovarRecusarTransferenciaAluno",
                        {
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            handleAs: "json",
                            data: ref.toJson({ cd_transferencia_aluno: itensSelecionados[0].cd_transferencia_aluno, id_status_transferencia: itensSelecionados[0].id_status_transferencia })
                        }).then(function (data) {
                            try {
                                data = jQuery.parseJSON(data);
                                if (data != null) {
                                    if (hasValue(data.retorno)) {
                                        var mensagensWeb = new Array();
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, data.retorno[0]);
                                        apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                    } else {
                                        var mensagensWeb = new Array();
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, data.MensagensWeb[0].mensagem);
                                        apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                    }

                                } else {
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não foi possível enviar o email. Tente novamente.");
                                    apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                }
                                PesquisarReceberTransferencia(true);

                                hideCarregando();
                            } catch (e) {
                                hideCarregando();
                                postGerarLog(e);
                            }
                        },
                            function (error) {
                                apresentaMensagem('apresentadorMensagemReceberTransferencia', error);
                                hideCarregando();
                            });
                });





        }
    }
    catch (e) {
        postGerarLog(e);
    }
}






function keepValues(Form, value, grid, ehLink) {
    try {
        getLimpar('#formReceberTransferencias');
        clearForm('formReceberTransferencias');
        limparGridReceberTransferencias();
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');


        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        if (hasValue(value)) {
            require(["dojo/_base/xhr"],
                function (xhr) {
                    xhr.get({
                        url: Endereco() + "/api/Secretaria/getReceberTransferenciaAlunoForEdit?cd_transferencia_aluno=" + value.cd_transferencia_aluno,
                        preventCache: true,
                        handleAs: "json",
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json",
                            "Authorization": Token()
                        }
                    }).then(function (retorno) {
                        try {
                            var dados = retorno;
                            if (dados != null) {
                                apresentaMensagem("apresentadorMensagem", null);

                                dojo.byId("cd_transferencia_aluno").value = dados.cd_transferencia_aluno;
                                dijit.byId("cbRecStatusTransferenciaCad").set("value", dados.id_status_transferencia);
                                dojo.byId("no_unidade_cad_origem").value = dados.no_unidade_origem;
                                dojo.byId("cdUnidadeCadOrigem").value = dados.cd_escola_origem;
                                dojo.byId("emailRecCadOrigem").value = dados.dc_email_origem;
                                dojo.byId("emailRecCadDestino").value = dados.dc_email_destino;
                                dojo.byId("noAlunoCad").value = dados.no_aluno;
                                dojo.byId("cdAlunoPesCad").value = dados.cd_aluno_origem;
                                if (dados.cd_aluno_destino != null &&
                                    dados.cd_aluno != undefined &&
                                    dados.cd_aluno_destino > 0) {
                                    dojo.byId("cdAlunoDest").value = dados.cd_aluno_destino;
                                }

                                dojo.byId("rafCad").value = dados.nm_raf;
                                dijit.byId("ckRecEmailEnviadoCadOrigem").set("checked", dados.id_email_origem);
                                dijit.byId("ckRecEmailEnviadoCadDestino").set("checked", dados.id_email_destino);
                                if (hasValue(dados.no_arquivo_historico)) {
                                    dojo.byId("edlayoutHistoricoAlunoCad").value = dados.no_arquivo_historico;
                                }
                                
                                dijit.byId("uploaderHistoricoAluno").set("disabled", 'true');
                                

                                if (hasValue(dados.no_arquivo_historico) &&
                                    (dados.id_status_transferencia == EnumsTipoStatusTransferenciaAluno.APROVADA ||
                                        dados.id_status_transferencia == EnumsTipoStatusTransferenciaAluno.EFETUADA)) {
                                    dijit.byId("btnDownHistoricoAlunoCad").set("disabled", false);
                                    dijit.byId("btnShowHistoricoAlunoCad").set("disabled", false);
                                } else {
                                    dijit.byId("btnDownHistoricoAlunoCad").set("disabled", true);
                                    dijit.byId("btnShowHistoricoAlunoCad").set("disabled", true);

                                }

                                dijit.byId("cbMotivoTransferenciaCad").set("value", dados.cd_motivo_transferencia);
                                dojo.byId("dt_cadastro_transferencia_cad").value = dados.dta_cadastro_transferencia;
                                dojo.byId("dt_solicitacao_transferencia_cad").value = dados.dta_solicitacao_transferencia;
                                dojo.byId("dt_confirmacao_transferencia_cad").value = dados.dta_confirmacao_transferencia;
                                dojo.byId("dt_transferencia_cad").value = dados.dta_transferencia;

                                dijit.byId("pesUnidadeCadOrigem").set("disabled", true);
                                dijit.byId("pesAlunoCad").set("disabled", true);

                                if (dados.id_status_transferencia == EnumsTipoStatusTransferenciaAluno.EFETUADA ||
                                    (dados.id_status_transferencia == EnumsTipoStatusTransferenciaAluno.RECUSADA && dados.id_email_destino == true) ||
                                    (dados.id_status_transferencia == EnumsTipoStatusTransferenciaAluno.APROVADA && dados.id_email_destino == true)) {
                                    dijit.byId("cbRecStatusTransferenciaCad").set("disabled", true);
                                } else {
                                    dijit.byId("cbRecStatusTransferenciaCad").set("disabled", false);
                                }
                                
                                dijit.byId("rafCad").set("disabled", true);
                                dijit.byId("ckRecEmailEnviadoCadOrigem").set("disabled", true);
                                dijit.byId("ckRecEmailEnviadoCadDestino").set("disabled", true);
                                dijit.byId("cbMotivoTransferenciaCad").set("disabled", true);

                                dijit.byId("cadReceberTransferencia").show();
                                IncluirAlterar(0,
                                    'divAlterarReceberTransferencia',
                                    'divIncluirReceberTransferencia',
                                    null,
                                    'apresentadorMensagemReceberTransferencia',
                                    'divCancelarReceberTransferencia',
                                    'divLimparReceberTransferencia');
                                hideCarregando();
                            }
                        } catch (er) {
                            hideCarregando();
                            postGerarLog(er);
                        }
                    },
                        function (error) {
                            hideCarregando();

                        });
                });
        }
    }
    catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}


function PesquisarReceberTransferencia(limparItens) {

    var params = montarParametrosUrlReceberTransferencia();
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/Secretaria/getReceberTransferenciaAlunoSearch?cd_unidade_origem=" + (params.cd_unidade || "") + "&no_aluno=" + (params.no_aluno || "") + "&nm_raf=" + (params.nm_raf || "") + "&cpf=" + (params.cpf || "") + "&status_transferencia=" + (params.statusTransferencia || "") + "&dataIni=" + (params.dataIni || "") + "&dataFim=" + (params.dataFim || ""),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_transferencia_aluno"
                }
                ), Memory({ idProperty: "cd_transferencia_aluno" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridReceberTransferencia = dijit.byId("gridReceberTransferencia");
            if (limparItens) {
                gridReceberTransferencia.itensSelecionados = [];
            }
            gridReceberTransferencia.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarParametrosUrlReceberTransferencia() {
    this.cd_unidade = hasValue(dojo.byId("cdUnidadePesUnidade").value) ? dojo.byId("cdUnidadePesUnidade").value : null;
    this.no_aluno = hasValue(dojo.byId("no_aluno").value) ? dojo.byId("no_aluno").value : null;
    this.nm_raf = hasValue(dojo.byId("raf").value) ? dojo.byId("raf").value : null
    this.cpf = hasValue(dojo.byId("cpf").value) ? dojo.byId("cpf").value : null
    this.statusTransferencia = hasValue(dijit.byId("cbStatusTransferencia").value) ? dijit.byId("cbStatusTransferencia").value : null;


    if (hasValue(dojo.byId("dtInicial").value)) {
        this.dataIni = dojo.date.locale.parse(dojo.byId("dtInicial").value,
            { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.dataIni = dojo.date.locale.format(this.dataIni,
            { selector: "date", datePattern: "yyyy-MM-dd", formatLength: "short", locale: "pt-br" });
    } else {
        this.dataIni = null;
    }

    if (hasValue(dojo.byId("dtFinal").value)) {
        this.dataFim = dojo.date.locale.parse(dojo.byId("dtFinal").value,
            { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.dataFim = dojo.date.locale.format(this.dataFim,
            { selector: "date", datePattern: "yyyy-MM-dd", formatLength: "short", locale: "pt-br" });
    } else {
        this.dataFim = null;
    }

    var ReceberTransferencias = function (cd_unidade, no_aluno, nm_raf, cpf, statusTransferencia, dataIni, dataFim) {
        this.cd_unidade = cd_unidade;
        this.no_aluno = no_aluno;
        this.nm_raf = nm_raf;
        this.cpf = cpf
        this.statusTransferencia = statusTransferencia;
        this.dataIni = dataIni;
        this.dataFim = dataFim;
    }


    objReceberTransferencias = new ReceberTransferencias(cd_unidade, no_aluno, nm_raf, cpf, statusTransferencia, dataIni, dataFim);
    return objReceberTransferencias;
}


