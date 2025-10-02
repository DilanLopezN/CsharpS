var TITULO_ABERTO = 1, TITULO_FECHADO = 2;
function montarMetodosFaturamento(permissoes) {
    //Criação da Grade de sala
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dojox/json/ref",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dijit/form/FilteringSelect",
    "dojo/dom"
    ], function (ready, xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, on, Button, ref, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, dom) {
        ready(function () {
            try {
                if (hasValue(permissoes))
                    document.getElementById("setValuePermissoes").value = permissoes;
                dojo.byId("footer").style.height = '0px';


                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes)) {
                    var myStore =
                                 Cache(
                                         JsonRest({
                                             target: Endereco() + "/api/financeiro/getTituloSearchFaturamentoGeral?cd_pessoa=" + parseInt(0) + "&responsavel=false&inicio=false" +
                                                                  "&numeroTitulo=&parcelaTitulo=&valorTitulo=&dtInicial=&dtFinal=&tipoTitulo=0",
                                             handleAs: "json",
                                             preventCache: true,
                                             headers: { "Accept": "application/json", "Authorization": Token() }
                                         }), Memory({}));
                }
                else {
                    var myStore =
                                 Cache(
                                         JsonRest({
                                             target: Endereco() + "/api/financeiro/getTituloSearchFaturamento?cd_pessoa=" + parseInt(0) + "&responsavel=false&inicio=false" +
                                                                  "&numeroTitulo=&parcelaTitulo=&valorTitulo=&dtInicial=&dtFinal=&tipoTitulo=0",
                                             handleAs: "json",
                                             preventCache: true,
                                             headers: { "Accept": "application/json", "Authorization": Token() }
                                         }), Memory({}));
                }


                var gridTituloFat = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    structure: [
                        {
                            name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;",
                            formatter: formatCheckBoxTitulo
                        },
                        { name: "Número", field: "nm_titulo", width: "8%", styles: "min-width:80px;" },
                        { name: "Parc.", field: "nm_parcela_titulo", width: "5%", styles: "min-width:80px; text-align: center;" },
                        { name: "Tipo", field: "dc_tipo_titulo", width: "4%", styles: "text-align: center;" },
                        { name: "Emissão", field: "dt_emissao", width: "8%", styles: "min-width:80px; text-align: center;" },
                        { name: "Vencimento", field: "dt_vcto", width: "8%", styles: "min-width:80px; text-align: center;" },
                        { name: "Valor", field: "vlTitulo", width: "8%", styles: "min-width:80px;text-align: right;" },
                        { name: "Saldo", field: "vlSaldoTitulo", width: "8%", styles: "text-align: right;" },
                        { name: "Cliente", field: "nomeResponsavel", width: "31%", styles: "text-align: left;" },
                        { name: "Status", field: "statusTitulo", width: "7%", styles: "text-align: center;" }
                    ],
                    noDataMessage: msgNotRegEnc,
                    canSort: true,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "32", "64", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridTituloFat");
                gridTituloFat.startup();
                gridTituloFat.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        gridTituloFat.itemSelecionado = item;
                        destroyCreateHistoricoTitulo();
                        setarTabCadTitulo();
                        showEditTitulo(gridTituloFat.itemSelecionado.cd_titulo, xhr, ready, Memory, FilteringSelect);
                        dijit.byId("cadTitulo").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridTituloFat.itenSelecionado = null;
                gridTituloFat.pagination.plugin._paginator.plugin.connect(gridTituloFat.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    try{
                        verificaMostrarTodos(evt, gridTituloFat, 'cd_titulo', 'selecionaTodos');
                    }catch (e) {
                        postGerarLog(e);
                    }
                });

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridTituloFat, "_onFetchComplete", function () {
                        try {
                            // Configura o check de todos:
                            if (hasValue(dojo.byId('selecionaTodos')) && dojo.byId('selecionaTodos').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_titulo', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridTituloFat')", gridTituloFat.rowsPerPage * 3);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    });
                });
                gridTituloFat.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9  };

                // Corrige o tamanho do pane que o dojo cria para o dialog com scroll no ie7:
                if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
                    var ieversion = new Number(RegExp.$1)
                    if (ieversion == 7)
                        // Para IE7
                        dojo.byId('cadTitulo').childNodes[1].style.height = '100%';
                }
                

                var statusStore = new Memory({
                    data: [
                      { name: "Todos", id: "0" },
                      { name: "Abertos", id: "1" },
                      { name: "Fechados", id: "2" }
                    ]
                });
                // Botões C.R.U.D título.

                var storeNaturezaCadTitulo = new Memory({
                    data: [
                      { name: "Receber", id: "1" },
                      { name: "Pagar", id: "2" }
                    ]
                });

                dijit.byId("cbNaturezaTitulo").store = storeNaturezaCadTitulo;

                var statusTitulo = new Memory({
                    data: [
                      { name: "Aberto", id: "1" },
                      { name: "Fechado", id: "2" }
                    ]
                });
                dijit.byId("cbStatusTitulo").store = statusTitulo;

                var statusStoreTipoTitulo = new Memory({
                    data: [
                    { name: "Todos", id: 0 },
                    { name: "PP", id: 1 },
                    { name: "TM", id: 2 },
                    { name: "TA", id: 3 },
                    { name: "ME", id: 4 },
                    { name: "MA", id: 5 },
                    { name: "AD", id: 6 },
                    { name: "AA", id: 7 },
                    { name: "MM", id: 8 }
                    ]
                });
                dijit.byId("pesqTipoTituloFat").store = statusStoreTipoTitulo;
                dijit.byId("pesqTipoTituloFat").set("value", 0);

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                    }
                }, "pesCadPessoaTitulo");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); }, 'apresentadorMensagemTitulo', BAIXA_FINANCEIRA);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, BAIXA_FINANCEIRA);
                                clearForm("formPlanoContasFK");
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesPlanoTitulo");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        try {
                            limparCadTitulos();
                            setarTabCadTitulo();
                            showEditTitulo(gridTituloFat.itensSelecionados[0].cd_titulo, xhr, ready, Memory, FilteringSelect);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarTitulo");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadTitulo").hide(); } }, "fecharTitulo");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        updateTitulo(xhr);
                    }
                }, "alterarTitulo");

                new Button({
                    label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        pesquisarTitulo(true);
                    }
                }, "pesquisaTitulosFat");
                decreaseBtn(document.getElementById("pesquisaTitulosFat"), '32px');

                new Button({
                    label: "Limpar", iconClass: '', disabled: true, onClick: function () {
                        try {
                            dojo.byId('cdPessoaFaturamento').value = 0;
                            dojo.byId("pessoaFaturamento").value = "";
                            dijit.byId('limparPessoaFaturamento').set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparPessoaFaturamento");
                if (hasValue(document.getElementById("limparPessoaFaturamento"))) {
                    document.getElementById("limparPessoaFaturamento").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPessoaFaturamento").parentNode.style.width = '40px';
                }


                //Metodo para a criação do dropDown de link
                //					if (dojo.byId('selecionaTodos').type == 'text')
                //						setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridDiario')", gridDiario.rowsPerPage * 3);

                // Ações Relacionadas:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarTitulo(gridTituloFat.itensSelecionados, xhr, ready, Memory, FilteringSelect); }
                });
                menu.addChild(acaoEditar);
                var acaoEditar = new MenuItem({
                    label: "NF de Serviço",
                    onClick: function () { eventoEmitirNF(gridTituloFat.itensSelecionados) }
                });
                menu.addChild(acaoEditar);               

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                // Itens selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () { buscarTodosItens(gridTituloFat, 'todosItens', ['pesquisaTitulosFat']); pesquisarTitulo(false); }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridTituloFat', 'selecionado', 'cd_titulo', 'selecionaTodos', ['pesquisaTitulosFat'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    abrirPessoaFK();
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisaPessoaFKTitulo();
                                    });
                                });
                            else
                                abrirPessoaFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesProPessoaFKPesqFaturamento");

                var buttonFkArray = ['pesCadPessoaTitulo', 'pesPlanoTitulo', 'pesProPessoaFKPesqFaturamento'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }
                dijit.byId("tgTituloFat").on("show", function (e) {
                    try {
                        gridTituloFat.update();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323052', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['numeroTituloFat', 'parcelaTituloFat', 'valorPesqFat', 'dtInicialFat', 'dtFinalFat'], 'pesquisaTitulosFat', ready);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function selecionaTabTituloFat(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];
        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        if (tab.getAttribute('widgetId') == 'tabContainerTituloFat_tablist_tabHistoricoFat' && !hasValue(dijit.byId("gridHistoricoTitulo"))) {
            var cd_titulo = dojo.byId("cd_titulo").value;
            montarGridHistoricoTitulo(parseInt(cd_titulo), dojo.xhr);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarGridHistoricoTitulo(cd_titulo, xhr) {
    xhr.get({
        url: Endereco() + "/api/escola/getLogGeralTitulo?cd_titulo=" + cd_titulo,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            apresentaMensagem("apresentadorMensagemTitulo", null);
            data = jQuery.parseJSON(data).retorno;
            if (!hasValue(data) || data.length <= 0)
                data = [];
            data = clearChildrenLenthZero(data);
            var data = {
                identifier: 'id',
                label: 'descricao',
                items: data
            };

            var store = new dojo.data.ItemFileWriteStore({ data: data });

            var model = new dijit.tree.ForestStoreModel({
                store: store, childrenAttrs: ['children']
            });

            var layout = [
              { name: 'Usuário', field: 'descricao', width: '30%' },
              { name: 'Data/Hora', field: 'dta_historico', width: '20%' },
              { name: 'Vl.Antigo', field: 'dc_valor_antigo', width: '20%', styles: "text-align: center;" },
              { name: 'Vl.Novo', field: 'dc_valor_novo', width: '20%', styles: "text-align: center;" },
              { name: 'Operação', field: 'dc_tipo_log', width: '10%', styles: "text-align: center;" },
              { name: '', field: 'id', width: '0%', styles: "display: none;" }
            ];
            destroyCreateHistoricoTitulo();
            var gridHistoricoBaixaTitulo = new dojox.grid.LazyTreeGrid({
                id: 'gridHistoricoTitulo',
                treeModel: model,
                structure: layout,
                noDataMessage: msgNotRegEnc
            }, document.createElement('div'));

            dojo.byId("gridHistoricoTitulo").appendChild(gridHistoricoBaixaTitulo.domNode);
            gridHistoricoBaixaTitulo.canSort = function (col) { return false; };
            gridHistoricoBaixaTitulo.startup();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemTitulo', error);
    });
}

function setarTagTitulo(value) {
    //Criação da Grade de sala
    require([
    "dojo/ready"
    ], function (ready) {
        ready(function () {
            try {
                if (value == true) {
                    dojo.byId('tgBaixaPesq').style.height = '200px';
                    dojo.byId("gridTituloFat").style.height = '280px';
                    dijit.byId("gridTituloFat").set("height", '280px');
                    dijit.byId("gridTituloFat").attr("height", '280px');
                    //dijit.byId("gridTituloFat").currentPageSize(9);
                    dijit.byId('gridTituloFat').resize(true);
                }
                else {
                    dojo.byId('tgBaixaPesq').style.height = '27px';
                    dojo.byId("gridTituloFat").style.height = '455px';
                    dijit.byId("gridTituloFat").set("height", '455px');
                    dijit.byId("gridTituloFat").attr("height", '455px');
                    //dijit.byId("gridTituloFat").currentPageSize(16);
                    dijit.byId('gridTituloFat').resize(true);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBoxTitulo(value, rowIndex, obj) {
    try {
        var gridName = 'gridTituloFat';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_titulo", grid._by_idx[rowIndex].item.cd_titulo);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_titulo', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_titulo', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPessoaFK() {
    try {
        limparPesquisaPessoaFK();
        //dijit.byId("tipoPessoaFK").set("value", 1);
        //dijit.byId("tipoPessoaFK").set("disabled", true);
        pesquisaPessoaFKTitulo();
        dijit.byId("proPessoa").show();
        apresentaMensagem('apresentadorMensagemProPessoa', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaFKTitulo() {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready"],
    function (JsonRest, ObjectStore, Cache, Memory, ready) {
        ready(function () {
            try {
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/aluno/getPessoaTituloSearch?nome=" + dojo.byId("_nomePessoaFK").value +
                                       "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked + "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                       "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                       "&sexo=" + dijit.byId("sexoPessoaFK").value + "&responsavel=" + document.getElementById("ckResponsavelFaturamento").checked,
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));

                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoa");
                grid.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function retornarPessoa() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0 || gridPessoaSelec.itensSelecionados.length > 1) {
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            $("#cdPessoaFaturamento").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
            $("#pessoaFaturamento").val(gridPessoaSelec.itensSelecionados[0].no_pessoa);
            //apresentaMensagem(dojo.byId("descApresMsg").value, null);
            dijit.byId("limparPessoaFaturamento").set("disabled", false);
        }

        if (!valido)
            return false;
        dijit.byId("proPessoa").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarTitulo(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var gridTituloFat = dijit.byId("gridTituloFat");
            var cdResponsavel = hasValue(dojo.byId("cdPessoaFaturamento").value) ? dojo.byId("cdPessoaFaturamento").value : 0;
            var parcelaTitulo = hasValue(dijit.byId("parcelaTituloFat").value) ? dijit.byId("parcelaTituloFat").value : 0;
            var valorPesq = hasValue(dijit.byId("valorPesqFat").value) ? dijit.byId("valorPesqFat").value : "";
            var numeroTitulo = hasValue(dijit.byId("numeroTituloFat").value) ? dijit.byId("numeroTituloFat").value : 0;
            var cdTipoTitulo = hasValue(dijit.byId("pesqTipoTituloFat").value) ? dijit.byId("pesqTipoTituloFat").value : 0;
            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
            if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes)) {
                var myStore =
                             Cache(
                                     JsonRest({
                                         target: Endereco() + "/api/financeiro/getTituloSearchFaturamentoGeral?cd_pessoa=" + parseInt(cdResponsavel) + "&responsavel=" + document.getElementById("ckResponsavelFaturamento").checked +
                                                            "&inicio=" + document.getElementById("inicioVal").checked + "&numeroTitulo=" + numeroTitulo + "&parcelaTitulo=" + parcelaTitulo +
                                                            "&valorTitulo=" + valorPesq + "&dtInicial=" + dojo.byId("dtInicialFat").value + "&dtFinal=" + dojo.byId("dtFinalFat").value + "&tipoTitulo=" + cdTipoTitulo,
                                         handleAs: "json",
                                         preventCache: true,
                                         headers: { "Accept": "application/json", "Authorization": Token() }
                                     }), Memory({}));



            }
            else {
                var myStore =
                             Cache(
                                     JsonRest({
                                         target: Endereco() + "/api/financeiro/getTituloSearchFaturamento?cd_pessoa=" + parseInt(cdResponsavel) + "&responsavel=" + document.getElementById("ckResponsavelFaturamento").checked +
                                                            "&inicio=" + document.getElementById("pesqTipoTituloFat").checked + "&numeroTitulo=" + numeroTitulo + "&parcelaTitulo=" + parcelaTitulo +
                                                            "&valorTitulo=" + valorPesq + "&dtInicial=" + dojo.byId("dtInicialFat").value + "&dtFinal=" + dojo.byId("dtFinalFat").value + "&tipoTitulo=" + cdTipoTitulo,
                                         handleAs: "json",
                                         preventCache: true,
                                         headers: { "Accept": "application/json", "Authorization": Token() }
                                     }), Memory({}));
            }
            var dataStore = new ObjectStore({ objectStore: myStore });


            if (limparItens)
                gridTituloFat.itensSelecionados = [];
            gridTituloFat.itemSelecionado = null;
            gridTituloFat.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function criarGridHistorico() {
    dojo.ready(function () {
        try {
            var dataHistoricoTt = [
                {
                    nm_usuario: 'Usuário 1', dta_historico: '19/06/2014 15:24:45', dc_antigo: '', dc_novo: '', id: 1, pai: null,
                    children: [
                        { nm_usuario: 'Vencimento', dta_historico: null, dc_antigo: '10/06/2014', dc_novo: '12/06/2014', id: 2, pai: 1 },
                        { nm_usuario: 'Valor Titulo', dta_historico: null, dc_antigo: '100,00', dc_novo: '120,00', id: 3, pai: 1 },
                        { nm_usuario: 'Local Movimento', dta_historico: null, dc_antigo: 'Bradesco', dc_novo: 'Itaú', id: 4, pai: 1 }
                    ]
                },
                {
                    nm_usuario: 'Usuário 2', dta_historico: '12/04/2014 12:20:45', dc_antigo: '', dc_novo: '', id: 8, pai: null,
                    children: [
                        { nm_usuario: 'Vencimento', dta_historico: null, dc_antigo: '10/03/2014', dc_novo: '15/03/2014', id: 9, pai: 8 }
                    ]
                }
            ];

            var data = {
                identifier: 'id',
                label: 'nm_usuario',
                items: dataHistoricoTt
            };
            var store = new dojo.data.ItemFileWriteStore({ data: data });
            var model = new dijit.tree.ForestStoreModel({
                store: store, childrenAttrs: ['children']
            });

            /* set up layout */
            var layout = [
              { name: 'Usuário', field: 'nm_usuario', width: '45%' },
              { name: 'Data/Hora', field: 'dta_historico', width: '20%' },
              { name: 'Vl.Antigo', field: 'dc_antigo', width: '15%', styles: "text-align: center;" },
              { name: 'Vl.Novo', field: 'dc_novo', width: '15%', styles: "text-align: center;" },
              { name: '', field: 'id', width: '5%', styles: "display:none;" }
            ];

            var gridHistoricoTt = new dojox.grid.LazyTreeGrid({
                id: 'gridHistoricoTitulo',
                treeModel: model,
                structure: layout
            }, document.createElement('div'));
            dojo.byId("gridHistoricoTitulo").appendChild(gridHistoricoTt.domNode);
            gridHistoricoTt.startup();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function setarTabCadTitulo() {
    try {
        var tabs = dijit.byId("tabContainerTituloFat");
        var pane = dijit.byId("tabPrincipalTitulo");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarTitulo(itensSelecionados, xhr, ready, Memory, FilteringSelect) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            destroyCreateHistoricoTitulo();
            setarTabCadTitulo();
            showEditTitulo(itensSelecionados[0].cd_titulo, xhr, ready, Memory, FilteringSelect);
            dijit.byId("cadTitulo").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function showEditTitulo(cd_titulo, xhr, ready, Memory, FilteringSelect) {
    try {
        showCarregando();
        xhr.get({
            url: Endereco() + "/api/financeiro/getTituloBaixaFinan?cd_titulo=" + cd_titulo,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                var existBaixa = false;
                data = jQuery.parseJSON(data).retorno;
                apresentaMensagem("apresentadorMensagemTitulo", null);
                dijit.byId('tabContainerTituloFat').resize();
                dojo.byId('tabContainerTituloFat_tablist').children[3].children[0].style.width = '100%';
                if ((data.id_status_titulo == TITULO_FECHADO) || (data.vl_titulo != data.vl_saldo_titulo)) {
                    if (hasValue(data.bancos) && data.bancos.length > 0)
                        criarOuCarregarCompFiltering("cbLocalMovtoTitulo", data.bancos, "", data.cd_local_movto, ready, Memory, FilteringSelect, 'cd_local_movto', 'nomeLocal');
                    if (hasValue(data.cd_tipo_financeiro) && data.cd_tipo_financeiro > 0)
                        criarOuCarregarCompFiltering("cbTipoFinan", [{ id: data.cd_tipo_financeiro, name: data.tipoDoc }], "", data.cd_tipo_financeiro, ready, Memory, FilteringSelect, 'id', 'name');
                } else {
                    //if ((data.id_natureza_titulo == RECEBER || data.id_natureza_titulo == PAGAR) && data.id_status_titulo == TITULO_ABERTO) {
                    if (hasValue(data.bancos) && data.bancos.length > 0)
                        criarOuCarregarCompFiltering("cbLocalMovtoTitulo", data.bancos, "", data.cd_local_movto, ready, Memory, FilteringSelect, 'cd_local_movto', 'nomeLocal');
                    if (hasValue(data.cd_tipo_financeiro) && data.cd_tipo_financeiro > 0)
                        criarOuCarregarCompFiltering("cbTipoFinan", [{ id: data.cd_tipo_financeiro, name: data.tipoDoc }], "", data.cd_tipo_financeiro, ready, Memory, FilteringSelect, 'id', 'name');
                }
                loadDataTitulio(data);
                if (data.vl_titulo != data.vl_saldo_titulo)
                    existBaixa = true;
                configLayoutCadTitulo(data.id_status_titulo, data.id_natureza_titulo, existBaixa);
                var cd_titulo = dojo.byId("cd_titulo").value;
                montarGridHistoricoTitulo(parseInt(cd_titulo), dojo.xhr);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemTitulo", error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataTitulio(titulo) {
    try {
        dojo.byId("cd_titulo").value = titulo.cd_titulo;
        dojo.byId("cdPessoaTitulo").value = titulo.cd_pessoa_titulo;
        dojo.byId("noPessoaTitulo").value = titulo.nomeResponsavel;
        dijit.byId("cadNumeroTitulo").set("value", titulo.nm_titulo);
        dijit.byId("cadParcelaTitulo").set("value", titulo.nm_parcela_titulo);
        dijit.byId("dtaEmissaoTitulo").set("value", titulo.dt_emissao_titulo);
        dijit.byId("dtaVencTitulo").set("value", titulo.dt_vcto_titulo);
        //cd_local_movto = x.cd_local_movto,
        dijit.byId("cadValorTitulo").set("value", titulo.vl_titulo);
        dijit.byId("cadJurosPercTitulo").set("value", titulo.pc_juros_titulo);
        dijit.byId("cadMultaPercTitulo").set("value", titulo.pc_multa_titulo);
        // tag valores
        dijit.byId("cadJurosTitulo").set("value", titulo.vl_juros_titulo);
        dijit.byId("cadMultaTitulo").set("value", titulo.vl_multa_titulo);
        dijit.byId("cadDescontoTitulo").set("value", titulo.vl_desconto_titulo);
        dijit.byId("cadSaldoTitulo").set("value", titulo.vl_saldo_titulo);
        //tag valores liquidação
        dijit.byId("dtBaixa").set("value", titulo.dt_liquidacao_titulo);
        dijit.byId("valorBaixa").set("value", titulo.vl_liquidacao_titulo);
        dijit.byId("idDescJuros").set("value", titulo.vl_desconto_juros);
        dijit.byId("idDescMulta").set("value", titulo.vl_desconto_multa);
        dijit.byId("idJurosBaixa").set("value", titulo.vl_juros_liquidado);
        dijit.byId("idMultaBaixa").set("value", titulo.vl_multa_liquidada);
        dijit.byId("dtCadastroTitulo").set("value", titulo.dh_cadastro_titulo);
        if (hasValue(titulo.hr_cadastro_titulo))
            dijit.byId("horaCadastroTitulo").set("value", "T" + titulo.hr_cadastro_titulo);

        //natureza
        dijit.byId("cbNaturezaTitulo").set("value", titulo.id_natureza_titulo);
        //Status titulo
        dijit.byId("cbStatusTitulo").set("value", titulo.id_status_titulo);

        //Plano de contas
        // MMC Melhoria 2705 - if (hasValue(titulo.cd_plano_conta_tit)) {
        //    dojo.byId('cdPlanoContasTitulo').value = titulo.dc_plano_conta;
        //    dojo.byId('cd_plano_contas_titulo').value = titulo.cd_plano_conta_tit;
        //}
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configLayoutCadTitulo(status, natureza, existBaixa) {
    try {
        if (status == TITULO_FECHADO || existBaixa) {
            dijit.byId("pesCadPessoaTitulo").set("disabled", true);
            dijit.byId("cbTipoFinan").set("disabled", true);
            dijit.byId("dtaEmissaoTitulo").set("disabled", true);
            dijit.byId("dtaVencTitulo").set("disabled", true);
            dijit.byId("cbLocalMovtoTitulo").set("disabled", true);
            dijit.byId("cadValorTitulo").set("disabled", true);
            dijit.byId("cadJurosPercTitulo").set("disabled", true);
            dijit.byId("cadMultaPercTitulo").set("disabled", true);
            dijit.byId("pesPlanoTitulo").set("disabled", true);
            dijit.byId("dtCadastroTitulo").set("disabled", true);
            dijit.byId("horaCadastroTitulo").set("disabled", true);

            dijit.byId("alterarTitulo").set("disabled", true);
            dijit.byId("cancelarTitulo").set("disabled", true);
        } else {
            //if (natureza == RECEBER && status == TITULO_ABERTO) {
            dijit.byId("pesCadPessoaTitulo").set("disabled", true);
            dijit.byId("cbTipoFinan").set("disabled", true);
            dijit.byId("dtaEmissaoTitulo").set("disabled", true);
            dijit.byId("dtaVencTitulo").set("disabled", false);
            dijit.byId("cbLocalMovtoTitulo").set("disabled", false);
            dijit.byId("cadValorTitulo").set("disabled", true);
            dijit.byId("cadJurosPercTitulo").set("disabled", false);
            dijit.byId("cadMultaPercTitulo").set("disabled", false);
            dijit.byId("pesPlanoTitulo").set("disabled", false);
            dijit.byId("dtCadastroTitulo").set("disabled", true);
            dijit.byId("horaCadastroTitulo").set("disabled", true);

            dijit.byId("alterarTitulo").set("disabled", false);
            dijit.byId("cancelarTitulo").set("disabled", false);
        }
        //if (natureza == PAGAR == status == TITULO_ABERTO) {
        //    dijit.byId("pesCadPessoaTitulo").set("disabled", false);
        //    dijit.byId("cbTipoFinan").set("disabled", false);
        //    dijit.byId("dtaEmissaoTitulo").set("disabled", false);
        //    dijit.byId("dtaVencTitulo").set("disabled", false);
        //    dijit.byId("cbLocalMovtoTitulo").set("disabled", false);
        //    dijit.byId("cadValorTitulo").set("disabled", false);
        //    dijit.byId("cadJurosPercTitulo").set("disabled", false);
        //    dijit.byId("cadMultaPercTitulo").set("disabled", false);
        //    dijit.byId("pesPlanoTitulo").set("disabled", false);
        //    dijit.byId("dtCadastroTitulo").set("disabled", false);
        //    dijit.byId("horaCadastroTitulo").set("disabled", false);
        //}
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparCadTitulos() {
    try {
        clearForm("formTituloPrincipal");
        dojo.byId("cd_titulo").value = 0;
        dojo.byId("cdPessoaTitulo").value = 0;
        dojo.byId("cd_plano_contas_titulo").value = 0;
        dijit.byId("cadNumeroTitulo").reset();
        dijit.byId("cadParcelaTitulo").reset();
        dijit.byId("dtaEmissaoTitulo").reset();
        dijit.byId("dtaVencTitulo").reset();
        //cd_local_movto = x.cd_local_movto,
        dijit.byId("cadValorTitulo").reset();
        dijit.byId("cadJurosPercTitulo").reset();
        dijit.byId("cadMultaPercTitulo").reset();
        // tag valores
        dijit.byId("cadJurosTitulo").reset();
        dijit.byId("cadMultaTitulo").reset();
        dijit.byId("cadDescontoTitulo").reset();
        dijit.byId("cadSaldoTitulo").reset();
        //tag valores liquidação
        dijit.byId("dtBaixa").reset();
        dijit.byId("valorBaixa").reset();
        dijit.byId("idDescJuros").reset();
        dijit.byId("idDescMulta").reset();
        dijit.byId("idJurosBaixa").reset();
        dijit.byId("idMultaBaixa").reset();
        dijit.byId("dtCadastroTitulo").reset();
        dijit.byId("horaCadastroTitulo").reset();
        //natureza
        dijit.byId("cbNaturezaTitulo").reset();
        //Status titulo
        dijit.byId("cbStatusTitulo").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region Metodo C.R.U.D titulo
function updateTitulo(xhr) {
    try {
        if (!dijit.byId("formTituloPrincipal").validate()) {
            setarTabCadTitulo();
            return false;
        }

        showCarregando();
        xhr.post({
            url: Endereco() + "/api/financeiro/postUpdateTituloBaixaFinan",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify({
                cd_titulo: dojo.byId("cd_titulo").value,
                dt_vcto_titulo: dojo.date.locale.parse(dojo.byId("dtaVencTitulo").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                //** MMC - melhoria 2705 - cd_plano_conta_tit: dojo.byId('cd_plano_contas_titulo').value,
                cd_local_movto: dijit.byId("cbLocalMovtoTitulo").get("value"),
                pc_juros_titulo: dijit.byId("cadJurosPercTitulo").value,
                pc_multa_titulo: dijit.byId("cadMultaPercTitulo").value
            })
        }).then(function (data) {
            try {
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridTituloFat';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    removeObjSort(grid.itensSelecionados, "cd_titulo", itemAlterado.cd_titulo);
                    insertObjSort(grid.itensSelecionados, "cd_titulo", itemAlterado, false);
                    buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_titulos', 'selecionaTodos', ['pesquisaTitulosFat'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_titulo");
                    showCarregando();
                    dijit.byId("cadTitulo").hide();
                } else {
                    apresentaMensagem('apresentadorMensagemTitulo', data);
                    showCarregando();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemTitulo', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clearChildrenLenthZero(dataRetorno) {
    try {
        for (var i = 0; i < dataRetorno.length; i++)
            if (dataRetorno[i].children.length > 0)
                for (var j = 0; j < dataRetorno[i].children.length; j++) {
                    if (dataRetorno[i].children[j].children != null && dataRetorno[i].children[j].children.length > 0) {
                        for (var m = 0; m < dataRetorno[i].children[j].children.length; m++)
                            delete dataRetorno[i].children[j].children[m].children;
                    } else delete dataRetorno[i].children[j].children;
                }
        return dataRetorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateHistoricoBaixaTitulo() {
    try {
        if (hasValue(dijit.byId("gridHistoricoBaixaTitulo"))) {
            dijit.byId("gridHistoricoBaixaTitulo").destroy();
            //$('<div>').attr('id', 'gridHistoricoBaixaTitulo').attr('style', 'height:100%;').attr('style', 'min-height:200px;').appendTo('#paiGridHistBaixaTitulo');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateHistoricoTitulo() {
    try {
        if (hasValue(dijit.byId("gridHistoricoTitulo"))) {
            dijit.byId("gridHistoricoTitulo").destroy();
            //$('<div>').attr('id', 'gridHistoricoBaixaTitulo').attr('style', 'height:100%;').attr('style', 'min-height:200px;').appendTo('#paiGridHistBaixaTitulo');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


function eventoEmitirNF(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else {
            caixaDialogo(DIALOGO_CONFIRMAR, msgConfirmGeracao, function () { processarMovimento(itensSelecionados); });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function processarMovimento(itensSelecionados) {
    var cd_movimento = itensSelecionados[0].cd_movimento;
    var id_tipo_movimento = itensSelecionados[0].id_tipo_movimento;
    dojo.xhr.post({
        url: Endereco() + "/api/escola/postGerarNFFaturamento",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: dojox.json.ref.toJson(itensSelecionados)
    }).then(function (data) {
        try {
            if (hasValue(data.erro))
                apresentaMensagem('apresentadorMensagem', data);
            else {
                pesquisarTitulo(true);
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgNFGerada);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                window.open(EnderecoRelatorioWeb() + data.retorno + "&enderecoWeb=" + EnderecoAbsoluto());
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}