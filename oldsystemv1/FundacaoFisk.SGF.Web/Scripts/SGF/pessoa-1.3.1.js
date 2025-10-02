var PESSOAJURIDICA = 2, PESSOAFISICA = 1;
var RELAENTREPESSOAEEMPRESA = 5

function montaGridPessoa() {
    require([
        "dojo/ready",
	    "dojo/_base/xhr",
	    "dojox/grid/EnhancedGrid",
	    "dojox/grid/enhanced/plugins/Pagination",
	    "dojo/store/JsonRest",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/on",
        "dijit/form/Button",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dijit/Dialog",
        "dijit/form/DateTextBox",
	    "dojo/domReady!"
    ], function (ready, xhr, EnhancedGrid, Pagination, JsonRest, Cache, Memory, on, Button, DropDownButton, DropDownMenu, MenuItem) {
        ready(function () {
            try {

                getParametroEscolaInternacional();

                $("#tdLabelPessoaAtiva").css("display", "none");
                $("#tdPessoaAtiva").css("display", "none");
                dojo.byId("descApresMsg").value = 'apresentadorMensagemPessoa';
                var myStore =
                Cache(
                        JsonRest({
                            target: Endereco() + "/api/aluno/GetPessoaSearch?nome=&apelido=&tipoPessoa=" + parseInt(0) + "&cnpjCpf=&papel=&sexo=0&inicio=false",
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }), Memory({}));


                /* Formatar o valor em armazenado, de modo a serem exibidos.*/
                var gridPessoa = new EnhancedGrid({
                    //store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure: [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoPessoa", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                       // { name: "Código", field: "cd_pessoa", width: "60px", styles: "text-align: right; min-width:60px; max-width:60px;" },
                        { name: "Nome", field: "no_pessoa", width: "30%", styles: "min-width:10%; max-width: 50%;" },
                        { name: "CPF\\CNPJ", field: "nm_cpf_cgc_dependente", width: "20%" },
                        { name: "Nome Reduzido", field: "dc_reduzido_pessoa", width: "100px" },
                        { name: "Data Cadastro", field: "dta_cadastro", width: "80px", styles: "min-width:80px; max-width:80px;" },
                        { name: "Natureza", field: "natureza_pessoa", width: "60px", styles: "text-align: left;" },
                        { name: "Papel", field: "papel", width: "10%", styles: "text-align: left; min-width:40px; max-width: 10%;" }
                    ],
                    noDataMessage: msgNotRegEncFiltro,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button",
                            plugins: { nestedSorting: false }
                        }
                    }
                }, "gridPessoa");
                gridPessoa.pagination.plugin._paginator.plugin.connect(gridPessoa.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    try {
                        verificaMostrarTodos(evt, gridPessoa, 'cd_pessoa', 'selecionaTodos');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPessoa, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoPessoa', -1, 'selecionaTodos', 'selecionaTodos', 'gridPessoa')", gridPessoa.rowsPerPage * 3);
                    });
                });
                gridPessoa.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 7; };
                gridPessoa.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                            item = this.getItem(idx),
                            store = this.store;
                        keepValues(item, gridPessoa, null);
                        IncluirAlterar(0, 'divAlterarPessoa', 'divIncluirPessoa', 'divExcluirPessoa', 'apresentadorMensagemPessoa', 'divCancelarPessoa', 'divClearPessoa');
                        dijit.byId("formulario").show();
                        //showPessoaFK(item.nm_natureza_pessoa, item.cd_pessoa, 2);
                        //dijit.byId("formulario").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridPessoa.startup();
                new Button({ label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisaPessoa(true); } }, "pesquisarPessoa");
                decreaseBtn(document.getElementById("pesquisarPessoa"), '32px');
                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        try {
                            dojo.xhr.get({
                                url: Endereco() + "/api/aluno/GetUrlRelatorioPessoa?" + getStrGridParameters('gridPessoa') + "nome=" + dojo.byId("_nome").value + "&apelido=" + dojo.byId("_fantasia").value +
                                "&tipoPessoa=" + parseInt(dijit.byId("tipoPessoa").value) + "&cnpjCpf=" + dojo.byId("_cnpjCpf").value + "&papel=" + dijit.byId('papel').value + "&sexo=" + dijit.byId("nm_sexo").value + "&inicio=" + document.getElementById("inicioPessoa").checked,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                try {
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioPessoa");
                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function () {
                        try {
                            IncluirAlterar(1, 'divAlterarPessoa', 'divIncluirPessoa', 'divExcluirPessoa', 'apresentadorMensagemPessoa', 'divCancelarPessoa', 'divClearPessoa');
                            dijit.byId('naturezaPessoa').set("disabled", false);
                            var papelRelac = new Array();
                            papelRelac[0] = RELAENTREPESSOAEEMPRESA;
                            showPessoaFK(PESSOAFISICA, 0, papelRelac);
                            sugereDataCorrente();
                            dijit.byId("formulario").show();
                            dijit.byId('excluirFoto').setAttribute('disabled', 1);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novaPessoa");
                dojo.query("#_nome").on("keyup", function (e) {
                    if (e.keyCode == 13) {
                        apresentaMensagem('apresentadorMensagem', null);
                        pesquisaPessoa(true);
                    }
                });
                adicionarAtalhoPesquisa(['_nome', '_fantasia', 'papel', 'tipoPessoa', '_cnpjCpf', 'nm_sexo'], 'pesquisarPessoa', ready);
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        try {
                            if (hasValue(dijit.byId("naturezaPessoa")) && hasValue(dijit.byId("naturezaPessoa").value)) {
                                if (dijit.byId("naturezaPessoa").value == PESSOAFISICA)
                                    incluirPessoaFisica();
                                else
                                    incluirPessoajuridica();
                            } else {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Nenhuma natureza foi selecionado.");
                                apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirPessoa");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        try {
                            limparCadPessoaFK();
                            keepValues(null, dijit.byId("gridPessoa"), false);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarPessoa");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formulario").hide(); } }, "fecharPessoa");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        try {
                            if (hasValue(dijit.byId("naturezaPessoa")) && hasValue(dijit.byId("naturezaPessoa").value)) {
                                if (dijit.byId("naturezaPessoa").value == PESSOAFISICA)
                                    alterarPessoaFisica();
                                else
                                    alterarPessoajuridica();
                            } else {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Nenhuma natureza foi selecionada.");
                                apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "alterarPessoa");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarPessoa() }); } }, "deletePessoa");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                        limparCadPessoaFK();
                    }
                }, "limparPessoa");
                loadNaturezaPessoaPesquisa();
                loadPesqSexo(Memory, dijit.byId("nm_sexo"));
                populaPapeis(null, "papel");
                dojo.byId("_cnpjCpf").value = "";
                dojo.byId("_cnpjCpf").readOnly = true;

                //Metodos para criação do link
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditar(gridPessoa.itensSelecionados); }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemoverPessoa(gridPessoa.itensSelecionados); }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dojo.byId("linkAcoes").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridPessoa, 'todosItens', ['pesquisarPessoa', 'relatorioPessoa']);
                        pesquisaPessoa(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridPessoa', 'selecionadoPessoa', 'cd_pessoa', 'selecionaTodos', ['pesquisarPessoa', 'relatorioPessoa'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dojo.byId("linkSelecionados").appendChild(button.domNode);
                //atachando evento de Procura de cpf e cnpj
                if (hasValue(dijit.byId("cpf")))
                    dijit.byId("cpf").on("blur", function (evt) {
                        try {
                            if (trim(dojo.byId("cpf").value) != "" && dojo.byId("cpf").value != "___.___.___-__")
                                if (validarCPF("#cpf", "apresentadorMensagemPessoa")) {
                                    cleanUsarCpf();
                                    ExtistsPessoByCpf();
                                    apresentaMensagem('apresentadorMensagemPessoa', '');
                                }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                if (hasValue(dijit.byId("cnpj")))
                    dijit.byId("cnpj").on("blur", function (evt) {
                        try {
                            if (trim(dojo.byId("cnpj").value) != "" && dojo.byId("cnpj").value != "__.___.___/____-__")
                                if (validarCnpj("#cnpj", "apresentadorMensagemPessoa"))
                                    //validarCPF() ?
                                    ExisitsEmpresaByCnpj();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                toggleDisabled(dijit.byId("nm_sexo"), true);
                dijit.byId("tipoPessoa").on("change", function (e) {
                    try {
                        dijit.byId("nm_sexo").set("value", 0);
                        if (e == 1)
                            toggleDisabled(dijit.byId("nm_sexo"), false);
                        else
                            toggleDisabled(dijit.byId("nm_sexo"), true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            try {
                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323045', '765px', '771px');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        });
                }
                //fim
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function ExtistsPessoByCpf() {
    try {
        var mensagensWeb = new Array();
        if ($("#cpf").val()) {
            dojo.xhr.get({
                url: Endereco() + "/api/secretaria/existePessoaEscolaOrByCpf?cpf=" + $("#cpf").val(),
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                data = jQuery.parseJSON(data);
                try {
                    if (data.retorno != null && data.retorno.pessoaFisica != null) {
                        caixaDialogo(DIALOGO_CONFIRMAR, data.MensagensWeb[0].mensagem, function executaRetorno() {
                            limparCadPessoaFK();
                            setarValuePessoaFisica(data.retorno);
                        });
                    }
                    else {
                        if (hasValue(data.MensagensWeb) && hasValue(data.MensagensWeb[0]))
                            caixaDialogo(DIALOGO_AVISO, data.MensagensWeb[0].mensagem, 0, 0, 0);
                        if (data != null && hasValue(data.erro))
                            apresentaMensagem(dojo.byId("descApresMsg").value, data.erro);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem(dojo.byId("descApresMsg").value, error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ExisitsEmpresaByCnpj() {
    try {
        var mensagensWeb = new Array();
        if ($("#cnpj").val()) {
            dojo.xhr.get({
                url: Endereco() + "/api/secretaria/existePessoaJuridicaEscolaOrByCpf?cnpj=" + $("#cnpj").val(),
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Accept": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (data.retorno != null && data.retorno.pessoaJuridica != null) {
                        caixaDialogo(DIALOGO_CONFIRMAR, data.MensagensWeb[0].mensagem, function executaRetorno() {
                            limparCadPessoaFK();
                            setarValuePessoaJuridica(data.retorno);
                        });
                    }
                    else {
                        if (hasValue(data.MensagensWeb) && hasValue(data.MensagensWeb[0]))
                            caixaDialogo(DIALOGO_AVISO, data.MensagensWeb[0].mensagem, 0, 0, 0);
                        if (data != null && hasValue(data.erro))
                            apresentaMensagem(dojo.byId("descApresMsg").value, data.erro);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem(dojo.byId("descApresMsg").value, error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaPapeis(idPapel, field) {
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
                    loadPapeis(data.retorno, field, idPapel);
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
        s
        postGerarLog(e);
    }
}

function loadPapeis(items, linkPapel, idPapel) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbPapel = dijit.byId(linkPapel);

            itemsCb.push({ id: 0, name: "Todos" });
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_papel, name: value.no_papel });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbPapel.store = stateStore;
            if (!hasValue(idPapel))
                idPapel = 0;
            cbPapel._onChangeActive = false;
            cbPapel.set("value", idPapel);
            cbPapel._onChangeActive = true;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

// ** fim da grade de Escola **\\

function pesquisaPessoa(limparItens) {
    require([
          "dojo/ready"
    ], function (ready) {
        ready(function () {
            try {
                findPessoa(dojo.byId("_nome").value, dojo.byId("_fantasia").value, dijit.byId("tipoPessoa").value, dojo.byId("_cnpjCpf").value, limparItens, dijit.byId('papel').value);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function findPessoa(nome, apelido, tipoPessoa, cnpjCpf, limparItens, cdPapel) {
    try {
        if (nome != "" || apelido != "" || status != "" || tipoPessoa != "" || cnpjCpf != "") {
            require(["dojo/store/JsonRest", "dojox/data/JsonRestStore", "dojo/data/ObjectStore","dojo/store/Cache", "dojo/store/Memory"],
            function ( JsonRest, JsonRestStore, ObjectStore, Cache, Memory) {
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/aluno/GetPessoaSearch?nome=" + encodeURIComponent(nome) + "&apelido=" + encodeURIComponent(apelido) + "&tipoPessoa=" + parseInt(tipoPessoa) + "&cnpjCpf=" + cnpjCpf + "&papel=" + cdPapel + "&sexo=" + dijit.byId("nm_sexo").value + "&inicio=" + document.getElementById("inicioPessoa").checked,
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));

                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPessoa");
                grid.setStore(dataStore);
                grid.noDataMessage = msgNotRegEnc;

                if (limparItens)
                    grid.itensSelecionados = [];
            })
        }
        else {
            mensagemPesquisa();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadNaturezaPessoaPesquisa() {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         try {
             var statusStore = new Memory({
                 data: [
                 { name: "Todos", id: "0" },
                 { name: "Física", id: "1" },
                 { name: "Jurídica", id: "2" }
                 ]
             });

             var tipoPessoa = new filteringSelect({
                 id: "tipoPessoa",
                 name: "tipoPessoa",
                 store: statusStore,
                 value: "0",
                 searchAttr: "name",
                 style: "width:90px;"
             }, "tipoPessoa");

             tipoPessoa.on("change", function () {
                 try {
                     if (this.get("value") == 0) {
                         dojo.byId("_cnpjCpf").value = "";
                         $('#_cnpjCpf').unmask();
                     }
                     dojo.byId("_cnpjCpf").readOnly = true;
                     if (this.get("value") == 1) {
                         dojo.byId("_cnpjCpf").value = "";
                         dojo.byId("_cnpjCpf").readOnly = false;
                         $("#_cnpjCpf").mask("999.999.999-99");
                     }
                     if (this.get("value") == 2) {
                         dojo.byId("_cnpjCpf").value = "";
                         dojo.byId("_cnpjCpf").readOnly = false;
                         $("#_cnpjCpf").mask("99.999.999/9999-99")
                     }
                 }
                 catch (e) {
                     postGerarLog(e);
                 }
             });
         }
         catch (e) {
             postGerarLog(e);
         }
     });
}

function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            //limparCadPessoaFK();
            apresentaMensagem('apresentadorMensagemPessoa', '');

            keepValues(null, dijit.byId('gridPessoa'), true);
            dijit.byId("formulario").show();
            IncluirAlterar(0, 'divAlterarPessoa', 'divIncluirPessoa', 'divExcluirPessoa', 'apresentadorMensagemPessoa', 'divCancelarPessoa', 'divClearPessoa');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverPessoa(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { DeletarPessoa(itensSelecionados); });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValues(value, grid, ehLink) {
    try {
        naturezaPessoa = dijit.byId('naturezaPessoa');
        naturezaPessoa.set("disabled", true);
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
            var papelRelac = new Array();
            papelRelac[0] = RELAENTREPESSOAEEMPRESA;
            showPessoaFK(value.nm_natureza_pessoa, value.cd_pessoa, papelRelac);
            //ShowFoto(value);
            return false;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function DeletarPessoa(itensSelecionados) {
    showCarregando();
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_pessoa').value != 0)
                    itensSelecionados = [{
                        cd_pessoa: dom.byId("cd_pessoa").value,
                        no_pessoa: dom.byId("nomPessoa").value
                    }];
            xhr.post({
                url: Endereco() + "/api/pessoa/postdeletePessoa",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItens_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formulario").hide();
                    dijit.byId("_nome").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridPessoa').itensSelecionados, "cd_pessoa", itensSelecionados[r].cd_pessoa);

                    pesquisaPessoa(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarPessoa").set('disabled', false);
                    dijit.byId("relatorioPessoa").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                    showCarregando();
                }
                catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formulario").style.display)) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemPessoa', error);
                }
                else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagem', error);
                }
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function incluirPessoajuridica() {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref", "dojo/window"], function (dom, domAttr, xhr, ref, windowUtils) {
        try {
            if (!validateCadPessoaFK(windowUtils))
                return false;
            if (!validarCnpj("#cnpj", "apresentadorMensagemPessoa"))
                return false;
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/secretaria/postInsertPessoaJuridica",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(montarDadosPessoaJuridica(dom, domAttr))
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridPessoa';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        data = data.retorno;

                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        insertObjSort(grid.itensSelecionados, "cd_pessoa", itemAlterado);
                        dijit.byId("formulario").hide();
                        buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_pessoa', 'selecionaTodos', ['pesquisarPessoa', 'relatorioPessoa'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_pessoa");
                    } else
                        apresentaMensagem('apresentadorMensagemPessoa', data);
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemPessoa', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function incluirPessoaFisica() {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref", "dojo/window", "dojo/date"], function (dom, domAttr, xhr, ref, windowUtils, date) {
        try {
            var cpfPessoaRelc = dojo.byId("cdPessoaCpf").value;
            cpfPessoaRelc = cpfPessoaRelc == 0 ? null : cpfPessoaRelc;
            apresentaMensagem('apresentadorMensagemPessoa', '');

            if (!validateCadPessoaFK(windowUtils))
                return false;
            if (!hasValue(cpfPessoaRelc) && dojo.byId("parametroEscolaInternacional").value == 0)
                if (!validarCPF("#cpf", "apresentadorMensagemPessoa"))
                    return false;
            var pessoaFisicaUI = montarDadosPessoaFisica(date, 'apresentadorMensagemPessoa');
            if (!pessoaFisicaUI) return false;
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/secretaria/postInsertPessoaFisica",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(pessoaFisicaUI)
            }).then(function (data) {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridPessoa';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    insertObjSort(grid.itensSelecionados, "cd_pessoa", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_pessoa', 'selecionaTodos', ['pesquisarPessoa', 'relatorioPessoa'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_pessoa");
                    dijit.byId("formulario").hide();
                }
                else
                    apresentaMensagem('apresentadorMensagemPessoa', data);
                showCarregando();
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemPessoa', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function alterarPessoaFisica() {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref", "dojo/window", "dojo/date"], function (dom, domAttr, xhr, ref, windowUtils, date) {
        try {
            var cpfPessoaRelc = dojo.byId("cdPessoaCpf").value;
            cpfPessoaRelc = cpfPessoaRelc == 0 ? null : cpfPessoaRelc;
            apresentaMensagem('apresentadorMensagemPessoa', '');

            if (!validateCadPessoaFK(windowUtils))
                return false;
            if (!hasValue(cpfPessoaRelc) && dojo.byId("parametroEscolaInternacional").value == 0)
                if (!validarCPF("#cpf", "apresentadorMensagemPessoa"))
                    return false;
            var pessoaFisicaUI = montarDadosPessoaFisica(date, 'apresentadorMensagemPessoa');
            if (!pessoaFisicaUI) return false;
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/secretaria/postUpdatePessoaFisica",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(pessoaFisicaUI)
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridPessoa';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];
                        removeObjSort(grid.itensSelecionados, "cd_pessoa", dom.byId("cd_pessoa").value);
                        insertObjSort(grid.itensSelecionados, "cd_pessoa", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_pessoa', 'selecionaTodos', ['pesquisarPessoa', 'relatorioPessoa'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_pessoa");
                        dijit.byId("formulario").hide();
                    } else
                        apresentaMensagem('apresentadorMensagemPessoa', data);
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemPessoa', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function alterarPessoajuridica() {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref", "dojo/window"], function (dom, domAttr, xhr, ref, windowUtils) {
        try {
            var relacionamentos = null;
            var descFoto = null;

            if (!validateCadPessoaFK(windowUtils))
                return false;
            if (!validarCnpj("#cnpj", "apresentadorMensagemPessoa"))
                return false;
            var outrosEnderecos = null;
            var outrosContatos = null;

            if (hasValue(dijit.byId("gridRelacionamentos")))
                relacionamentos = montarRelacionamentos();
            if (hasValue(dijit.byId("gridEnderecos")) && hasValue(dijit.byId("gridEnderecos").store.objectStore) && hasValue(dijit.byId("gridEnderecos").store.objectStore.data))
                outrosEnderecos = dijit.byId("gridEnderecos").store.objectStore.data;
            if (hasValue(dijit.byId("gridContatos")) && hasValue(dijit.byId("gridContatos").store.objectStore) && hasValue(dijit.byId("gridContatos").store.objectStore.data))
                outrosContatos = dijit.byId("gridContatos").store.objectStore.data;
            if (hasValue(dijit.byId("uploader")) && hasValue(dijit.byId("uploader")._files)) {
                descFoto = getNameFoto(dijit.byId("uploader")._files[0]);
            }
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/secretaria/postUpdatePessoaJuridica",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(montarDadosPessoaJuridica(dom, domAttr))
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridPessoa';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        removeObjSort(grid.itensSelecionados, "cd_pessoa", dom.byId("cd_pessoa").value);
                        insertObjSort(grid.itensSelecionados, "cd_pessoa", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_pessoa', 'selecionaTodos', ['pesquisarPessoa', 'relatorioPessoa'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_pessoa");
                        dijit.byId("formulario").hide();
                    } else
                        apresentaMensagem('apresentadorMensagemPessoa', data);
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemPessoa', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridPessoa';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionadoPessoa', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getNameFoto(files) {
    try {
        if (hasValue(files.name))
            return files.name;
        return null;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function getParametroEscolaInternacional() {
    dojo.xhr.get({
        url: Endereco() + "/api/aluno/getParametroEscolaInternacional",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
            if (hasValue(data) && data.escolaInternacional == true) {
                dijit.byId("cpf").set("required", false);
                dojo.byId("parametroEscolaInternacional").value = 1;
            } else {
                dijit.byId("cpf").set("required", true);
                dojo.byId("parametroEscolaInternacional").value = 0;
            }

        },
        function (error) {
            apresentaMensagem('apresentadorMensagemAluno', error);
        });
}
