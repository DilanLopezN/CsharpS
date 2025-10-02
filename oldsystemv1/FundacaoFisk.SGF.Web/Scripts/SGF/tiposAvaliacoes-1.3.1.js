var HAS_ATIVO = 0;
var HAS_AVAL_CURSO = 8, HAS_AVALIACAO_CURSO = 3;
var SOBE = 1, DESCE = 2;

//Limpa a grade

//#region Métodos auxiliares - existNotaTipoAvaliacao - eventoEditar - limparTipoAvaliacao - EnviarDadosTipoAvaliacao -  atualiza(i)

function enableOrDisableButton(grid) {
    try{
        if (grid == null || grid.store.objectStore.data.length == 0) {
            document.getElementById('divAvaliacaoCima').style.display = "none";
            document.getElementById('divAvaliacaoBaixo').style.display = "none";
            document.getElementById('linhaAcoesUM').style.display = "none";
            document.getElementById('linhaAcoesDois').style.display = "none";
        }
        else {
            document.getElementById('divAvaliacaoCima').style.display = "";
            document.getElementById('divAvaliacaoBaixo').style.display = "";
            document.getElementById('linhaAcoesUM').style.display = "";
            document.getElementById('linhaAcoesDois').style.display = "";
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function existNotaTipoAvaliacao() {
    try{
        var exitsValorNotaTipoAval = dijit.byId('vl_total_nota').value;
        exitsValorNotaTipoAval = isNaN(exitsValorNotaTipoAval) ? 0 : exitsValorNotaTipoAval;
        if (exitsValorNotaTipoAval == null || exitsValorNotaTipoAval <= 0)
            return false;
        else return true;
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditar(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            showCarregando();
            getLimpar('#formTipoAvaliacao');
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(null, dijit.byId('gridTipoAvaliacao'), true);
            dijit.byId("cadTipoAvaliacao").show();
            IncluirAlterar(0, 'divAlterarTipoAvaliacao', 'divIncluirTipoAvaliacao', 'divExcluirTipoAvaliacao', 'apresentadorMensagemTipoAvaliacao', 'divCancelarTipoAvaliacao', 'divLimparTipoAvaliacao');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function loadProdutoPes() {
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
       function (xhr, Memory, Array) {
           xhr.get({
               url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_AVAL_CURSO + "&cd_produto=",
               preventCache: true,
               handleAs: "json",
               headers: { "Accept": "application/json", "Authorization": Token() }
           }).then(function (dataProduto) {
               try{
                   var dataProd = jQuery.parseJSON(dataProduto).retorno;
                   var items = [];
                   items.push({ id: 0, name: "Todos" });
                   Array.forEach(dataProd, function (value, i) {
                       items.push({ id: value.cd_produto, name: value.no_produto });
                   });
                   var stateStore = new Memory({
                       data: items
                   });
                   dijit.byId("pesProdutoAval").store = stateStore;
                   dijit.byId("pesProdutoAval").set("required", false);
                   dijit.byId("pesProdutoAval").set("value", 0);
               } catch (e) {
                   postGerarLog(e);
               }
           },
             function (error) {
                 apresentaMensagem('apresentadorMensagem', error);
             });
       })

}

function loadCursoPes() {
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
       function (xhr, Memory, Array) {
           xhr.get({
               url: Endereco() + "/api/curso/getPesCursos?hasDependente=" + HAS_AVALIACAO_CURSO + "&cd_curso=&cd_produto=",
               preventCache: true,
               handleAs: "json",
               headers: { "Accept": "application/json", "Authorization": Token() }
           }).then(function (dataCurso) {
               try{
                   var items = [];
                   items.push({ id: 0, name: "Todos" });
                   Array.forEach(dataCurso.retorno, function (value, i) {
                       items.push({ id: value.cd_curso, name: value.no_curso });
                   });
                   var stateStore = new Memory({
                       data: items
                   });
                   dijit.byId("pesCursoAval").store = stateStore;
                   dijit.byId("pesCursoAval").set("value", 0);
               } catch (e) {
                   postGerarLog(e);
               }
           },
             function (error) {
                 apresentaMensagem('apresentadorMensagem', error);
             });
       })

}
function limparTipoAvaliacao() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
    function (Memory, ObjectStore) {
        try{
            getLimpar('#formTipoAvaliacao');
            clearForm('formTipoAvaliacao');
            IncluirAlterar(1, 'divAlterarTipoAvaliacao', 'divIncluirTipoAvaliacao', 'divExcluirTipoAvaliacao', 'apresentadorMensagemTipoAvaliacao', 'divCancelarTipoAvaliacao', 'divLimparTipoAvaliacao');
            document.getElementById("cd_tipo_avaliacao").value = '';
            dijit.byId("gridCursoAval").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
            dijit.byId("gridCursoAval").itensSelecionados = [];
            dijit.byId("gridCursoAval").update();

            dijit.byId("gridOrdenacao").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
            dijit.byId("gridOrdenacao").itensSelecionados = [];
            dijit.byId("gridOrdenacao").update();
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function EnviarDadosTipoAvaliacao() {
    try{
        if (document.getElementById("divAlterarTipoAvaliacao").style.display == "")
            AlterarTipoAvaliacao();
        else
            IncluirTipoAvaliacao();
    } catch (e) {
        postGerarLog(e);
    }
}

//Marca na grade o registro inserido
function atualiza(i) {
    try{
        var grid = dijit.byId('gridOrdenacao');
        grid.getRowNode(i).id = '';
        grid.getRowNode(i).id = 'ordem_' + grid._by_idx[i].item.cd_criterio_avaliacao;
        window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
        window.location.hash = '#ordem_' + grid._by_idx[i].item.cd_criterio_avaliacao;
    } catch (e) {
        postGerarLog(e);
    }
}

function clearGrid(nameGrid) {
    var limparGrid = dijit.byId(nameGrid);
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try{
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                limparGrid.setStore(dataStore);
                limparGrid.update();
            } catch (e) {
                postGerarLog(e);
            }
        });
}

//#endregion

//#region popula dropDows

// retorna os nomes dos critérios
function populaNomesAvaliacao(field, ativo) {
    // Popula os produtos:
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getNomesCriterio",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                loadNomesAvaliacao(jQuery.parseJSON(data).retorno, field);
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    });
}

function loadNomesAvaliacao(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbTipo = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_criterio_avaliacao, name: value.dc_criterio_avaliacao });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbTipo.store = stateStore;
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function populaNomesCriterioAtivos(field) {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getAvaliacaoCriterio?cd_tipo_avaliacao=0&cd_criterio_avaliacao=0",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataCriterio) {
            try{
                loadNomesCriteriosAtivos(jQuery.parseJSON(dataCriterio).retorno, field);
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadNomesCriteriosAtivos(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbTipo = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_criterio_avaliacao, name: value.dc_criterio_avaliacao });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbTipo.store = stateStore;
        } catch (e) {
            postGerarLog(e);
        }
    });
}

//#endregion

