var TITULOFK = 2;
function formatCheckBoxTituloFK(value, rowIndex, obj) {
    try{
        var gridName = 'gridPesquisaTituloFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTituloFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_titulo", grid._by_idx[rowIndex].item.cd_titulo);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_titulo', 'selecionadoTituloFK', -1, 'selecionaTodosTituloFK', 'selecionaTodosTituloFK', '" + gridName + "')", grid.rowsPerPage * 18);

        setTimeout("configuraCheckBox(" + value + ", 'cd_titulo', 'selecionadoTituloFK', " + rowIndex + ", '" + id + "', 'selecionaTodosTituloFK', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montarGridPesquisaTitulo(especializada, funcao, permissoes) {
    require([
       "dojo/_base/xhr",
       "dojox/grid/EnhancedGrid",
       "dojox/grid/enhanced/plugins/Pagination",
       "dojo/store/JsonRest",
       "dojo/data/ObjectStore",
       "dojo/store/Cache",
       "dojo/store/Memory",
       "dijit/form/Button",
       "dojo/ready",
       "dojo/on"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, on) {
        ready(function () {
            try{
                var myStore = null;
                var store = null;
                returnLocalMovimento();
                if (!especializada) {
                    var myStore =
                            Cache(JsonRest({
                                target: Endereco() + "/api/financeiro/getTituloSearchGeral?cd_pessoa=" + parseInt(0) + "&responsavel=false&inicio=false&locMov=" + parseInt(0) +
                                                     "&natureza=" + parseInt(1) + "&status=" + parseInt(1) + "&numeroTitulo=&parcelaTitulo=&valorTitulo=&dtInicial=&dtFinal=" +
                                                     "&emissao=true&baixa=false&vencimento=false&locMovBaixa=0&cdTipoLiquidacao=0&tipoTitulo=0&nossoNumero=0",
                                handleAs: "json",
                                preventCache: true,
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }), Memory({}));
                    store = new ObjectStore({ objectStore: myStore });
                } else
                    store = new ObjectStore({ objectStore: new Memory({ data: null }) });

                //*** Cria a grade de Titulo **\\
                var gridPesquisaTituloFK = new EnhancedGrid({
                    store: store,
                    structure:
                            [
                                { name: "<input id='selecionaTodosTituloFK' style='display:none'/>", field: "selecionadoTituloFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTituloFK },
                                { name: "Número", field: "nm_titulo", width: "8%", styles: "min-width:80px;" },
                                { name: "Parc.", field: "nm_parcela_titulo", width: "5%", styles: "min-width:80px;text-align: center;" },
                                { name: "Emissão", field: "dt_emissao", width: "10%", styles: "min-width:80px;" },
                                { name: "Vencimento", field: "dt_vcto", width: "10%", styles: "min-width:80px;" },
                                { name: "Valor", field: "vlTitulo", width: "8%", styles: "min-width:80px;text-align: right;" },
                                { name: "Cliente", field: "nomeResponsavel", width: "30%", styles: "text-align: left;"}
                            ],
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["15", "30", "45", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "15",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridPesquisaTituloFK");

                gridPesquisaTituloFK.canSort = function (col) { return Math.abs(col) != 1 };
                gridPesquisaTituloFK.pagination.plugin._paginator.plugin.connect(gridPesquisaTituloFK.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridPesquisaTituloFK, 'cd_titulo', 'selecionaTodosTituloFK');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPesquisaTituloFK, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosTituloFK').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_titulo', 'selecionadoTituloFK', -1, 'selecionaTodosTituloFK', 'selecionaTodosTituloFK', 'gridPesquisaTituloFK')", gridPesquisaTituloFK.rowsPerPage * 3);
                    });
                });
                gridPesquisaTituloFK.startup();
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    dojo.byId("selecionaRespFKCnab").value = TITULOFK;
                                    abrirPessoaFK();
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisaPessoaFKTitulo();
                                    });
                                });
                            else {
                                dojo.byId("selecionaRespFKCnab").value = TITULOFK;
                                abrirPessoaFK();
                            }
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesProPessoaFKTituloFK");

                decreaseBtn(document.getElementById("pesProPessoaFKTituloFK"), '18px');

                new Button({
                    label: "Limpar", iconClass: '', disabled: true, onClick: function () {
                        try{
                            dojo.byId('cdPessoaRespFK').value = 0;
                            dojo.byId("pessoaTituloFK").value = "";
                            dijit.byId('limparPessoaTituloFK').set("disabled", true);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparPessoaTituloFK");
                if (hasValue(document.getElementById("limparPessoaTituloFK"))) {
                    document.getElementById("limparPessoaTituloFK").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPessoaTituloFK").parentNode.style.width = '40px';
                }
                new Button({
                    label: "",
                    onClick: function () { apresentaMensagem('apresentadorMensagem', null); if (!especializada) pesquisarTituloFK(); },
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "pesquisarTituloFK");
                decreaseBtn(document.getElementById("pesquisarTituloFK"), '32px');
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try{
                            if (hasValue(retornarTituloFK)) {
                                retornarTituloFK(permissoes);
                            }
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaTituloFK");
            
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { }
                }, "fecharTituloFK");
                adicionarAtalhoPesquisa(['localMovtoPesq', 'numeroTitulo', 'parcelaTitulo', 'dtInicial', 'dtFinal'], 'pesquisarTituloFK', ready);
                if(hasValue(funcao))
                    funcao.call();
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}
//#endregion

function pesquisarTituloFK() {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var localMovtoPesq = dijit.byId("localMovtoPesq").value;
            var cdLocMovto = hasValue(localMovtoPesq) ? localMovtoPesq : 0;
            var cdResponsavel = hasValue(dojo.byId("cdPessoaRespFK").value) ? dojo.byId("cdPessoaRespFK").value : 0;
        
            var myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/financeiro/getTituloSearchGeral?cd_pessoa=" + cdResponsavel + "&responsavel=" + dijit.byId("ckResponsavelPesq").checked + "&inicio=false&locMov=" + cdLocMovto +
                                                                  "&natureza=" + parseInt(1) + "&status=" + parseInt(1) + "&numeroTitulo=" + dojo.byId("numeroTitulo").value + "&parcelaTitulo=" + dojo.byId("parcelaTitulo").value + "&valorTitulo=&dtInicial=" + dojo.byId("dtInicial").value +
                                                                  "&dtFinal=" + dojo.byId("dtFinal").value + "&emissao=false&vencimento=true&baixa=false&locMovBaixa=0&cdTipoLiquidacao=0&tipoTitulo=0&nossoNumero=0",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_Titulo"
            }), Memory({ idProperty: "cd_Titulo" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridTitulo = dijit.byId("gridPesquisaTituloFK");
            gridTitulo.itensSelecionados = [];
            gridTitulo.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function limparPesquisaTituloFK() {
    try{
        getLimpar('#formTituloFK');
        dojo.byId('cdPessoaRespFK').value = 0;
        dojo.byId("pessoaTituloFK").value = "";
        dijit.byId('limparPessoaTituloFK').set("disabled", true);
        if (hasValue(dijit.byId("gridPesquisaTituloFK")) && hasValue(dijit.byId("gridPesquisaTituloFK").itensSelecionados))
            dijit.byId("gridPesquisaTituloFK").itensSelecionados = [];
    } catch (e) {
        postGerarLog(e);
    }
}


function returnLocalMovimento() {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getLocalMovtoTituloRec",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                loadLocalMovimento(data.retorno);
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemCurso', error);
        });
    });
}

function loadLocalMovimento(items) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbMotivo = dijit.byId('localMovtoPesq');
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_local_movto, name: value.nomeLocal });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbMotivo.store = stateStore;
        } catch (e) {
            postGerarLog(e);
        }
    });
}