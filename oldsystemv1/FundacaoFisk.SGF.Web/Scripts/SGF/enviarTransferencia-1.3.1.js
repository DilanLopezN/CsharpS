var PESQUISAALUNOFILTRO = 3;

var EnumsTipoStatusTransferenciaAluno = {
    CADASTRADA: 0,
    SOLICITADA: 1, 
    APROVADA: 2,
    EFETUADA: 3,
    RECUSADA: 9
}

function formatCheckBoxEnviarTransferencia() {

}

function formatCheckBoxEnviarTransferencia(value, rowIndex, obj) {
    try {
        var gridName = 'gridEnviarTransferencia';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEnviarTransferencia');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_transferencia_aluno", grid._by_idx[rowIndex].item.cd_transferencia_aluno);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  Id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_transferencia_aluno', 'selecionadoEnviarTransferencia', -1, 'selecionaTodosEnviarTransferencia', 'selecionaTodosEnviarTransferencia', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_transferencia_aluno', 'selecionadoEnviarTransferencia', " + rowIndex + ", '" + id + "', 'selecionaTodosEnviarTransferencia', '" + gridName + "')", 2);

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

function montarCadastroEnviarTransferencia() {
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
                    montarStatusTransferencia("cbStatusTransferenciaCad", "0");
                    criarOuCarregarCompFiltering("cbMotivoTransferenciaCad", [], "", null, ready, Memory, FilteringSelect, 'cd_motivo_transferencia_aluno', 'dc_motivo_transferencia_aluno');
                    /*Fim Loaders*/

                    /**
                     * Ações Relacionadas
                     */
                    var menu = new DropDownMenu({ style: "height: 25px" });
                    var acaoEditar = new MenuItem({
                        label: "Editar",
                        onClick: function () {

                            limparFormEnviarTransferencias();
                            showCarregando();
                            eventoEditarEnviarTransferencia(dijit.byId("gridEnviarTransferencia").itensSelecionados, xhr);

                            //limparGrid();
                            hideCarregando();



                        }
                    });
                    menu.addChild(acaoEditar);

                    var acaoRemover = new MenuItem({
                        label: "Excluir",
                        onClick: function () {
                            eventoRemover(dijit.byId("gridEnviarTransferencia").itensSelecionados,
                                'DeletarEnviarTransferencia(itensSelecionados)');
                        }
                    });
                    menu.addChild(acaoRemover);
                    var acaoEnviarEmail = new MenuItem({
                        label: "Enviar Email",
                        onClick: function () {
                            eventoEnviarEmail(dijit.byId("gridEnviarTransferencia").itensSelecionados, xhr);
                        }
                    });
                    menu.addChild(acaoEnviarEmail);

                    var acaoEnviarEmail = new MenuItem({
                        label: "Transferir",
                        onClick: function () {
                            eventoTransferirAluno(dijit.byId("gridEnviarTransferencia").itensSelecionados, xhr);
                        }
                    });
                    menu.addChild(acaoEnviarEmail);

                    var button = new DropDownButton({
                        label: "Ações Relacionadas",
                        name: "acoesRelacionadasEnviarTransferencia",
                        dropDown: menu,
                        id: "acoesRelacionadasEnviarTransferencia"
                    });
                    dom.byId("linkAcoesEnviarTransferencia").appendChild(button.domNode);

                    // Adiciona link de selecionados:
                    menu = new DropDownMenu({ style: "height: 25px" });
                    var menuTodosItens = new MenuItem({
                        label: "Todos Itens",
                        onClick: function () {
                            buscarTodosItens(gridEnviarTransferencia,
                                'todosEnviarTransferencia',
                                ['pesquisarEnviarTransferencia', 'relatorioEnviarTransferencia']);
                            PesquisarEnviarTransferencia(false);
                        }
                    });
                    menu.addChild(menuTodosItens);

                    var menuItensSelecionados = new MenuItem({
                        label: "Itens Selecionados",
                        onClick: function () {
                            buscarItensSelecionados('gridEnviarTransferencia',
                                'selecionadoEnviarTransferencia',
                                'cd_transferencia_aluno',
                                'selecionaTodosEnviarTransferencia',
                                ['pesquisarEnviarTransferencia', 'relatorioEnviarTransferencia'],
                                'todosItensEnviarTransferencia');
                        }
                    });
                    menu.addChild(menuItensSelecionados);

                    var button = new DropDownButton({
                        label: "Todos Itens",
                        name: "todosItensEnviarTransferencia",
                        dropDown: menu,
                        id: "todosItensEnviarTransferencia"
                    });
                    dom.byId("linkSelecionadosEnviarTransferencia").appendChild(button.domNode);
                    /*-------------Fim Ações relacionadas -----------*/

                    


                    /*
                     * Botões
                     */

                    new Button({
                        label: "Salvar",
                        iconClass: 'dijitEditorIcon dijitEditorIconSave',
                        onClick: function () {
                            IncluirEnviarTransferencia();
                        }
                    },
                        "incluirEnviarTransferencia");
                    new Button({
                        label: "Salvar",
                        iconClass: 'dijitEditorIcon dijitEditorIconSave',
                        onClick: function () {
                            AlterarEnviarTransferencia();
                        }
                    },
                        "alterarEnviarTransferencia");
                    new Button({
                        label: "Excluir",
                        iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                        onClick: function () {

                            caixaDialogo(DIALOGO_CONFIRMAR,
                                '',
                                function executaRetorno() {
                                    DeletarEnviarTransferencia();
                                });
                        }
                    },
                        "deleteEnviarTransferencia");

                    new Button({
                        label: "Limpar",
                        iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                        onClick: function () {
                            limparFormEnviarTransferencias();
                            //dojo.byId("atendente").value = document.getElementById('nomeUsuario').innerText;
                        }
                    },
                        "limparEnviarTransferencia");

                    new Button({
                        label: "Cancelar",
                        iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                        onClick: function () {
                            showCarregando();
                            keepValues(null, null, dijit.byId("gridEnviarTransferencia"), null);
                            showCarregando();
                        }
                    },
                        "cancelarEnviarTransferencia");
                    new Button({
                        label: "Fechar",
                        iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                        onClick: function () {
                            dijit.byId("cadEnviarTransferencia").hide();
                        }
                    },
                        "fecharEnviarTransferencia");



                    new Button({
                        label: "",
                        iconClass: 'dijitEditorIconSearchSGF',
                        onClick: function () {
                            apresentaMensagem('apresentadorMensagem', null);
                            PesquisarEnviarTransferencia(true);
                        }
                    }, "pesquisarEnviarTransferencia");

                    new Button({
                        label: "Novo",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                        onClick: function () {
                            try {


                                getComponentesEnviarTransferencia(ready, Memory, FilteringSelect);

                            } catch (e) {
                                postGerarLog(e);
                                showCarregando();
                            }
                        }
                    },
                        "novoEnviarTransferencia");

                    new Button({
                        label: getNomeLabelRelatorio(),
                        iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                        onClick: function () {
                            var enviarTransferencia = montarParametrosUrlEnviarTransferencia();
                            require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"],
                                function (dom, domAttr, xhr, ref) {
                                    xhr.get({
                                        url: Endereco() +
                                            "/api/Secretaria/GetUrlRelatorioEnviarTransferencia?" +
                                            getStrGridParameters('gridEnviarTransferencia') +
                                            "&cd_unidade_destino=" + (enviarTransferencia.cd_turma || 0) + "&cd_aluno=" + (enviarTransferencia.cd_aluno || 0) + "&nm_raf=" + (enviarTransferencia.nm_raf || '') + "&cpf=" + (enviarTransferencia.cpf || '') + "&status_transferencia=" + (enviarTransferencia.statusTransferencia || '0') + "&dataIni=" + (enviarTransferencia.dataIni || "") + "&dataFim=" + (enviarTransferencia.dataFim || ""),
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
                        "relatorioEnviarTransferencia");

                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                            try {
                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                    montarGridPesquisaAluno(false, function () {
                                        abrirAlunoFK(ENVIARTRANSFERENCIA);
                                    });
                                }
                                else
                                    abrirAlunoFK(ENVIARTRANSFERENCIA);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }
                    }, "pesAluno");

                    var pesAluno = document.getElementById('pesAluno');
                    if (hasValue(pesAluno)) {
                        pesAluno.parentNode.style.minWidth = '18px';
                        pesAluno.parentNode.style.width = '18px';
                    }

                    //criação do botões pesquisa principal
                    new Button({
                        label: "Limpar", iconClass: '', type: "reset", disabled: true,
                        onClick: function () {
                            dojo.byId("cdAlunoPes").value = 0;
                            dojo.byId("noAluno").value = null;
                            dijit.byId("noAluno").value = null;
                            apresentaMensagem('apresentadorMensagem', null);
                            dijit.byId("limparAlunoPes").set('disabled', true);
                        }
                    }, "limparAlunoPes");

                    if (hasValue(document.getElementById("limparAlunoPes"))) {
                        document.getElementById("limparAlunoPes").parentNode.style.minWidth = '40px';
                        document.getElementById("limparAlunoPes").parentNode.style.width = '40px';
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
                                        abrirAlunoFKCad(ENVIARTRANSFERENCIACAD);
                                    });
                                }
                                else
                                    abrirAlunoFKCad(ENVIARTRANSFERENCIACAD);
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
                    }, "pesUnidadeCadDestino");

                    var pesUnidadeCadDestino = document.getElementById('pesUnidadeCadDestino');
                    if (hasValue(pesUnidadeCadDestino)) {
                        pesUnidadeCadDestino.parentNode.style.minWidth = '18px';
                        pesUnidadeCadDestino.parentNode.style.width = '18px';
                    }

                    new Button({
                        label: "Limpar", iconClass: '', type: "reset", disabled: true,
                        onClick: function () {
                            dojo.byId("no_unidade_cad_destino").value = "";
                            dojo.byId("cdUnidadePesUnidadeCadDestino").value = 0;
                            dijit.byId("limparUnidadePesCadDestino").set('disabled', true);
                            dojo.byId("emailCadDestino").value = "";
                            apresentaMensagem('apresentadorMensagem', null);
                        }
                    }, "limparUnidadePesCadDestino");
                    if (hasValue(document.getElementById("limparUnidadePesCadDestino"))) {
                        document.getElementById("limparUnidadePesCadDestino").parentNode.style.minWidth = '40px';
                        document.getElementById("limparUnidadePesCadDestino").parentNode.style.width = '40px';
                    }

                    new Button({
                        label: "",
                        iconClass: 'dijitEditorIcon dijitEditorIconDownload',
                        onClick: function() {
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
                                    apresentaMensagem('apresentadorMensagemReceberTransferencia', mensagensWeb);
                                    dijit.byId("uploaderHistoricoAluno")._files = [];
                                    return false;
                                } else {
                                    if (hasValue(files[0]) && (files[0].size > 1000000)) {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoHistoricoExcedeuTamanho);
                                        apresentaMensagem('apresentadorMensagemReceberTransferencia', mensagensWeb);
                                        dijit.byId("uploaderHistoricoAluno")._files = [];
                                        return false;
                                    }
                                    var nomeArquivo = files[0].name;
                                    var extArquivo = nomeArquivo.substr(nomeArquivo.length - 4, nomeArquivo.length);
                                    if (hasValue(extArquivo) && (extArquivo.toLowerCase().indexOf(".pdf") === -1)) {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtensaoFileHistoricoAlunoInvalida);
                                        apresentaMensagem('apresentadorMensagemReceberTransferencia', mensagensWeb);
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
                     * Grid EnviarTransferencias
                     */

                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/Secretaria/getEnviarTransferenciaAlunoSearch?desc=&cd_unidade_destino=0&cd_aluno=0&nm_raf=&cpf=&status_transferencia=1&dataIni=&dataFim=",
                            handleAs: "json",
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            idProperty: "cd_transferencia_aluno"
                        }),
                        Memory({ idProperty: "cd_transferencia_aluno" }));

                    var gridEnviarTransferencia = new EnhancedGrid({
                        store: ObjectStore({ objectStore: myStore }),
                        //store: ObjectStore({ objectStore: new dojo.store.Memory({ data: myStore }) }),
                        structure: [
                            {
                                name: "<input id='selecionaTodosEnviarTransferencia' style='display:none'/>",
                                field: "selecionadoEnviarTransferencia",
                                width: "5%",
                                styles: "text-align:center; min-width:15px; max-width:20px;",
                                formatter: formatCheckBoxEnviarTransferencia
                            },
                            //   { name: "Código", field: "cd_item", width: "5%", styles: "text-align: right; min-width:75px; max-width:75px;" },
                            { name: "Unidade de Destino ", field: "no_unidade_destino", width: "20%", styles: "min-width:80px;" },
                            { name: "Aluno ", field: "no_aluno", width: "30%", styles: "min-width:80px;" },
                            { name: "CPF ", field: "cpf", width: "15%", styles: "min-width:80px;" },
                            { name: "Email Enviado", field: "email_origem_enviado", width: "15%", styles: "min-width:80px;" },
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
                        "gridEnviarTransferencia"); // make sure you have a target HTML element with this id
                    // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                    gridEnviarTransferencia.pagination.plugin._paginator.plugin.connect(gridEnviarTransferencia.pagination.plugin._paginator,
                        'onSwitchPageSize',
                        function (evt) {
                            verificaMostrarTodos(evt, gridEnviarTransferencia, 'cd_item', 'selecionaTodosEnviarTransferencia');
                        });
                    var idGrupoItem = 0;
                    gridEnviarTransferencia.startup();
                    gridEnviarTransferencia.canSort = function (col) { return Math.abs(col) != 1; };
                    gridEnviarTransferencia.on("RowDblClick",
                        function (evt) {
                            try {

                                //limparGridEnviarTransferencias();
                                var idx = evt.rowIndex,
                                    item = this.getItem(idx),
                                    store = this.store;

                                limparFormEnviarTransferencias();

                                item.cd_transferencia_aluno = item.cd_transferencia_aluno == null ? 0 : item.cd_transferencia_aluno;
                                dojo.byId("cd_transferencia_aluno").value = item.cd_transferencia_aluno;
                                apresentaMensagem('apresentadorMensagem', '');
                                keepValues(null, item, gridEnviarTransferencia, false);
                                dijit.byId("cadEnviarTransferencia").show();
                                IncluirAlterar(0,
                                    'divAlterarEnviarTransferencia',
                                    'divIncluirEnviarTransferencia',
                                    'divExcluirEnviarTransferencia',
                                    'apresentadorMensagemEnviarTransferencia',
                                    'divCancelarEnviarTransferencia',
                                    'divLimparEnviarTransferencia');
                                //limparGrid();



                            } catch (e) {
                                hideCarregando();
                                postGerarLog(e);
                            }
                        }, true);

                    require(["dojo/aspect"],
                        function (aspect) {
                            aspect.after(gridEnviarTransferencia,
                                "_onFetchComplete",
                                function () {
                                    // Configura o check de todos:
                                    if (dojo.byId('selecionaTodosEnviarTransferencia').type == 'text')
                                        setTimeout(
                                            "configuraCheckBox(false, 'cd_transferencia_aluno', 'selecionadoEnviarTransferencia', -1, 'selecionaTodosEnviarTransferencia', 'selecionaTodosEnviarTransferencia', 'gridEnviarTransferencia')",
                                            gridEnviarTransferencia.rowsPerPage * 3);
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

function getComponentesEnviarTransferencia(ready, Memory, FilteringSelect) {
    showCarregando();
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
            function (dom, xhr, ref) {
                dojo.xhr.get({
                    url: Endereco() + "/api/Secretaria/getComponentesEnviarTransferenciaCad",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (retorno) {
                    try {
                        if (hasValue(retorno)) {
                            console.log(retorno);
                            limparFormEnviarTransferencias();
                            dojo.byId('dt_cadastro_transferencia_cad').value = dojo.date.locale.format(new Date(),
                                { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });

                            criarOuCarregarCompFiltering("cbMotivoTransferenciaCad", retorno.MotivosTransferencia, "", null, ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_motivo_transferencia_aluno', 'dc_motivo_transferencia_aluno');
                            dojo.byId("emailCadOrigem").value = retorno.emailOrigem;
                            dijit.byId("uploaderHistoricoAluno").set("disabled", true);
                            dijit.byId("btnDownHistoricoAlunoCad").set("disabled", true);
                            dijit.byId("btnShowHistoricoAlunoCad").set("disabled", true);
                            //dojo.byId("atendente").value = document.getElementById('nomeUsuario').innerText;
                            //dojo.byId('dt_atual').value = new Date();
                            dijit.byId("cbStatusTransferenciaCad").set("disabled", true);
                            dijit.byId("rafCad").set("disabled", true);
                            dijit.byId("ckEmailEnviadoCadOrigem").set("disabled", true);
                            dijit.byId("ckEmailEnviadoCadDestino").set("disabled", true);

                            
                            apresentaMensagem('apresentadorMensagem', null);
                            IncluirAlterar(1, 'divAlterarEnviarTransferencia', 'divIncluirEnviarTransferencia', 'divExcluirEnviarTransferencia', 'apresentadorMensagemEnviarTransferencia', 'divCancelarEnviarTransferencia', 'divLimparEnviarTransferencia');

                            dijit.byId("cadEnviarTransferencia").show();

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
                        { name: "Cadastrada", id: "0" },
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
                        { name: "Cadastrada", id: "0" },
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

function abrirAlunoFK(origem) {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = origem;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        
        limparPesquisaAlunoFK();
        if (dijit.byId("statusAlunoFK") != null && dijit.byId("statusAlunoFK") != undefined) {
            dijit.byId("statusAlunoFK").set("value", 1);
            dijit.byId("statusAlunoFK").set("disabled", true);
        }
        pesquisarAlunoFK(true, origem);
        dijit.byId("proAluno").show();
        dijit.byId('gridPesquisaAluno').update();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function abrirAlunoFKCad(origem) {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = origem;
        //dojo.byId('tipoRetornoAlunoFK').value = ReceberTransferenciaCAD;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        limparPesquisaAlunoFK();
        pesquisarAlunoFK(true, origem);
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
            dojo.byId("idOrigemPessoaFK").value = ENVIARTRANSFERENCIACAD;
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
                function(dom, xhr, ref) {
                    dojo.xhr.get({
                        url: Endereco() + "/api/Secretaria/getEmailUnidade?cdEscola=" + gridPessoaSelec.itensSelecionados[0].cd_pessoa,
                        preventCache: true,
                        handleAs: "json",
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json",
                            "Authorization": Token()
                        }
                    }).then(function(retorno) {
                        try {
                                if ((gridPessoaSelec.itensSelecionados[0].cd_pessoa + "") === dojo.byId("_ES0").value) {
                                    dojo.byId("no_unidade_cad_destino").value = "";
                                    dojo.byId("cdUnidadePesUnidadeCadDestino").value = 0;
                                    dijit.byId("limparUnidadePesCadDestino").set('disabled', true);
                                    dojo.byId("emailCadDestino").value = "";
                                    caixaDialogo(DIALOGO_AVISO, "A unidade de destino não pode ser igual a de origem", null);
                                }else
                                if (hasValue(retorno)) {
                                    console.log(retorno);
                                    dojo.byId("no_unidade_cad_destino").value =
                                        gridPessoaSelec.itensSelecionados[0].dc_reduzido_pessoa;
                                    dojo.byId("cdUnidadePesUnidadeCadDestino").value =
                                        gridPessoaSelec.itensSelecionados[0].cd_pessoa;
                                    dijit.byId('limparUnidadePesCadDestino').set("disabled", false);
                                    dojo.byId("emailCadDestino").value = retorno;
                                } else {
                                    dojo.byId("no_unidade_cad_destino").value =
                                        gridPessoaSelec.itensSelecionados[0].dc_reduzido_pessoa;
                                    dojo.byId("cdUnidadePesUnidadeCadDestino").value =
                                        gridPessoaSelec.itensSelecionados[0].cd_pessoa;
                                    dijit.byId('limparUnidadePesCadDestino').set("disabled", false);
                                    dojo.byId("emailCadDestino").value = "";
                                }

                                showCarregando();
                                

                               

                            } catch (e) {
                                showCarregando();
                                postGerarLog(e);

                            }
                        },
                        function(error) {
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

function limparFormEnviarTransferencias() {
    try {
        showCarregando();
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
            function (dom, xhr, ref) {
                dojo.xhr.get({
                    url: Endereco() + "/api/Secretaria/getComponentesEnviarTransferenciaCad",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (retorno) {
                    try {
                        if (hasValue(retorno)) {

                            dojo.byId('dt_cadastro_transferencia_cad').value = dojo.date.locale.format(new Date(),
                                { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });

                            criarOuCarregarCompFiltering("cbMotivoTransferenciaCad", retorno.MotivosTransferencia, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_motivo_transferencia_aluno', 'dc_motivo_transferencia_aluno');
                            dojo.byId("emailCadOrigem").value = retorno.emailOrigem;
                            dojo.byId("cd_transferencia_aluno").value = 0;
                            dijit.byId("cbStatusTransferenciaCad").set("value", 0);
                            dojo.byId("no_unidade_cad_destino").value = '';
                            dojo.byId("cdUnidadeCadDestino").value = 0;
                            dojo.byId("emailCadDestino").value = '';
                            dojo.byId("noAlunoCad").value = '';
                            dojo.byId("cdAlunoPesCad").value = 0;

                            dojo.byId("cdAlunoDest").value = 0;


                            dojo.byId("rafCad").value = '';
                            dijit.byId("ckEmailEnviadoCadOrigem").set("checked", false);
                            dijit.byId("ckEmailEnviadoCadDestino").set("checked", false);

                            dojo.byId("edlayoutHistoricoAlunoCad").value = '';


                            dijit.byId("uploaderHistoricoAluno").set("disabled", true);
                            dijit.byId("btnDownHistoricoAlunoCad").set("disabled", true);
                            dijit.byId("btnShowHistoricoAlunoCad").set("disabled", true);


                            dojo.byId("dt_solicitacao_transferencia_cad").value = '';
                            dojo.byId("dt_confirmacao_transferencia_cad").value = '';
                            dojo.byId("dt_transferencia_cad").value = '';

                            dijit.byId("cbStatusTransferenciaCad").set("disabled", true);
                            dijit.byId("rafCad").set("disabled", true);
                            dijit.byId("ckEmailEnviadoCadOrigem").set("disabled", true);
                            dijit.byId("ckEmailEnviadoCadDestino").set("disabled", true);
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

function limparGridEnviarTransferencias() {
    try {
        var gridAluno = dijit.byId('gridAluno');
        if (hasValue(gridAluno)) {
            gridAluno.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridAluno.update();
        }

        //var gridEnviarTransferencia = dijit.byId('gridEnviarTransferencia');
        //if (hasValue(gridEnviarTransferencia)) {
        //    gridEnviarTransferencia.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        //    gridEnviarTransferencia.update();
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


function IncluirEnviarTransferencia() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemEnviarTransferencia', null);
        var itemCad = new ObjItemCad();


        if (hasValue(itemCad)) {

            require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"],
                function (xhr, ref, windows) {
                    if (!validarCamposEnviarTransferencias(windows))
                        return false;

                    showCarregando();

                    xhr.post(Endereco() + "/api/Secretaria/postInsertEnviarTransferenciaAluno",
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
                                    var gridEnviarTransferencia = 'gridEnviarTransferencia';
                                    var grid = dijit.byId(gridEnviarTransferencia);
                                    apresentaMensagem('apresentadorMensagem', data);
                                    dijit.byId("cadEnviarTransferencia").hide();
                                    if (!hasValue(grid.itensSelecionados)) {
                                        grid.itensSelecionados = [];
                                    }
                                    insertObjSort(grid.itensSelecionados,
                                        "cd_transferencia_aluno",
                                        itemAlterado);
                                    buscarItensSelecionados(gridEnviarTransferencia,
                                        'selecionadoEnviarTransferencia',
                                        'cd_transferencia_aluno',
                                        'selecionaTodosEnviarTransferencia',
                                        ['pesquisarEnviarTransferencia', 'relatorioEnviarTransferencia'],
                                        'todosItensEnviarTransferencia');
                                    grid.sortInfo =
                                        2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                    setGridPagination(grid, itemAlterado, "cd_transferencia_aluno");
                                    hideCarregando();

                                } else {
                                    apresentaMensagem('apresentadorMensagemEnviarTransferencia', data);
                                    hideCarregando();
                                }
                            } catch (e) {
                                hideCarregando();
                                postGerarLog(e);
                            }
                        },
                            function (error) {
                                apresentaMensagem('apresentadorMensagemEnviarTransferencia', error);
                                hideCarregando();
                            });
                });



        } else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "");
            apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
        }
    } catch (e) {
        postGerarLog(e);
    } 
    
}

function validarCamposEnviarTransferencias(windowUtils) {
    try {
        var validado = true;
        apresentaMensagem("apresentadorMensagemEnviarTransferencia", null);
        

        if (!hasValue(dojo.byId("no_unidade_cad_destino").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Unidade de destino.");
            apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("emailCadOrigem").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Email de origem.");
            apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("emailCadDestino").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Email de destino.");
            apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("noAlunoCad").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Aluno.");
            apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("noAlunoCad").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Aluno.");
            apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
            return false;
        } 

        if (dijit.byId("cbMotivoTransferenciaCad").value == null || dijit.byId("cbMotivoTransferenciaCad").value == undefined || dijit.byId("cbMotivoTransferenciaCad").value <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Preencha o campo Motivo.");
            apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
            return false;
        }

        if (!dijit.byId("formEnviarTransferencias").validate()) {
            return false;
        }

        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function AlterarEnviarTransferencia() {
    try {

        if (dijit.byId("cbStatusTransferenciaCad").value == EnumsTipoStatusTransferenciaAluno.RECUSADA) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Transferência com status Recusada não pode ser alterada.");
            apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
            return false;
        }

        if (dijit.byId("cbStatusTransferenciaCad").value == EnumsTipoStatusTransferenciaAluno.EFETUADA) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Transferência com status Efetuada não pode ser alterada.");
            apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
            return false;
        }

        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemEnviarTransferencia', null);

        var itemEdit = null;

        //Salva com a imagem em base 64
        var files = dijit.byId("uploaderHistoricoAluno")._files;
        if (hasValue(files) && files.length > 0 && dijit.byId("cbStatusTransferenciaCad").value == EnumsTipoStatusTransferenciaAluno.APROVADA) {
            if (window.FormData !== undefined) {
                for (i = 0; i < files.length; i++) {
                    getBase64(files[i]).then(function (result) {


                        var item = {
                            cd_transferencia_aluno: dojo.byId("cd_transferencia_aluno").value > 0 ? dojo.byId("cd_transferencia_aluno").value : 0,
                            cd_escola_destino: dojo.byId("cdUnidadeCadDestino").value,
                            cd_aluno_origem: dojo.byId("cdAlunoPesCad").value,
                            cd_motivo_transferencia: dijit.byId("cbMotivoTransferenciaCad").value,
                            dc_email_origem: dojo.byId("emailCadOrigem").value,
                            dc_email_destino: dojo.byId("emailCadDestino").value,
                            id_status_transferencia: dijit.byId("cbStatusTransferenciaCad").value,
                            id_email_origem: dijit.byId("ckEmailEnviadoCadOrigem").checked,
                            id_email_destino: dijit.byId("ckEmailEnviadoCadDestino").checked,
                            no_arquivo_historico: dojo.byId("edlayoutHistoricoAlunoCad").value,
                            pdf_historico: result

                        }


                        require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"],
                            function (xhr, ref, windows) {
                                if (!validarCamposEnviarTransferencias(windows))
                                    return false;

                                if (item.id_status_transferencia == EnumsTipoStatusTransferenciaAluno.EFETUADA) {
                                    caixaDialogo(DIALOGO_AVISO, "Transferência efetuada não podem ser editada.", null);
                                    return false;
                                }


                                showCarregando();

                                xhr.post(Endereco() + "/api/Secretaria/postEditEnviarTransferenciaAluno",
                                    {
                                        headers: {
                                            "Accept": "application/json",
                                            "Content-Type": "application/json",
                                            "Authorization": Token()
                                        },
                                        handleAs: "json",
                                        data: ref.toJson(item)
                                    }).then(function (data) {
                                        try {
                                            data = jQuery.parseJSON(data);
                                            if (!hasValue(data.erro)) {
                                                var itemAlterado = data.retorno;
                                                var gridName = 'gridEnviarTransferencia';
                                                var grid = dijit.byId(gridName);
                                                apresentaMensagem('apresentadorMensagem', data);
                                                dijit.byId("cadEnviarTransferencia").hide();
                                                if (!hasValue(grid.itensSelecionados)) {
                                                    grid.itensSelecionados = [];
                                                }
                                                removeObjSort(grid.itensSelecionados, "cd_transferencia_aluno", dom.byId("cd_transferencia_aluno").value);
                                                insertObjSort(grid.itensSelecionados, "cd_transferencia_aluno", itemAlterado);
                                                buscarItensSelecionados(gridName,
                                                    'selecionadoEnviarTransferencia',
                                                    'cd_transferencia_aluno',
                                                    'selecionaTodosEnviarTransferencia',
                                                    ['pesquisarEnviarTransferencia', 'relatorioEnviarTransferencia'],
                                                    'todosItensEnviarTransferencia');
                                                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                                setGridPagination(grid, itemAlterado, "cd_transferencia_aluno");
                                                showCarregando();
                                            } else {
                                                apresentaMensagem('apresentadorMensagemEnviarTransferencia', data);
                                                showCarregando();
                                            }
                                        } catch (e) {
                                            hideCarregando();
                                            postGerarLog(e);
                                        }
                                    },
                                        function (error) {
                                            apresentaMensagem('apresentadorMensagemMat', error);
                                            hideCarregando();
                                        });
                            });

                    })
                        .catch(function (e) { console.log(e) });
                }
            } else {
                itemEdit = new ObjItemEdit();
            }
        } else {
            itemEdit = new ObjItemEdit();
        }


        //Salva quando não há arquivo
        if (itemEdit != null) {
            require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/window"],
                function (dom, xhr, ref, windows) {

                    if (!validarCamposEnviarTransferencias(windows))
                        return false;

                    if (itemEdit.id_status_transferencia == EnumsTipoStatusTransferenciaAluno.EFETUADA) {
                        caixaDialogo(DIALOGO_AVISO, "Transferência efetuada não podem ser editada.", null);
                        return false;
                    }

                    showCarregando();

                    xhr.post(Endereco() + "/api/Secretaria/postEditEnviarTransferenciaAluno",
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
                                    var gridName = 'gridEnviarTransferencia';
                                    var grid = dijit.byId(gridName);
                                    apresentaMensagem('apresentadorMensagem', data);
                                    dijit.byId("cadEnviarTransferencia").hide();
                                    if (!hasValue(grid.itensSelecionados)) {
                                        grid.itensSelecionados = [];
                                    }
                                    removeObjSort(grid.itensSelecionados, "cd_transferencia_aluno", dom.byId("cd_transferencia_aluno").value);
                                    insertObjSort(grid.itensSelecionados, "cd_transferencia_aluno", itemAlterado);
                                    buscarItensSelecionados(gridName,
                                        'selecionadoEnviarTransferencia',
                                        'cd_transferencia_aluno',
                                        'selecionaTodosEnviarTransferencia',
                                        ['pesquisarEnviarTransferencia', 'relatorioEnviarTransferencia'],
                                        'todosItensEnviarTransferencia');
                                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                    setGridPagination(grid, itemAlterado, "cd_transferencia_aluno");
                                    showCarregando();
                                } else {
                                    apresentaMensagem('apresentadorMensagemEnviarTransferencia', data);
                                    showCarregando();
                                }
                            } catch (er) {
                                postGerarLog(er);
                            }
                        },
                            function (error) {
                                showCarregando();
                                apresentaMensagem('apresentadorMensagemEnviarTransferencia', error.response.data);
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
            cd_escola_destino: dojo.byId("cdUnidadeCadDestino").value,
            cd_aluno_origem: dojo.byId("cdAlunoPesCad").value,
            cd_motivo_transferencia: dijit.byId("cbMotivoTransferenciaCad").value,
            dc_email_origem: dojo.byId("emailCadOrigem").value,
            dc_email_destino: dojo.byId("emailCadDestino").value,
            id_status_transferencia: dijit.byId("cbStatusTransferenciaCad").value,
            id_email_origem: dijit.byId("ckEmailEnviadoCadOrigem").checked,
            id_email_destino: dijit.byId("ckEmailEnviadoCadDestino").checked,
        }

        return item;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ObjItemCad() {
    try {
        if (hasValue(dojo.byId("dt_cadastro_transferencia_cad").value)) {
            this.dataCad = dojo.date.locale.parse(dojo.byId("dt_cadastro_transferencia_cad").value,
                { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            var dt_cadastro_transferencia_cad = dojo.date.locale.format(this.dataCad,
                {
                    selector: "date",
                    datePattern: "yyyy-MM-dd",
                    formatLength: "short",
                    locale: "pt-br"
                });
        }
        var item = {
            cd_transferencia_aluno: 0,
            cd_escola_destino: dojo.byId("cdUnidadePesUnidadeCadDestino").value,
            cd_aluno_origem: dojo.byId("cdAlunoPesCad").value,
            cd_motivo_transferencia: dijit.byId("cbMotivoTransferenciaCad").value,
            dt_cadastro_transferencia: dt_cadastro_transferencia_cad,
            dc_email_origem: dojo.byId("emailCadOrigem").value,
            dc_email_destino: dojo.byId("emailCadDestino").value,
            id_status_transferencia: dijit.byId("cbStatusTransferenciaCad").value,
            id_email_origem: false,
            id_email_destino: false,
        }

        return item;
    } catch (e) {

    }
}

function getBase64(file) {
    return new Promise((resolve, reject) => {
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}


function eventoEditarEnviarTransferencia(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {


            var gridEnviarTransferencia = dijit.byId("gridEnviarTransferencia");


            dojo.byId("cd_transferencia_aluno").value = itensSelecionados[0].cd_transferencia_aluno;
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(null, itensSelecionados[0], gridEnviarTransferencia, false);





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

                    if (itensSelecionados[0].id_status_transferencia == EnumsTipoStatusTransferenciaAluno.APROVADA) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não é permitido enviar email de solicitação para transferência com status aprovada.");
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return false;
                    }

                    if (itensSelecionados[0].id_status_transferencia == EnumsTipoStatusTransferenciaAluno.RECUSADA) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não é permitido enviar email de solicitação para transferência com status recusada.");
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return false;
                    }

                    if (itensSelecionados[0].id_status_transferencia == EnumsTipoStatusTransferenciaAluno.EFETUADA) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não é permitido enviar email de solicitação para transferência com status efetuada.");
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return false;
                    }

                    if (itensSelecionados[0].id_email_origem == true) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Email já enviado.");
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
                        return false;
                    }
                    

                    showCarregando();

                    xhr.post(Endereco() + "/api/Secretaria/PostSendEmailSolicitarTransferenciaAluno",
                        {
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            handleAs: "json",
                            data: ref.toJson({ cd_transferencia_aluno: itensSelecionados[0].cd_transferencia_aluno, id_status_transferencia: itensSelecionados[0].id_status_transferencia})
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
                                PesquisarEnviarTransferencia(true);

                                hideCarregando();
                            } catch (e) {
                                hideCarregando();
                                postGerarLog(e);
                            }
                        },
                            function (error) {
                                apresentaMensagem('apresentadorMensagemEnviarTransferencia', error);
                                hideCarregando();
                            });
                });



            

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


function eventoTransferirAluno(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
                if (itensSelecionados[0].id_status_transferencia != EnumsTipoStatusTransferenciaAluno.APROVADA) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Só é permitido realizar esta operação quando o status da transferência estiver como aprovada.");
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
                    return false;
                }
                //gerarHistorico(itensSelecionados, xhr, function () { postEnviar(itensSelecionados, xhr) });
                postEnviar(itensSelecionados, xhr);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function gerarHistorico(itensSelecionados, xhr, pFuncao) {
    if (!hasValue(itensSelecionados[0].pdf_historico)) {
        showCarregando();
        this.historico = "0";
        this.avaliacao = "0";
        this.avalEstagio = "0";
        this.titulos = "0";
        this.mostrarEstagio = true;
        this.mostrarAtividade = true;
        this.mostrarObservacao = true;
        this.mostrarFollow = true;
        this.mostrarItem = true;
        this.cd_aluno = itensSelecionados[0].cd_aluno_origem;
        this.no_aluno = itensSelecionados[0].no_aluno;

        var HistoricoAluno = function (historico, avaliacao, avalEstagio, titulos, mostrarEstagio, mostrarAtividade, mostrarObservacao, mostrarFollow, mostrarItem, cd_aluno, no_aluno) {
            this.historico = historico;
            this.avaliacao = avaliacao;
            this.avalEstagio = avalEstagio;
            this.titulos = titulos;
            this.mostrarEstagio = mostrarEstagio;
            this.mostrarAtividade = mostrarAtividade;
            this.mostrarObservacao = mostrarObservacao;
            this.mostrarFollow = mostrarFollow;
            this.mostrarItem = mostrarItem;
            this.cd_aluno = cd_aluno;
            this.no_aluno = no_aluno;
        };

        /*Historico Aluno*/
        historicoAluno = new HistoricoAluno(historico, avaliacao, avalEstagio, titulos, mostrarEstagio, mostrarAtividade, mostrarObservacao, mostrarFollow, mostrarItem, cd_aluno, no_aluno);
        require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"],
            function (xhr, ref, windows) {

            dojo.xhr.get({
                url: Endereco() + "/relatorio/GerarHistorico",
                preventCache: true,
                handleAs: "json",
                content: {
                    sort: "",
                    produtos: historicoAluno.historico,
                    turmaAvaliacao: historicoAluno.avaliacao,
                    estagioAvaliacao: historicoAluno.avalEstagio,
                    statusTitulo: historicoAluno.titulos,
                    mostrarEstagio: historicoAluno.mostrarEstagio,
                    mostrarAtividade: historicoAluno.mostrarAtividade,
                    mostrarObservacao: historicoAluno.mostrarObservacao,
                    mostrarFollow: historicoAluno.mostrarFollow,
                    mostrarItem: historicoAluno.mostrarItem,
                    cd_aluno: historicoAluno.cd_aluno,
                    no_aluno: historicoAluno.no_aluno
                },
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                hideCarregando();
                try {
                    window.open(data);
                    window.focus();
                    window.close();
                    //if (hasValue(pFuncao))
                    //    pFuncao.call();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            });
        })

    } else {
        hideCarregando();
        showMessage('apresentadorMensagem', MENSAGEM_ERRO, "O Campo Aluno é obrigatório.");
    }

}

function postEnviar(itensSelecionados, xhr) {
    historicoAluno = null;
    if (!hasValue(itensSelecionados[0].pdf_historico)) {
        this.produtos = "0";
        this.turmaAvaliacao = "0";
        this.estagioAvaliacao = "0";
        this.statusTitulo = "0";
        this.mostrarEstagio = true;
        this.mostrarAtividade = true;
        this.mostrarObservacao = true;
        this.mostrarFollow = true;
        this.mostrarItem = true;
        this.cd_aluno = itensSelecionados[0].cd_aluno_origem;
        this.no_aluno = itensSelecionados[0].no_aluno;

        var HistoricoAluno = function (historico, avaliacao, avalEstagio, titulos, mostrarEstagio, mostrarAtividade, mostrarObservacao, mostrarFollow, mostrarItem, cd_aluno, no_aluno) {
            this.produtos = produtos;
            this.turmaAvaliacao = turmaAvaliacao;
            this.estagioAvaliacao = estagioAvaliacao;
            this.statusTitulo = statusTitulo;
            this.mostrarEstagio = mostrarEstagio;
            this.mostrarAtividade = mostrarAtividade;
            this.mostrarObservacao = mostrarObservacao;
            this.mostrarFollow = mostrarFollow;
            this.mostrarItem = mostrarItem;
            this.cd_aluno = cd_aluno;
            this.no_aluno = no_aluno;
        };

        /*Historico Aluno*/
        historicoAluno = new HistoricoAluno(produtos, turmaAvaliacao, estagioAvaliacao, statusTitulo, mostrarEstagio, mostrarAtividade, mostrarObservacao, mostrarFollow, mostrarItem, cd_aluno, no_aluno);

    }
    

    require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"],
        function (xhr, ref, windows) {
        showCarregando();

        xhr.post(Endereco() + "/api/Secretaria/PostTransferirAluno",
            {
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": Token()
                },
                handleAs: "json",
                data: ref.toJson({ cd_transferencia_aluno: itensSelecionados[0].cd_transferencia_aluno, id_status_transferencia: itensSelecionados[0].id_status_transferencia, no_unidade_origem: itensSelecionados[0].no_unidade_origem, historicoAlunoReport: historicoAluno })
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
                    PesquisarEnviarTransferencia(true);

                    hideCarregando();
                } catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
                hideCarregando();
            });
        })
}

function keepValues(Form, value, grid, ehLink) {
    try {
        getLimpar('#formEnviarTransferencias');
        clearForm('formEnviarTransferencias');
        limparGridEnviarTransferencias();
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
                        url: Endereco() + "/api/Secretaria/getEnviarTransferenciaAlunoForEdit?cd_transferencia_aluno=" + value.cd_transferencia_aluno,
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
                                dijit.byId("cbStatusTransferenciaCad").set("value", dados.id_status_transferencia);
                                dojo.byId("no_unidade_cad_destino").value = dados.no_unidade_destino;
                                dojo.byId("cdUnidadeCadDestino").value = dados.cd_escola_destino;
                                dojo.byId("emailCadOrigem").value = dados.dc_email_origem;
                                dojo.byId("emailCadDestino").value = dados.dc_email_destino;
                                dojo.byId("noAlunoCad").value = dados.no_aluno;
                                dojo.byId("cdAlunoPesCad").value = dados.cd_aluno_origem;
                                if (dados.cd_aluno_destino != null &&
                                    dados.cd_aluno != undefined &&
                                    dados.cd_aluno_destino > 0) {
                                    dojo.byId("cdAlunoDest").value = dados.cd_aluno_destino;
                                }

                                dojo.byId("rafCad").value = dados.nm_raf;
                                dijit.byId("ckEmailEnviadoCadOrigem").set("checked", dados.id_email_origem);
                                dijit.byId("ckEmailEnviadoCadDestino").set("checked", dados.id_email_destino);
                                if (hasValue(dados.no_arquivo_historico)) {
                                    dojo.byId("edlayoutHistoricoAlunoCad").value = dados.no_arquivo_historico;
                                }
                                if (dados.id_status_transferencia == EnumsTipoStatusTransferenciaAluno.APROVADA) {
                                    
                                    dijit.byId("uploaderHistoricoAluno").set("disabled", 'false');
                                    dijit.byId("uploaderHistoricoAluno").set("disabled", false);
                                    
                                } else {
                                    dijit.byId("uploaderHistoricoAluno").set("disabled", 'true');
                                }

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

                                dijit.byId("cbStatusTransferenciaCad").set("disabled", true);
                                dijit.byId("rafCad").set("disabled", true);
                                dijit.byId("ckEmailEnviadoCadOrigem").set("disabled", true);
                                dijit.byId("ckEmailEnviadoCadDestino").set("disabled", true);

                                dijit.byId("cadEnviarTransferencia").show();
                                IncluirAlterar(0,
                                    'divAlterarEnviarTransferencia',
                                    'divIncluirEnviarTransferencia',
                                    'divExcluirEnviarTransferencia',
                                    'apresentadorMensagemEnviarTransferencia',
                                    'divCancelarEnviarTransferencia',
                                    'divLimparEnviarTransferencia');
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


function PesquisarEnviarTransferencia(limparItens) {

    var params = montarParametrosUrlEnviarTransferencia();
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/Secretaria/getEnviarTransferenciaAlunoSearch?cd_unidade_destino=" + (params.cd_unidade || "") + "&cd_aluno=" + (params.cd_aluno || "") + "&nm_raf=" + (params.nm_raf || "") + "&cpf=" + (params.cpf || "") + "&status_transferencia=" + (params.statusTransferencia || "") + "&dataIni=" + (params.dataIni || "") + "&dataFim=" + (params.dataFim || "") ,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_transferencia_aluno"
                }
                ), Memory({ idProperty: "cd_transferencia_aluno" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridEnviarTransferencia = dijit.byId("gridEnviarTransferencia");
            if (limparItens) {
                gridEnviarTransferencia.itensSelecionados = [];
            }
            gridEnviarTransferencia.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarParametrosUrlEnviarTransferencia() {
    this.cd_unidade = hasValue(dojo.byId("cdUnidadePesUnidade").value) ? dojo.byId("cdUnidadePesUnidade").value : null;
    this.cd_aluno = hasValue(dojo.byId("cdAlunoPes").value) ? dojo.byId("cdAlunoPes").value : null;
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

    var EnviarTransferencias = function (cd_unidade, cd_aluno, nm_raf, cpf, statusTransferencia, dataIni, dataFim) {
        this.cd_unidade = cd_unidade;
        this.cd_aluno = cd_aluno;
        this.nm_raf = nm_raf;
        this.cpf = cpf
        this.statusTransferencia = statusTransferencia;
        this.dataIni = dataIni;
        this.dataFim = dataFim;
    }

    
    objEnviarTransferencias = new EnviarTransferencias(cd_unidade, cd_aluno, nm_raf, cpf, statusTransferencia, dataIni, dataFim);
    return objEnviarTransferencias;
}


function DeletarEnviarTransferencia(itensSelecionados) {
    
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
        function (dom, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                if (dojo.byId('cd_transferencia_aluno').value != 0) {
                    itensSelecionados = [{ cd_transferencia_aluno: dom.byId("cd_transferencia_aluno").value, id_status_transferencia: dijit.byId("cbStatusTransferenciaCad").value }];

                }

            } else {

                itensSelecionados = itensSelecionados.map((function (a) { return { cd_transferencia_aluno: a.cd_transferencia_aluno, id_status_transferencia: a.id_status_transferencia } }));
            }

            if (itensSelecionados[0].id_status_transferencia == EnumsTipoStatusTransferenciaAluno.RECUSADA) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Transferência com status Recusada não pode ser deletada.");
                apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                return false;
            }

            if (itensSelecionados[0].id_status_transferencia == EnumsTipoStatusTransferenciaAluno.EFETUADA) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Transferência com status Efetuada não pode ser deletada.");
                apresentaMensagem("apresentadorMensagemEnviarTransferencia", mensagensWeb);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                return false;
            }

            xhr.post({
                url: Endereco() + "/api/Secretaria/PostDeleteTransferenciaAluno",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensEnviarTransferencia");
                        apresentaMensagem('apresentadorMensagem', data);
                        data = jQuery.parseJSON(data).retorno;
                        dijit.byId("cadEnviarTransferencia").hide();
                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridEnviarTransferencia').itensSelecionados,
                                "cd_transferencia_aluno",
                                itensSelecionados[r].cd_transferencia_aluno);
                        PesquisarEnviarTransferencia(true);
                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarEnviarTransferencia").set('disabled', false);
                        dijit.byId("relatorioEnviarTransferencia").set('disabled', false);
                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    } else {
                        if (!hasValue(dojo.byId("cadEnviarTransferencia").style.display))
                            apresentaMensagem('apresentadorMensagemEnviarTransferencia', error);
                        else
                            apresentaMensagem('apresentadorMensagem', error);
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    if (!hasValue(dojo.byId("cadEnviarTransferencia").style.display))
                        apresentaMensagem('apresentadorMensagemEnviarTransferencia', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                });
        });
}