//Pega os Antigos dados do Formulário
function keepValues(value, grid, ehLink) {
    try{
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
        dojo.byId("cd_tipo_avaliacao").value = value.cd_tipo_avaliacao;
        dojo.byId("dc_tipo_avaliacao").value = value.dc_tipo_avaliacao;
        dijit.byId("vl_total_nota").set('value', value.vl_total_nota);
        dojo.byId("id_tipo_ativo").value = value.id_tipo_ativo == true ? dijit.byId("id_tipo_ativo").set("value", true) : dijit.byId("id_tipo_ativo").set("value", false);
        marcarCursosTipo();
        popularGridAvaliacao();
    } catch (e) {
        postGerarLog(e);
    }
}

function keepValuesAvaliacao(dataReal, item) {
    try{
        if (hasValue(item)) {
            var peso = parseFloat(item.nm_peso_avaliacao).toFixed(2);
            document.getElementById("cd_avaliacao").value = item.cd_avaliacao,
            document.getElementById("nota").value = item.vl_nota;
            document.getElementById("peso").value = peso.toString().replace(".", ",");
            dijit.byId('cbNome').set("value", item.cd_criterio_avaliacao);
            dijit.byId("id_avaliacao_ativa").set("value", item.id_avaliacao_ativa);
            dojo.byId('cd_criterio_avaliacao').value = item.cd_criterio_avaliacao;
            document.getElementById("numOrdem").value = item.nm_ordem_avaliacao;
            dojo.byId('data').value = new dataRealAvaliacao(item);
            alterarOrd = 1;
            ordemOri = item.nm_ordem_avaliacao;
            document.getElementById('divCancelarAval').style.display = "";
            document.getElementById('divLimparAval').style.display = "none";
            document.getElementById('divIncluirAval').style.display = "none";
            document.getElementById('divEditarAval').style.display = "";
        }
        if (hasValue(dataReal)) {
            dijit.byId('cbNome').set('value', dataReal.cd_criterio_avaliacao);
            dojo.byId("id_avaliacao_ativa").value = dataReal.id_avaliacao_ativa;
            dojo.byId("numOrdem").value = dataReal.nm_ordem_avaliacao;
            dojo.byId("peso").value = dataReal.nm_peso_avaliacao;
            dojo.byId("nota").value = dataReal.vl_nota;
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxCursoVal(value, rowIndex, obj) {
    try{
        var gridName = 'gridCursoAval';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_curso", grid._by_idx[rowIndex].item.cd_curso);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_curso', 'selecionadoCurso', -1, 'selecionaTodosCurso', 'selecionaTodosCurso', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_curso', 'selecionadoCurso', " + rowIndex + ", '" + id + "', 'selecionaTodosCurso', '" + gridName + "')", 2);
        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBox(value, rowIndex, obj) {
    try{
        var gridName = 'gridTipoAvaliacao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_avaliacao", grid._by_idx[rowIndex].item.cd_tipo_avaliacao);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_avaliacao', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_avaliacao', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);
        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montaCadastroTipoAvaliacao() {
    require([
        "dojo/_base/xhr",
        "dojo/dom",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        ready(function () {
            try{
                montarStatus("statusTipoAvaliacao");
                $(".Dialogo").css('display', 'none');
                dijit.byId("tabPrincipalTipo").resize();
                loadProdutoPes();
                loadCursoPes();
                populaNomesAvaliacao("cbNomes", null);
                populaNomesCriterioAtivos("cbNome", null);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323020',
                                '765px',
                                '771px'); // Avaliação do Curso
                        });
                }
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/coordenacao/getTipoAvaliacaoSearch?descricao=&inicio=false&status=1&cd_tipo_avaliacao=0&cd_criterio_avaliacao=0&cdCurso=0&cdProduto=0",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                ), Memory({}));

                var gridTipoAvaliacao = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                                { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                              //  { name: "Código", field: "cd_tipo_avaliacao", width: "50px", styles: "text-align: right; min-width:60px; max-width:75px;" },
                                { name: "Descrição", field: "dc_tipo_avaliacao", width: "60%", styles: "min-width:80px;" },
                                { name: "Nota", field: "vl_total_nota", width: "25%", styles: "min-width:80px;" },
                                { name: "Ativo", field: "tipo_ativo", width: "10%", styles: "text-align:center; min-width:40px; max-width: 50px;" }
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
                            position: "button"
                        }
                    }
                }, "gridTipoAvaliacao");
                // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                gridTipoAvaliacao.pagination.plugin._paginator.plugin.connect(gridTipoAvaliacao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridTipoAvaliacao, 'cd_tipo_avaliacao', 'selecionaTodos'); });
                gridTipoAvaliacao.canSort = function (col) { return Math.abs(col) != 1; };
                gridTipoAvaliacao.startup();
                gridTipoAvaliacao.on("RowDblClick", function (evt) {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        getLimpar('#formTipoAvaliacao');
                        showCarregando();
                        apresentaMensagem('apresentadorMensagem', '');
                        dijit.byId("pesquisaCurso").reset();
                        dijit.byId("pesquisaProduto").reset();
                        keepValues(item, gridTipoAvaliacao, false);
                        dijit.byId("cadTipoAvaliacao").show();
                        IncluirAlterar(0, 'divAlterarTipoAvaliacao', 'divIncluirTipoAvaliacao', 'divExcluirTipoAvaliacao', 'apresentadorMensagemTipoAvaliacao', 'divCancelarTipoAvaliacao', 'divLimparTipoAvaliacao');
                        dijit.byId("cursos").set("open", false);
                        dijit.byId("criterios").set("open", false);
                        xhr.get({
                            url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_ATIVO + "&cd_produto=null",
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }).then(function (dataProdAtivo) {
                            xhr.get({
                                url: Endereco() + "/api/coordenacao/getAvaliacaoByIdTipoAvaliacao?idTipoAvaliacao=" + dojo.byId("cd_tipo_avaliacao").value,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                dijit.byId("gridOrdenacao").setStore(new ObjectStore({ objectStore: new Memory({ data: data.retorno }) }));
                                dijit.byId("gridOrdenacao").update();
                                var gridAvaliacaoOrdem = dijit.byId("gridOrdenacao");
                                enableOrDisableButton(gridAvaliacaoOrdem);
                                loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, 'pesquisaProduto');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagemTipoAvaliacao', error);
                            });
                        });
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                //#region gridOrdenacao
                var gridOrdenacao = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure: [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoOrdenacao", width: "5%", styles: "text-align: center;", formatter: formatCheckBoxOrdenacao },
                       // { name: "Código", field: "cd_avaliacao", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Nome", field: "dc_criterio_avaliacao", width: "55%", styles: "min-width:40px; text-align: left;" },
                      //  { name: "Abreviatura", field: "dc_criterio_abreviado", width: "15%", styles: "min-width:40px; text-align: left;"},
                        { name: "Nota", field: "vl_nota", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Peso", field: "peso", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Ordem", field: "nm_ordem_avaliacao", width: "10%", styles: "min-width:50px; text-align: left;" },
                        { name: "Ativa", field: "avaliacao_ativa", width: "10%", styles: "min-width:50px; text-align: left;" }
                    ],
                    selectionMode: "single",
                    canSort: false,
                    noDataMessage: msgNotRegEnc// msgNotRegEstag

                }, "gridOrdenacao");
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridOrdenacao, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosNomes').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_avaliacao', 'selecionadoOrdenacao', -1, 'selecionaTodosNomes', 'selecionaTodosNomes', 'gridOrdenacao')", gridOrdenacao.rowsPerPage * 3);
                    });
                });
                gridOrdenacao.canSort = function () { return false };
                gridOrdenacao.startup();
                gridOrdenacao.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                           item = this.getItem(idx),
                           store = this.store;
                        xhr.get({
                            url: Endereco() + "/api/coordenacao/getAvaliacaoCriterio?cd_tipo_avaliacao=0&cd_criterio_avaliacao=" + item.cd_criterio_avaliacao,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (dataCriterio) {
                            loadNomesCriteriosAtivos(jQuery.parseJSON(dataCriterio).retorno, 'cbNome');
                            keepValuesAvaliacao(null, item);
                            dijit.byId("gridOrdenacao").itensSelecionados = [];
                            dijit.byId("gridOrdenacao").update();
                            dijit.byId("dialogAval").show();
                        });
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridOrdenacao.rowsPerPage = 1000;
                enableOrDisableButton(gridOrdenacao);
                //#endregion

                ///////Montando GridCursoAval
                var gridCursoAval = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                      { name: "<input id='selecionaTodosCurso' style='display:none'/>", field: "selecionadoCurso", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCursoVal },
                      { name: "Produto", field: "no_produto", width: "20%", styles: "width:75px; text-align: left;" },
                      { name: "Curso", field: "no_curso", width: "75%", styles: "min-width:50px;" }
                    ],
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                        pageSizes: ["5", "10", "15", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                        defaultPageSize: "5",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridCursoAval"); // make sure you have a target HTML element with this id
                gridCursoAval.canSort = function (col) { return Math.abs(col) != 1; };
                gridCursoAval.startup();
                dijit.byId("ContentGridCursoAval").resize();
                IncluirAlterar(1, 'divAlterarTipoAvaliacao', 'divIncluirTipoAvaliacao', 'divExcluirTipoAvaliacao', 'apresentadorMensagemTipoAvaliacao', 'divCancelarTipoAvaliacao', 'divLimparTipoAvaliacao');

                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosTipoAvaliacao(); } }, "incluirTipoAvaliacao");
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosTipoAvaliacao(); } }, "alterarTipoAvaliacao");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarTipoAvaliacao() }); } }, "deleteTipoAvaliacao");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparTipoAvaliacao(); } }, "limparTipoAvaliacao");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        showCarregando();
                        apresentaMensagem('apresentadorMensagemTipoAvaliacao', null);
                        keepValues(null, gridTipoAvaliacao, null)
                    }
                }, "cancelarTipoAvaliacao");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadTipoAvaliacao").hide(); } }, "fecharTipoAvaliacao");

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridTipoAvaliacao, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_tipo_avaliacao', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridTipoAvaliacao')", gridTipoAvaliacao.rowsPerPage * 3);
                    });
                });

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditar(gridTipoAvaliacao.itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);
                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridTipoAvaliacao.itensSelecionados, 'DeletarTipoAvaliacao(itensSelecionados)');
                    }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridTipoAvaliacao, 'todosItens', ['pesquisarTipoAvaliacao', 'relatorioTipoAvaliacao']);
                        PesquisarTipoAvaliacao(false);

                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridTipoAvaliacao', 'selecionado', 'cd_tipo_avaliacao', 'selecionaTodos', ['pesquisarTipoAvaliacao', 'relatorioTipoAvaliacao'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                //link grade cursos
                //Metodos para criação do link
                var menuCurso = new DropDownMenu({ style: "height: 25px" });
                var acaoRemoverCurso = new MenuItem({
                    label: "Excluir",
                    onClick: function () { ExcluirRelac(); }
                });
                menuCurso.addChild(acaoRemoverCurso);

                var buttonCurso = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasRelac",
                    dropDown: menuCurso,
                    id: "acoesRelacionadasCurso"
                });
                dojo.byId("linkAcoesCurso").appendChild(buttonCurso.domNode);

                // link para Nomes da avaliação
                var menuNomes = new DropDownMenu({ style: "height: 25px" });
                var acaoRemoverNomes = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarOrdenacao(); }
                });
                menuNomes.addChild(acaoRemoverNomes);

                var buttonNomes = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasNomes",
                    dropDown: menuNomes,
                    id: "acoesRelacionadasNomes"
                });
                dojo.byId("linkAcoesNomes").appendChild(buttonNomes.domNode);

                //fim

                montaBotoesTipoAvaliacao();

                dijit.byId("proCurso").on("Show", function (e) {
                    dijit.byId("gridPesquisaCurso").update();
                });

                dijit.byId("cursos").on("Show", function (e) {
                    dijit.byId("gridCursoAval").update();
                });

                dijit.byId("criterios").on("Show", function (e) {
                    dijit.byId("gridOrdenacao").update();
                });

                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        limparPesquisaCursoFK(true);
                        dijit.byId("proCurso").show();
                        pesquisarCursoFK();
                    }
                }, "incluirCursoFK");

                //***Botões para o dialogo das notas e nomes
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        try{
                            apresentaMensagem('apresentadorMensagemTipoAvaliacao', null);
                            var alterarOrd = dojo.byId('alterarOrdem').value;
                            if (alterarOrd == 1 || dojo.byId("cd_avaliacao").value > 0) {
                                alterarAvaliacaoGrid(dijit.byId("gridOrdenacao"));
                                dijit.byId("dialogAval").hide();
                            }
                            else
                                incluiAvaliacaoGrid();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirAval");

                new Button({
                    label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagemTipoAvaliacao', null);
                        alterarAvaliacaoGrid(dijit.byId("gridOrdenacao"));
                    }
                }, "alterarAvaliacao");

                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        try{
                            dijit.byId("nota").set('value', "000");
                            dijit.byId("peso").set('value', "1,00");
                            dijit.byId("id_avaliacao_ativa").set("value", true);
                            dijit.byId('cbNome').reset();
                            if (dijit.byId("gridOrdenacao")._by_idx.length > 0)
                                document.getElementById("numOrdem").value = dijit.byId("gridOrdenacao")._by_idx[0].item.nm_ordem_avaliacao + 1;
                            else
                                document.getElementById("numOrdem").value = 1;
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparAval");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        try{
                            var dataReal = dojo.byId('data').value;
                            keepValuesAvaliacao(dataReal, null);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarAval");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("dialogAval").hide();
                    }
                }, "fecharAval");
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        try{
                            alterarOrd = 0;
                            document.getElementById("cd_avaliacao").value = '';
                            document.getElementById("nota").value = "000";
                            document.getElementById("peso").value = "1,00";
                            dijit.byId("id_avaliacao_ativa").set("value", true);
                            dijit.byId('cbNome').reset();
                            var idTipoAvaliacao = dojo.byId("cd_tipo_avaliacao").value == "" ? 0 : dojo.byId("cd_tipo_avaliacao").value;
                            xhr.get({
                                url: Endereco() + "/api/coordenacao/getAvaliacaoCriterio?cd_tipo_avaliacao=0&cd_criterio_avaliacao=0",
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (dataCriterio) {
                                loadNomesCriteriosAtivos(jQuery.parseJSON(dataCriterio).retorno, 'cbNome');
                                if (dijit.byId("gridOrdenacao")._by_idx.length > 0) {
                                    var grid = dijit.byId("gridOrdenacao");
                                    for (var i = 0; i < grid._by_idx.length ; i++) {
                                        document.getElementById("numOrdem").value =grid._by_idx[i].item.nm_ordem_avaliacao;
                                    }
                                    document.getElementById("numOrdem").value = parseInt(document.getElementById("numOrdem").value) + 1;
                                }
                                else
                                    document.getElementById("numOrdem").value = 1;
                                document.getElementById('divCancelarAval').style.display = "none";
                                document.getElementById('divLimparAval').style.display = "";
                                document.getElementById('divIncluirAval').style.display = "";
                                document.getElementById('divEditarAval').style.display = "none";
                                dojo.byId('cd_criterio_avaliacao').value = 0;
                                dijit.byId("dialogAval").show();
                            });
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirNomesCriterio");

                if (!hasValue(dijit.byId('subir'))) {
                    new Button({
                        label: "Subir", iconClass: 'dijitEditorIcon dijitEditorIconTabUp',
                        onClick: function () {
                            subirDescerOrdemAvaliacao(dijit.byId("gridOrdenacao"), SOBE);
                        }
                    }, "subir");
                    new Button({
                        label: "Descer", iconClass: 'dijitEditorIcon dijitEditorIconTabDown',
                        onClick: function () {
                            subirDescerOrdemAvaliacao(dijit.byId("gridOrdenacao"), DESCE);
                        }
                    }, "descer");
                }

                dijit.byId("statusCursoFK").set("disabled", true);

                dijit.byId("id_avaliacao_ativa").on("click", function (e) {
                    if (this.checked == true) {
                        document.getElementById('numOrdem').value = hasValue(dijit.byId("gridOrdenacao")._by_idx[0]) ? dijit.byId("gridOrdenacao")._by_idx[0].item.nm_ordem_avaliacao + 1 : 1;
                    }
                    else
                        document.getElementById('numOrdem').value = 0;
                });
                adicionarAtalhoPesquisa(['descTipoAvaliacao', 'statusTipoAvaliacao', 'cbNomes', 'pesProdutoAval', 'pesCursoAval'], 'pesquisarTipoAvaliacao', ready);
                adicionarAtalhoPesquisa(['pesquisaProduto', 'pesquisaCurso'], 'pesquisarCurso', ready);
                dijit.byId("pesquisarCursoFK").on("click", function (e) {
                    apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                    pesquisarCursoFK();
                });

                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function marcarCursosTipo() {
    require([
		"dojo/_base/xhr",
		"dojo/data/ObjectStore",
		"dojo/store/Memory",
		"dojo/ready"
    ], function (xhr, ObjectStore, Memory, ready) {
        ready(function () {
            xhr.get({
                url: Endereco() + "/api/coordenacao/getCursoTipoAvaliacao?id=" + dojo.byId("cd_tipo_avaliacao").value,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try{
                    dataStore = new ObjectStore({ objectStore: new Memory({ data: data.retorno }) });
                    var gridCursoAval = dijit.byId("gridCursoAval");

                    gridCursoAval.setStore(dataStore);
                } catch (e) {
                    postGerarLog(e);
                }
            });
        });
    });
}

function montaBotoesTipoAvaliacao() {
    require([
          "dojo/_base/xhr",
          "dijit/form/Button"
    ], function (xhr, Button) {
        try{
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarTipoAvaliacao(true); }
            }, "pesquisarTipoAvaliacao");
            decreaseBtn(document.getElementById("pesquisarTipoAvaliacao"), '32px');

            dijit.byId("pesquisaProduto").set("required", false);
            new Button({
                label: "",
                iconClass: 'dijitEditorIcon dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarCurso(true); }
            }, "pesquisarCurso");

            new Button({
                label: "",
                iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarNomes(true); }
            }, "pesquisarNomes");
            decreaseBtn(document.getElementById("pesquisarCurso"), '32px');
            btnPesquisar(document.getElementById("pesquisarNomes"));

            // TipoAvaliacao
            var tipoAvaliacao = hasValue(dijit.byId('cbTipos')) ? dijit.byId('cbTipos').value : 0;
            var criterioAvaliacao = hasValue(dijit.byId('cbNomes')) ? dijit.byId('cbNomes').value : 0;
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        showCarregando();
                        populaProduto(xhr);
                        limparTipoAvaliacao();
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarTipoAvaliacao', 'divIncluirTipoAvaliacao', 'divExcluirTipoAvaliacao', 'apresentadorMensagemTipoAvaliacao', 'divCancelarTipoAvaliacao', 'divLimparTipoAvaliacao');
                        dijit.byId("cursos").set("open", false);
                        dijit.byId("criterios").set("open", false);
                        clearGrid('gridOrdenacao');
                        var gridAvaliacaoOrdem = dijit.byId("gridOrdenacao");
                        enableOrDisableButton(gridAvaliacaoOrdem);
                        dijit.byId("cadTipoAvaliacao").show();
                        dijit.byId("gridOrdenacao").update();
                        showCarregando();
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoTipoAvaliacao");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    try{
                        var tipoAvaliacao = hasValue(dijit.byId('cbTipos')) ? dijit.byId('cbTipos').value : 0;
                        var criterioAvaliacao = hasValue(dijit.byId('cbNomes')) ? dijit.byId('cbNomes').value : 0;
                        var cdProdutoPes = hasValue(dijit.byId("pesProdutoAval")) ? dijit.byId("pesProdutoAval").value : 0;
                        var cdCursoPes = hasValue(dijit.byId("pesCursoAval")) ? dijit.byId("pesCursoAval").value : 0;
                        xhr.get({
                            url: Endereco() + "/api/coordenacao/GetUrlRelatorioTipoAvaliacao?" + getStrGridParameters('gridTipoAvaliacao') + "descricao=" + encodeURIComponent(document.getElementById("descTipoAvaliacao").value) + "&inicio=" + document.getElementById("inicioTipoAvaliacao").checked + "&status=" + retornaStatus("statusTipoAvaliacao") + "&cd_tipo_avaliacao=" + tipoAvaliacao + "&cd_criterio_avaliacao=" + criterioAvaliacao + "&cdCurso=" +  cdCursoPes + "&cdProduto=" + cdProdutoPes,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "relatorioTipoAvaliacao");
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function populaProduto(xhr) {
    xhr.get({
        url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_ATIVO + "&cd_produto=null",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Authorization": Token() }
    }).then(function (dataProdAtivo) {
        try{
            loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, 'pesquisaProduto');
        } catch (e) {
            postGerarLog(e);
        }
    }, function (error) {
        apresentaMensagem('apresentadorMensagemTipoAvaliacao', error);
    });
}

function IncluirTipoAvaliacao() {
    try{
        var cursos = CursosTipo();
        var avaliacoes = NomesAvaliacao();
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemTipoAvaliacao', null);
        if (!dijit.byId("formTipoAvaliacao").validate())
            return false;
        showCarregando();
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            xhr.post(Endereco() + "/api/coordenacao/postTipoAvaliacao", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_tipo_avaliacao: dom.byId("dc_tipo_avaliacao").value,
                    vl_total_nota: dom.byId("vl_total_nota").value,
                    id_tipo_ativo: domAttr.get("id_tipo_ativo", "checked"),
                    AvaliacaoCurso: cursos,
                    Avaliacao: avaliacoes
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        if (data.retorno != null) {
                            var itemAlterado = data.retorno;
                            var gridName = 'gridTipoAvaliacao';
                            var grid = dijit.byId(gridName);

                            apresentaMensagem('apresentadorMensagem', data);
                            dijit.byId("cadTipoAvaliacao").hide();

                            if (!hasValue(grid.itensSelecionados))
                                grid.itensSelecionados = [];
                            dijit.byId("statusTipoAvaliacao").set("value", 0);
                            insertObjSort(grid.itensSelecionados, "cd_tipo_avaliacao", itemAlterado);

                            buscarItensSelecionados(gridName, 'selecionadoTipoAvaliacao', 'cd_tipo_avaliacao', 'selecionaTodosTipoAvaliacao', ['pesquisarTipoAvaliacao', 'relatorioTipoAvaliacao'], 'todosItens');
                            grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                            setGridPagination(grid, itemAlterado, "cd_tipo_avaliacao");
                        } else {
                            apresentaMensagem('apresentadorMensagemTipoAvaliacao', data.MensagensWeb);
                        }
                    }
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoAvaliacao', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function PesquisarTipoAvaliacao(limparItens) {
    try{
        var tipoAvaliacao = hasValue(dijit.byId('cbTipos')) ? dijit.byId('cbTipos').value : 0;
        var criterioAvaliacao = hasValue(dijit.byId('cbNomes')) ? dijit.byId('cbNomes').value : 0;
        var cdProdutoPes = hasValue(dijit.byId("pesProdutoAval"))? dijit.byId("pesProdutoAval").value : 0;
        var cdCursoPes = hasValue(dijit.byId("pesCursoAval"))? dijit.byId("pesCursoAval").value : 0;
        require([
                  "dojo/store/JsonRest",
                  "dojo/data/ObjectStore",
                  "dojo/store/Cache",
                  "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                var myStore = Cache(
                        JsonRest({
                            handleAs: "json",
                            target: Endereco() + "/api/coordenacao/getTipoAvaliacaoSearch?descricao=" + encodeURIComponent(document.getElementById("descTipoAvaliacao").value) + "&inicio=" + document.getElementById("inicioTipoAvaliacao").checked + "&status=" + retornaStatus("statusTipoAvaliacao") + "&cd_tipo_avaliacao=" + tipoAvaliacao + "&cd_criterio_avaliacao=" + criterioAvaliacao + "&cdCurso=" + cdCursoPes + "&cdProduto=" + cdProdutoPes,
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }
                ), Memory({}));
                dataStore = new ObjectStore({ objectStore: myStore });

                var gridTipoAvaliacao = dijit.byId("gridTipoAvaliacao");

                if (limparItens)
                    gridTipoAvaliacao.itensSelecionados = [];

                gridTipoAvaliacao.setStore(dataStore);
            } catch (e) {
                postGerarLog(e);
            }
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function loadProduto(data, linkProduto) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        if ((linkProduto == 'pesquisaProduto'))
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_produto, name: value.no_produto });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId(linkProduto).store = stateStore;
		    } catch (e) {
		        postGerarLog(e);
		    }
		});
}

