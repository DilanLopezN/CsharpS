var CONSULTA_PRODUTO_HAS_CURSO = 4;
var CONSULTA_MODALIDADE_HAS_CURSO = 2;
var CONSULTA_NIVEL_HAS_ATIVO = 0;

//Declarando as variável para retorno da FK de aluno.
var REAJUSTE_ANUAL_CURSO = 1;

function montargridPesquisaCurso(pesquisaInicial, funcao) {
    require([
       "dojo/_base/xhr",
       "dojox/grid/EnhancedGrid",
       "dojox/grid/enhanced/plugins/Pagination",
       "dojo/store/JsonRest",
       "dojo/data/ObjectStore",
       "dojo/store/Cache",
       "dojo/store/Memory",
       "dojo/dom-attr",
       "dijit/form/Button",
       "dojo/ready",
       "dojo/on"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, domAttr, Button, ready, on) {
        ready(function () {
            try {
                montarStatus("statusCursoFK");
                var myStore = null;
                var store = null
                if (pesquisaInicial != null && pesquisaInicial != undefined && pesquisaInicial == true) {
                    
                    store = new ObjectStore({ objectStore: new Memory({ data: null }) });
                } else {
                     myStore = Cache(
                        JsonRest({
                                target: !hasValue(document.getElementById("descCursoFK").value) ? Endereco() + "/api/curso/getCursoSearch?desc=&inicio=" + document.getElementById("inicioCursoFK").checked + "&status=1" + "&produto=" + dijit.byId('cbPesqProdutoFK').value + "&estagio=" + dijit.byId('cbPesqEstagiosFK').value + "&modalidade=" + dijit.byId('cbPesqModalidadesFK').value + "&nivel=" + dijit.byId('cbPesqNivel').value : Endereco() + "/api/curso/getCursoSearch?desc=" + encodeURIComponent(document.getElementById("descCurso").value) + "&inicio=" + document.getElementById("inicioCursoFK").checked + "&status=1" + "&produto=" + dijit.byId('cbPesqProdutoFK').value + "&estagio=" + dijit.byId('cbPesqEstagiosFK').value + "&modalidade=" + dijit.byId('cbPesqModalidadesFK').value + "&nivel=" + dijit.byId('cbPesqNivel').value,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }
                        ), Memory({}));
                     store = new ObjectStore({ objectStore: myStore });
                }
                

                //*** Cria a grade de Cursos **\\
                var gridCursoFK = new EnhancedGrid({
                    store: store,
                    structure:
                      [
                        { name: "<input id='selecionaTodosCursoFK' style='display:none'/>", field: "selecionadoCursoFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCursoFK },
                        //{ name: "Código", field: "cd_curso", width: "75px", styles: "width:75px; text-align: left;" },
                        { name: "Nome", field: "no_curso", width: "25%", styles: "min-width:80px;" },
                        { name: "Produto", field: "no_produto", width: "20%", styles: "min-width:80px;" },
                        { name: "Estágio", field: "no_estagio", width: "20%", styles: "min-width:80px;" },
                        { name: "Série", field: "no_modalidade", width: "20%", styles: "min-width:80px;" },
                        { name: "Ativo", field: "curso_ativo", width: "10%", styles: "text-align: center; min-width:40px; max-width: 50px;" }
                      ],
                    noDataMessage: "Nenhum registro encontrado.",
                    selectionMode: "single",
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
                }, "gridPesquisaCurso");
                gridCursoFK.on("RowDblClick", function () {
                    //if (hasValue(retornarCursoFK)) {
                    //    dojo.byId("ehSelectGrade").value = true;
                    //    retornarCursoFK();
                    //}
                }, true);
                gridCursoFK.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 3 && Math.abs(col) != 4 && Math.abs(col) != 5 && Math.abs(col) != 6; };
                gridCursoFK.pagination.plugin._paginator.plugin.connect(gridCursoFK.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    try {
                        verificaMostrarTodos(evt, gridCursoFK, 'cd_curso', 'selecionaTodosCursoFK');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridCursoFK, "_onFetchComplete", function () {
                        try {
                            // Configura o check de todos:
                            if (hasValue(dojo.byId('selecionaTodosCursoFK')) && dojo.byId('selecionaTodosCursoFK').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_curso', 'selecionadoCursoFK', -1, 'selecionaTodosCursoFK', 'selecionaTodosCursoFK', 'gridPesquisaCurso')", gridCursoFK.rowsPerPage * 3);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                });
                gridCursoFK.startup();

                populaProdutosFK(null, 'cbPesqProdutoFK', CONSULTA_PRODUTO_HAS_CURSO);
                populaNiveisFK(null, 'cbPesqNivel', CONSULTA_NIVEL_HAS_ATIVO);
                populaModalidadesFK(null, 'cbPesqModalidadesFK', CONSULTA_MODALIDADE_HAS_CURSO);

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "pesquisarCursoFK");
                decreaseBtn(document.getElementById("pesquisarCursoFK"), '32px');
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            if (hasValue(retornarCursoFK)) {
                                retornarCursoFK();
                                dojo.byId("ehSelectGrade").value = false;
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaCursoFK");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("proCurso").hide(); } }, "fecharCursoFK");

                dijit.byId("cbPesqProdutoFK").on("change", function (e) {
                    try {
                        if (e != null && e > 0)
                            populaEstagiosFK(e, null, 'cbPesqEstagiosFK', CONSULTA_MODALIDADE_HAS_CURSO);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                adicionarAtalhoPesquisa(['descCursoFK', 'cbPesqProdutoFK', 'cbPesqEstagiosFK', 'cbPesqModalidadesFK', 'statusCursoFK'], 'pesquisarCursoFK', ready);

                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function populaProdutosFK(idProduto, field, criterios) {
    try {
        // Popula os produtos:
        dojo.xhr.get({
            url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + criterios + "&cd_produto=null",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataProdAtivo) {
            try {
                loadProdutoFK(jQuery.parseJSON(dataProdAtivo).retorno, field, idProduto);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaNiveisFK(idNivel, field, criterios) {
    try {
        // Popula os niveis:
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/Coordenacao/getNivel?hasDependente=" + criterios,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataNivdAtivo) {
                loadNivelFK(jQuery.parseJSON(dataNivdAtivo).retorno, field, idNivel);
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemFks', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaModalidadesFK(idModalidade, field, criterios) {
    try {
        // Popula as modalidades:
        dojo.xhr.get({
            url: Endereco() + "/api/Coordenacao/getModalidades?criterios=" + criterios + "&cd_modalidade=" + idModalidade,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataModAtiva) {
            try {
                loadModalidadesFK(jQuery.parseJSON(dataModAtiva).retorno, field, idModalidade);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaEstagiosFK(idProduto, idEstagio, field, criterios) {
    try {
        var cbEstagios = dijit.byId(field);
        if (!hasValue(idEstagio) && hasValue(cbEstagios))
            cbEstagios.reset();

        if (hasValue(idProduto)) {
            // Popula os estágios:
            dojo.xhr.get({
                url: Endereco() + "/api/Coordenacao/getEstagioOrdem?codP=" + idProduto + "&cd_estagio=" + idEstagio + "&tipoC=" + criterios,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEstagios) {
                try {
                    loadEstagiosFK(dataEstagios.retorno, field, idEstagio);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadProdutoFK(items, linkProduto, idProduto) {
    require(["dojo/store/Memory", "dojo/_base/array"],
	function (Memory, Array) {
	    try {
	        var itemsCb = [];
	        var cbProduto = dijit.byId(linkProduto);

	        Array.forEach(items, function (value, i) {
	            itemsCb.push({ id: value.cd_produto, name: value.no_produto });
	        });
	        var stateStore = new Memory({
	            data: itemsCb
	        });
	        cbProduto.store = stateStore;
	        if (hasValue(idProduto)) {
	            cbProduto._onChangeActive = false;
	            cbProduto.set("value", idProduto);
	            cbProduto._onChangeActive = true;
	        }
	    }
	    catch (e) {
	        postGerarLog(e);
	    }
	});
}

function loadNivelFK(items, linkNivel, idNivel) {
    require(["dojo/store/Memory", "dojo/_base/array"],
	function (Memory, Array) {
	    try {
	        var itemsCb = [];
	        var cbNivel = dijit.byId(linkNivel);

	        Array.forEach(items, function (value, i) {
	            itemsCb.push({ id: value.cd_nivel, name: value.dc_nivel });
	        });
	        var stateStore = new Memory({
	            data: itemsCb
	        });
	        cbNivel.store = stateStore;
	        if (hasValue(idNivel)) {
	            cbNivel._onChangeActive = false;
	            cbNivel.set("value", idNivel);
	            cbNivel._onChangeActive = true;
	        }
	    }
	    catch (e) {
	        postGerarLog(e);
	    }
	});
}

function loadEstagiosFK(items, link, idEstagio) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try {
		        var itemsCb = [];
		        var cbEstagios = dijit.byId(link);

		        Array.forEach(items, function (value, i) {
		            itemsCb.push({ id: value.cd_estagio, name: value.no_estagio });
		        });
		        var stateStore = new Memory({
		            data: itemsCb
		        });
		        cbEstagios.store = stateStore;
		        if (hasValue(idEstagio))
		            cbEstagios.set("value", idEstagio);
		    }
		    catch (e) {
		        postGerarLog(e);
		    }
		})
}

function loadModalidadesFK(items, link, idModalidade) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try {
		        var itemsCb = [];
		        var cbModalidade = dijit.byId(link);

		        Array.forEach(items, function (value, i) {
		            itemsCb.push({ id: value.cd_modalidade, name: value.no_modalidade });
		        });
		        var stateStore = new Memory({
		            data: itemsCb
		        });
		        cbModalidade.store = stateStore;
		        if (hasValue(idModalidade))
		            cbModalidade.set("value", idModalidade);
		    }
		    catch (e) {
		        postGerarLog(e);
		    }
		});
}

function pesquisarCursoFK() {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            if (!hasValue(dijit.byId("cbPesqProdutoFK").value) && hasValue(dijit.byId("cbPesqProdutoFK").produto_selecionado))
                dijit.byId("cbPesqProdutoFK").set("value", dijit.byId("cbPesqProdutoFK").produto_selecionado);
            var myStore = Cache(
                    JsonRest({
                        target: !hasValue(document.getElementById("descCursoFK").value) ? Endereco() + "/api/curso/getCursoSearch?desc=&inicio=" + document.getElementById("inicioCursoFK").checked + "&status=" + retornaStatus("statusCursoFK") + "&produto=" + dijit.byId('cbPesqProdutoFK').value + "&estagio=" + dijit.byId('cbPesqEstagiosFK').value + "&modalidade=" + dijit.byId('cbPesqModalidadesFK').value + "&nivel=" + dijit.byId('cbPesqNivel').value : Endereco() + "/api/curso/getCursoSearch?desc=" + encodeURIComponent(document.getElementById("descCursoFK").value) + "&inicio=" + document.getElementById("inicioCursoFK").checked + "&status=" + retornaStatus("statusCursoFK") + "&produto=" + dijit.byId('cbPesqProdutoFK').value + "&estagio=" + dijit.byId('cbPesqEstagiosFK').value + "&modalidade=" + dijit.byId('cbPesqModalidadesFK').value + "&nivel=" + dijit.byId('cbPesqNivel').value,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaCurso");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function formatCheckBoxCursoFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridPesquisaCurso';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosCursoFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_curso", grid._by_idx[rowIndex].item.cd_curso);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBoxCursoFK'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_curso', 'selecionadoCursoFK', -1, 'selecionaTodosCursoFK', 'selecionaTodosCursoFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_curso', 'selecionadoCursoFK', " + rowIndex + ", '" + id + "', 'selecionaTodosCursoFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPesquisaCursoFK(limparGrade) {
    require(["dojo/store/Memory"],
function (Memory) {
    try {
        var gridRelacionamentos = dijit.byId("gridRelacionamentos");
        if (hasValue(gridRelacionamentos))
            gridRelacionamentos.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));

        var gridCursoFKLimp = dijit.byId("gridPesquisaCurso");
        dojo.byId("descCursoFK").value = "";
        dijit.byId("cbPesqProdutoFK").reset();
        dijit.byId("cbPesqEstagiosFK").reset();
        dijit.byId("cbPesqModalidadesFK").reset();
        if (hasValue(dijit.byId("statusCursoFK"))) {
            dijit.byId("statusCursoFK").reset();
        }
        
        dijit.byId("inicioCursoFK").reset();
        if (hasValue(gridCursoFKLimp) && hasValue(gridCursoFKLimp.itensSelecionados))
            gridCursoFKLimp.itensSelecionados = [];
        // dijit.byId("gridPesquisaCurso").update();
        if (limparGrade)
            gridCursoFKLimp.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));
    }
    catch (e) {
        postGerarLog(e);
    }
});
}