function CursosTipo() {
    try{
        var cursos = null;
        if (hasValue(dijit.byId("gridCursoAval")))
            if (dijit.byId("gridCursoAval").store.objectStore.data.length > 0)
                cursos = dijit.byId("gridCursoAval").store.objectStore.data;
            else
                cursos = [];
        return cursos;
    } catch (e) {
        postGerarLog(e);
    }
}

function NomesAvaliacao() {
    try{
        var nomes = null;
        if (hasValue(dijit.byId("gridOrdenacao")))
            if (dijit.byId("gridOrdenacao").store.objectStore.data.length > 0)
                nomes = dijit.byId("gridOrdenacao").store.objectStore.data;
            else
                nomes = [];
        return nomes;
    } catch (e) {
        postGerarLog(e);
    }
}

//function PesquisarCurso(limparItens) {
//    var cdTipoAval = !hasValue(dojo.byId("cd_tipo_avaliacao").value) ? 0 : dojo.byId("cd_tipo_avaliacao").value;
//    require([
//              "dojo/store/JsonRest",
//              "dojo/data/ObjectStore",
//              "dojo/store/Cache",
//              "dojo/store/Memory"
//    ], function (JsonRest, ObjectStore, Cache, Memory) {
//        try{
//            var myStore= Cache(
//            JsonRest({
//                target: Endereco() + "/api/curso/getCursoProdutoPorTipoAval?desc=" + encodeURIComponent(document.getElementById("pesquisaCurso").value) + "&cdProd=" + dijit.byId("pesquisaProduto").get("value") + "&cdTipoAvaliacao=" + cdTipoAval,
//                handleAs: "json",
//                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
//                idProperty: "cd_curso"
//            }), Memory({ idProperty: "cd_curso" }));
//            dataStore = new ObjectStore({ objectStore: myStore });

//            var gridCursoAval = dijit.byId("gridCursoAval");

//            if (limparItens)
//                gridCursoAval.itensSelecionados = [];

//            gridCursoAval.setStore(dataStore);
//        } catch (e) {
//            postGerarLog(e);
//        }
//    })
//  //  marcarCursosTipo();
//}

function PesquisarCurso(limparItens) {
    var cdTipoAval = !hasValue(dojo.byId("cd_tipo_avaliacao").value) ? 0 : dojo.byId("cd_tipo_avaliacao").value;
    showCarregando();
    require([
		"dojo/_base/xhr",
		"dojo/store/JsonRest",
		"dojo/data/ObjectStore",
		"dojo/store/Memory",
		"dojo/ready"
    ], function (xhr, JsonRest, ObjectStore, Memory, ready) {
        ready(function () {
            xhr.get({
                url: Endereco() + "/api/curso/getCursoProdutoPorTipoAval?desc=" + encodeURIComponent(document.getElementById("pesquisaCurso").value) + "&cdProd=" + dijit.byId("pesquisaProduto").get("value") + "&cdTipoAvaliacao=" + cdTipoAval,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    dijit.byId("gridCursoAval").setStore(new ObjectStore({ objectStore: new Memory({ data: data }) }));
                } catch (e) {
                    postGerarLog(e);
                }
                showCarregando();
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoAvaliacao', error);
            });
        });
    });
}

function AlterarTipoAvaliacao() {
    var cursos = CursosTipo();
    var avaliacoes = NomesAvaliacao();
    var gridName = 'gridTipoAvaliacao';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formTipoAvaliacao").validate())
        return false;
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/coordenacao/postalterarTipoAvaliacao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_tipo_avaliacao: dom.byId("cd_tipo_avaliacao").value,
                dc_tipo_avaliacao: dom.byId("dc_tipo_avaliacao").value,
                vl_total_nota: dom.byId("vl_total_nota").value,
                id_tipo_ativo: domAttr.get("id_tipo_ativo", "checked"),
                AvaliacaoCurso: cursos,
                Avaliacao: avaliacoes
            })
        }).then(function (data) {
            try{
                if (!hasValue(data.erro)) {
                    if (data.retorno != null) {
                        var itemAlterado = data.retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];
                        dijit.byId("statusTipoAvaliacao").set("value", 0);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadTipoAvaliacao").hide();
                        removeObjSort(grid.itensSelecionados, "cd_tipo_avaliacao", dom.byId("cd_tipo_avaliacao").value);
                        insertObjSort(grid.itensSelecionados, "cd_tipo_avaliacao", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoTipoAvaliacao', 'cd_tipo_avaliacao', 'selecionaTodosTipoAvaliacao', ['pesquisarTipoAvaliacao', 'relatorioTipoAvaliacao'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_tipo_avaliacao");
                    } else {
                        apresentaMensagem('apresentadorMensagemTipoAvaliacao', data.MensagensWeb);
                    }
                }
                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemTipoAvaliacao', error);
        });
    });
}

function DeletarTipoAvaliacao(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_tipo_avaliacao').value != 0)
                    itensSelecionados = [{
                        cd_tipo_avaliacao: dom.byId("cd_tipo_avaliacao").value,
                        dc_tipo_avaliacao: dom.byId("dc_tipo_avaliacao").value,
                        vl_total_nota: dom.byId("vl_total_nota").value,
                        id_tipo_ativo: domAttr.get("id_tipo_ativo", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteTipoAvaliacao",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItens_label");
                    PesquisarTipoAvaliacao(false);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadTipoAvaliacao").hide();
                    dijit.byId("pesquisarTipoAvaliacao").set("value", '');
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridTipoAvaliacao').itensSelecionados, "cd_tipo_avaliacao", itensSelecionados[r].cd_tipo_avaliacao);
                    dijit.byId("pesquisarTipoAvaliacao").set('disabled', false);
                    dijit.byId("relatorioTipoAvaliacao").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadTipoAvaliacao").style.display))
                    apresentaMensagem('apresentadorMensagemTipoAvaliacao', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function retornarCursoFK() {
    require(["dojo/ready", "dojo/store/Memory", "dojo/data/ObjectStore"],
        function (ready, Memory, ObjectStore) {
            try{
                var gridCursoAval = dijit.byId("gridCursoAval");
                var gridCursosFK = dijit.byId("gridPesquisaCurso");
                if (!hasValue(gridCursosFK.itensSelecionados) || gridCursosFK.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                else {
                    var storeGridCurso = (hasValue(gridCursoAval) && hasValue(gridCursoAval.store.objectStore.data)) ? gridCursoAval.store.objectStore.data : [];
                    if (storeGridCurso != null && storeGridCurso.length > 0) {
                        $.each(gridCursosFK.itensSelecionados, function (idx, value) {
                            insertObjSort(gridCursoAval.store.objectStore.data, "cd_curso", { cod_tipo_avaliacao_curso: 0, cd_tipo_avaliacao: 0, cd_curso: value.cd_curso, no_produto: value.no_produto, no_curso: value.no_curso });
                        });
                        gridCursoAval.setStore(new ObjectStore({ objectStore: new Memory({ data: gridCursoAval.store.objectStore.data }) }));
                    } else {
                        var dados = [];
                        $.each(gridCursosFK.itensSelecionados, function (index, val) {
                            insertObjSort(dados, "cd_curso", { cod_tipo_avaliacao_curso: 0, cd_tipo_avaliacao: 0, cd_curso: val.cd_curso, no_produto: val.no_produto, no_curso: val.no_curso });
                        });
                        gridCursoAval.setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));
                    }
                }
                dojo.byId("ehSelectGrade").value = false;
                dijit.byId("proCurso").hide();
            } catch (e) {
                postGerarLog(e);
            }
        });
}

function ExcluirRelac() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
   function (Memory, ObjectStore) {
       try{
           var value = null;
           var gridCursoAval = dijit.byId("gridCursoAval");
           var dados = [];
           value = (hasValue(gridCursoAval) && hasValue(gridCursoAval.itensSelecionados)) ? gridCursoAval.itensSelecionados : [];
           var itensSelecionados = value;
           if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
               caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
               return false;
           }

           var arrayRelacionamentos = [];
           arrayCursos = (hasValue(gridCursoAval) && hasValue(gridCursoAval.store.objectStore.data)) ? gridCursoAval.store.objectStore.data : [];
           if (itensSelecionados.length > 0) {
               $.each(itensSelecionados, function (idx, value) {
                   arrayCursos = jQuery.grep(arrayCursos, function (val) {
                       return val != value;
                   });
                   insertObjSort(arrayRelacionamentos, "cd_curso", value);
               });
           }
           if (hasValue(arrayRelacionamentos) && arrayRelacionamentos.length > 0)
               $.each(arrayRelacionamentos, function (index, val) {
                   removeObjSort(gridCursoAval.itensSelecionados, "cd_curso", val.cd_curso);
               });
           dados = arrayCursos.slice(0);

           var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
           gridCursoAval.setStore(dataStore);
           var mensagensWeb = new Array();
           mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Curso excluido com sucesso.");
           apresentaMensagem("apresentadorMensagemTipoAvaliacao", mensagensWeb);
       } catch (e) {
           postGerarLog(e);
       }
   });
}


//#region montado a grade de nomes ordenada

// inicio da formatação da Avaliacao
function formatCheckBoxOrdenacao(value, rowIndex, obj) {
    try{
        var gridName = 'gridOrdenacao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nm_ordem_avaliacao", grid._by_idx[rowIndex].item.nm_ordem_avaliacao);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nm_ordem_avaliacao', 'selecionadoOrdenacao', -1, 'selecionaTodosNomes', 'selecionaTodosNomes', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'nm_ordem_avaliacao', 'selecionadoOrdenacao', " + rowIndex + ", '" + id + "', 'selecionaTodosNomes', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region  manipulando dados da grade - popularGridAvaliacao - incluiAvaliacaoGrid - alterarAvaliacaoGrid - setStoreGrid - deletarOrdenacao -
// deletarOrdenacao - montarAvaliacoesSelecionadas - validarTotalNotaTipoAvaliacao - subirDescerOrdemAvaliacao
// Método para incluir na grade
function popularGridAvaliacao() {
    require([
		"dojo/_base/xhr",
		"dojo/store/JsonRest",
		"dojo/data/ObjectStore",
		"dojo/store/Memory",
		"dojo/ready"
    ], function (xhr, JsonRest, ObjectStore, Memory, ready) {
        ready(function () {
            xhr.get({
                url: Endereco() + "/api/coordenacao/getAvaliacaoByIdTipoAvaliacao?idTipoAvaliacao=" + dojo.byId("cd_tipo_avaliacao").value,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try{
                    dijit.byId("gridOrdenacao").setStore(new ObjectStore({ objectStore: new Memory({ data: data.retorno }) }));
                } catch (e) {
                    postGerarLog(e);
                }
                showCarregando();
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoAvaliacao', error);
            });
        });
    });
}

function incluiAvaliacaoGrid() {
    try{
        if (!dijit.byId("formAvaliacao").validate())
            return false;
        var gridAvaliacaoOrdem = dijit.byId("gridOrdenacao");
        var cdAval = dojo.byId("cd_avaliacao").value == "" ? 0 : dojo.byId("cd_avaliacao").value;
        if (!validarTotalNotaTipoAvaliacao(dojo.number.parse(dojo.byId("nota").value), parseFloat(dojo.byId("peso").value)))
            return false
        var myNewStore = setStoreGrid();
        if (myNewStore.id_avaliacao_ativa == false)
            myNewStore.nm_ordem_avaliacao = null;
        if (hasValue(gridAvaliacaoOrdem) && (gridAvaliacaoOrdem.store.objectStore.data.length > 0))
            for (var i = 0; i < gridAvaliacaoOrdem.store.objectStore.data.length; i++) {
                if (gridAvaliacaoOrdem.store.objectStore.data[i].cd_criterio_avaliacao == myNewStore.cd_criterio_avaliacao) {
                    caixaDialogo(DIALOGO_AVISO, 'Esse registro já existe na grade.', null);
                    return false;
                    break;
                }
            }
        gridAvaliacaoOrdem.store.newItem(myNewStore);
        gridAvaliacaoOrdem.store.save();

        for (var l = 0; l < gridAvaliacaoOrdem._by_idx.length ; l++)
            gridAvaliacaoOrdem._by_idx[l].nm_ordem_avaliacao = gridAvaliacaoOrdem._by_idx[l].item.nm_ordem_avaliacao;

        quickSortObj(gridAvaliacaoOrdem._by_idx, 'nm_ordem_avaliacao');
        gridAvaliacaoOrdem._by_idx.reverse();

        for (var l = gridAvaliacaoOrdem._by_idx.length - 1, j = 1; l >= 0; l--, j++)
            if (gridAvaliacaoOrdem._by_idx[l].item.nm_ordem_avaliacao > 0)
                gridAvaliacaoOrdem._by_idx[l].item.nm_ordem_avaliacao = j;
            else
                j = j - 1;

        gridAvaliacaoOrdem._by_idx.reverse();

        for (var i = 0; i < gridAvaliacaoOrdem._by_idx.length ; i++) {
            gridAvaliacaoOrdem.selection.setSelected(i, false);
            if (gridAvaliacaoOrdem._by_idx[i].item.nm_ordem_avaliacao == myNewStore.nm_ordem_avaliacao) {
                //Seleciona a linha com o item editado por default:
                gridAvaliacaoOrdem.selection.setSelected(i, true);
                //Posiciona o foco do scroll no item editado:
                setTimeout('atualiza(' + i + ')', 10);
            }
        }
        enableOrDisableButton(gridAvaliacaoOrdem);
        dijit.byId("dialogAval").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

//Edita um registro na grid
function alterarAvaliacaoGrid(grid) {
    try{
        var cdAval = dojo.byId("cd_avaliacao").value == "" ? 0 : dojo.byId("cd_avaliacao").value;
        var peso = 0;
        for (var l = 0; l < grid._by_idx.length ; l++) {
            if (grid._by_idx[l].item.cd_criterio_avaliacao == dijit.byId('cbNome').value)
                if (grid._by_idx[l].item.nm_ordem_avaliacao == ordemOri) {
                    // Muda os itens de lugares e altera o registro na grade:
                    grid._by_idx[l].item.vl_nota = dojo.byId("nota").value;
                    if (!validarTotalNotaTipoAvaliacao(dojo.number.parse(dojo.byId("nota").value), parseFloat(dojo.byId("peso").value))) {
                        grid._by_idx[l].item.vl_nota = 0;
                        return false;
                    }
                    peso = dojo.byId("peso").value;
                    grid._by_idx[l].item.nm_peso_avaliacao = parseFloat(peso.toString().replace(",", ".")).toFixed(2);
                    grid._by_idx[l].item.peso = peso;
                    grid._by_idx[l].item.id_avaliacao_ativa = document.getElementById('id_avaliacao_ativa').checked;
                    grid._by_idx[l].item.dc_criterio_avaliacao = dojo.byId('cbNome').value;
                    grid._by_idx[l].item.cd_criterio_avaliacao = dijit.byId('cbNome').value;
                    grid._by_idx[l].item.avaliacao_ativa = 'Sim';
                    if (grid._by_idx[l].item.nm_ordem_avaliacao < parseInt(document.getElementById('numOrdem').value))
                        grid._by_idx[l].item.nm_ordem_avaliacao = parseInt(document.getElementById('numOrdem').value) + 0.5;
                    else
                        grid._by_idx[l].item.nm_ordem_avaliacao = parseInt(document.getElementById('numOrdem').value) - 0.5;
                    if (document.getElementById('id_avaliacao_ativa').checked == false) {
                        grid._by_idx[l].item.avaliacao_ativa = 'Não';
                        grid._by_idx[l].item.nm_ordem_avaliacao = 0;
                    }
                }
            grid._by_idx[l].nm_ordem_avaliacao = grid._by_idx[l].item.nm_ordem_avaliacao;
        }
        quickSortObj(grid._by_idx, 'nm_ordem_avaliacao');
        grid._by_idx.reverse();
        for (var l = grid._by_idx.length - 1, j = 1; l >= 0; l--, j++) {
            if (grid._by_idx[l].item.nm_ordem_avaliacao > 0)
                grid._by_idx[l].item.nm_ordem_avaliacao = j;
            else
                j = j - 1;
        }
        grid._by_idx.reverse();
        grid.update();
        for (var i = 0; i <= grid._by_idx.length - 1; i++) {
            grid.selection.setSelected(i, false);
            if (grid._by_idx[i].item.cd_avaliacao == parseInt(document.getElementById('cd_avaliacao').value)) {
                //Seleciona a linha com o item editado por default:
                grid.selection.setSelected(i, true);
                ////Posiciona o foco do scroll no item editado:
                grid.getRowNode(i).id = '';
                grid.getRowNode(i).id = 'ordem_' + parseInt(document.getElementById('cd_avaliacao').value);
                window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                window.location.hash = '#ordem_' + parseInt(document.getElementById('cd_avaliacao').value);
                //break;
            }
        }
        dijit.byId("dialogAval").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function setStoreGrid() {
    try{
        var pesoAval = dojo.byId("peso").value;
        var myNewStore = {
            cd_avaliacao: hasValue(dojo.byId('cd_avaliacao').value) ? dojo.byId('cd_avaliacao').value : "",
            cd_criterio_avaliacao: dijit.byId("cbNome").value,
            dc_criterio_avaliacao: dojo.byId("cbNome").value,
            avaliacao_ativa: document.getElementById('id_avaliacao_ativa').checked == true ? "Sim" : "Não",
            id_avaliacao_ativa: document.getElementById('id_avaliacao_ativa').checked,
            nm_ordem_avaliacao: parseInt(document.getElementById('numOrdem').value) > 0 ? parseInt(document.getElementById('numOrdem').value) - 0.5 : parseInt(document.getElementById('numOrdem').value),
            vl_nota: dojo.byId("nota").value,
            nm_peso_avaliacao: parseFloat(pesoAval.toString().replace(",", ".")).toFixed(2),
            peso: pesoAval
        };
        return myNewStore;
    } catch (e) {
        postGerarLog(e);
    }
}

function dataRealAvaliacao(value) {
    try{
        this.cd_criterio_avaliacao = value.cd_criterio_avaliacao;
        this.id_avaliacao_ativa = value.id_avaliacao_ativa;
        this.nm_ordem_avaliacao = value.nm_ordem_avaliacao;
        this.vl_nota = value.vl_nota;
        this.nm_peso_avaliacao = value.nm_peso_avaliacao;
        this.peso = value.peso;
    } catch (e) {
        postGerarLog(e);
    }
}

//Deleta os dados da gride
function deletarOrdenacao() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
    function (Memory, ObjectStore) {
        try{
            var aval = montarAvaliacoesSelecionadas();
            if (aval.length > 0) {
                var arrayAval = dijit.byId("gridOrdenacao")._by_idx;
                $.each(aval, function (idx, valueAval) {
                    arrayAval = jQuery.grep(arrayAval, function (value) {
                        return value.item != valueAval;
                    });
                });
                var dados = [];
                $.each(arrayAval, function (index, value) {
                    dados.push(value.item);
                });
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
                dijit.byId("gridOrdenacao").setStore(dataStore);
            }
            //for (var l = dijit.byId("gridOrdenacao")._by_idx.length - 1, j = 1; l >= 0; l--, j++)
            //    dijit.byId("gridOrdenacao")._by_idx[l].item.nm_ordem_avaliacao = j;
            var m = 0;
            for (var i = 0; i < dijit.byId("gridOrdenacao")._by_idx.length; i++) {
                m++;
                dijit.byId("gridOrdenacao")._by_idx[i].item.nm_ordem_avaliacao = m;
            }
            dijit.byId("gridOrdenacao").itensSelecionados = [];
            dijit.byId("gridOrdenacao").update();
        } catch (e) {
            postGerarLog(e);
        }
    });
}

//Monta os dados da grade ordenacao para serem deletados na fução deletarOrdenação
function montarAvaliacoesSelecionadas() {
    try{
        var dados = [];
        if (hasValue(dijit.byId("gridOrdenacao")) && hasValue(dijit.byId("gridOrdenacao").store.objectStore.data)) {
            var storeAval = dijit.byId("gridOrdenacao").itensSelecionados;
            $.each(storeAval, function (index, val) {
                dados.push(val);
            });
            return dados;
        }
        return null;
    } catch (e) {
        postGerarLog(e);
    }
}

// Consistência da nota para não ultrapassar a nota do tipo de avaliação. - subirDescerOrdemAvaliacao
function validarTotalNotaTipoAvaliacao(nota, peso) {
    try{
        var grid = hasValue(dijit.byId('gridOrdenacao')) ? dijit.byId('gridOrdenacao') : null;
        var notaTotalAvaliacao = dojo.number.parse(dijit.byId('vl_total_nota').value);
        var idCriterio = dojo.number.parse(dojo.byId('cd_criterio_avaliacao').value);
        notaTotalAvaliacao = isNaN(notaTotalAvaliacao) ? 0 : notaTotalAvaliacao;
        var notasLancadas = 0;
        var retorno = true;
        if ((nota * peso) > notaTotalAvaliacao) {
            caixaDialogo(DIALOGO_AVISO, 'A nota lançada não pode ser maior que a nota máxima da avaliação.', null);
            retorno = false;
            return false;
        }
        if (grid != null) {
            for (var i = 0; i < dijit.byId('gridOrdenacao')._by_idx.length; i++)
                if (dijit.byId('gridOrdenacao')._by_idx[i].item.cd_criterio_avaliacao != idCriterio) {
                    notasLancadas = notasLancadas + (dojo.number.parse(dijit.byId('gridOrdenacao')._by_idx[i].item.vl_nota) * parseFloat(dijit.byId('gridOrdenacao')._by_idx[i].item.nm_peso_avaliacao));
                }
            //atribui a nota so se não existir critério
            //if (idCriterio == 0)
            notasLancadas = notasLancadas + (nota * peso);
            if (notasLancadas > notaTotalAvaliacao) {
                caixaDialogo(DIALOGO_AVISO, 'Somatória das notas de todas as avaliações deste tipo ultrapassou o valor máximo definido neste.', null);
                retorno = false;
                return false;
            }
        }
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}

function subirDescerOrdemAvaliacao(grid, sobeDesce) {
    try{
        var operacao = sobeDesce == SOBE ? 1 : -1;
        if (grid.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            grid.itensSelecionados = [];
            grid.update();
            return false;
        }

        if (grid.itensSelecionados.length > 0 && grid.itensSelecionados[0].id_avaliacao_ativa == false) {
            caixaDialogo(DIALOGO_AVISO, msgNaoRegOrdenarInativo, null);
            grid.itensSelecionados = [];
            grid.update();
            return false;
        }

        var itemSelecionado = grid.itensSelecionados;

        if (itemSelecionado.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegOrdem, null);
        else {
            for (var l = 0; l < grid._by_idx.length; l++)
                if (grid._by_idx[l].item.nm_ordem_avaliacao == itemSelecionado[0].nm_ordem_avaliacao) {
                    if (hasValue(grid._by_idx[l - (operacao)])) {
                        var itemEncontrado = grid._by_idx[l].item;
                        var ordemEncontrada = grid._by_idx[l].item.nm_ordem_avaliacao;
                        var posicaoEncontrada = grid.selection.selectedIndex;

                        // Muda as ordens de lugares:
                        if (grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao == 0) {
                            caixaDialogo(DIALOGO_AVISO, msgItemPrimeiraPosicao, null);
                            grid.update();
                            return false;
                        } else {
                            grid._by_idx[l].item.nm_ordem_avaliacao = grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao;
                            grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao = ordemEncontrada;
                            // Muda os itens de lugares:
                            grid._by_idx[l].item = grid._by_idx[l - (operacao)].item;
                            grid._by_idx[l - (operacao)].item = itemEncontrado;
                            grid.update();
                        }
                        grid.getRowNode(l).id = '';
                        grid.getRowNode(l - (operacao)).id = 'ordem_' + grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao;
                        window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                        window.location.hash = '#' + 'ordem_' + grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao;

                        // Atualiza o item selecionado:
                        grid.selection.setSelected(posicaoEncontrada, false);
                        if (posicaoEncontrada < grid._by_idx.length)
                            grid.selection.setSelected(posicaoEncontrada - 1, true);
                        var codAlteradoSubir = grid._by_idx[l].item.nm_ordem_avaliacao + ";" + grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao + ";";

                    }
                    return codAlteradoSubir;
                    break;
                }
        }
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion