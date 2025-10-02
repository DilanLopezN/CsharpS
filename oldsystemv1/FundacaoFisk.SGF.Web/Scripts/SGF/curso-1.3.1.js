var CONSULTA_PRODUTO_HAS_ATIVO = 0;
var CONSULTA_PRODUTO_HAS_ESTAGIO = 3;
var CONSULTA_PRODUTO_HAS_CURSO = 4;
var CONSULTA_ESTAGIO_HAS_ATIVO = 1;
var CONSULTA_ESTAGIO_HAS_CURSO = 2;
var CONSULTA_MODALIDADE_HAS_CURSO = 2;
var CONSULTA_MODALIDADE_HAS_ATIVO = 1;
var CONSULTA_NIVEL_HAS_ATIVO = 0;
var TIPOMOVIMENTO = null;
var MATERIALDIDATICO = 1;

//Pega os Antigos dados do Formulário
function keepValues(tipoForm, grid, ehLink) {
    try{
        apresentaMensagem('apresentadorMensagemCurso', null);
        var value = grid.itemSelecionado;
        var linkAnterior = document.getElementById('link');
        var gridMatDid = dijit.byId('gridMaterialDidatico');

        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        if (hasValue(value)) {
            dojo.byId("cd_curso").value = value.cd_curso;
            dojo.byId("nomeCurso").value = hasValue(value.no_curso) ? value.no_curso : "";
            dojo.byId("ckCursoAtivo").value = value.id_curso_ativo == true ? dijit.byId("ckCursoAtivo").set("value", true) : dijit.byId("ckCursoAtivo").set("value", false);
            dojo.byId("ckMatricula").value = value.id_permitir_matricula == true ? dijit.byId("ckMatricula").set("value", true) : dijit.byId("ckMatricula").set("value", false);

            dijit.byId('cbProduto')._onChangeActive = false;
            dijit.byId('cbProduto').set('value', value.cd_produto);
            dijit.byId('cbProduto')._onChangeActive = true;

            populaProdutos(value.cd_produto, 'cbProduto', CONSULTA_PRODUTO_HAS_ESTAGIO);
            populaNiveis(value.cd_nivel,'cbNivel', CONSULTA_NIVEL_HAS_ATIVO);
            populaEstagios(value.cd_produto, value.cd_estagio, "cbEstagios", CONSULTA_ESTAGIO_HAS_ATIVO);

            dijit.byId('cbModalidades').set('value', value.cd_modalidade);
            dojo.byId("cargaHorariaTotal").value = hasValue(value.nm_carga_horaria) ? value.nm_carga_horaria : "";
            dojo.byId("cargaHorariaMaxima").value = hasValue(value.nm_carga_maxima) ? value.nm_carga_maxima : "";
            dojo.byId("vagas").value = hasValue(value.nm_vagas_curso) ? value.nm_vagas_curso : "";
            dojo.byId("pcCriterioAprovacao").value = hasValue(value.pc_criterio_aprovacao) ? value.pc_criterio_aprovacao : "";
            dojo.byId("totalNota").value = hasValue(value.nm_total_nota) ? value.nm_total_nota : "";
            dojo.byId("faixaMinima").value = hasValue(value.nm_faixa_etaria_minima) ? value.nm_faixa_etaria_minima : "";
            dojo.byId("faixaMaxima").value = hasValue(value.nm_faixa_etaria_maxima) ? value.nm_faixa_etaria_maxima : "";

            populaProximosCursos(value.cd_proximo_curso);
            populaModalidades(value.cd_modalidade, 'cbModalidades', CONSULTA_MODALIDADE_HAS_ATIVO);
            dijit.byId('cbProximoCurso').set('value', value.cd_proximo_curso);

            dijit.byId('ckCertificado').value = value.id_certificado == true ? dijit.byId("ckCertificado").set("value", true) : dijit.byId("ckCertificado").set("value", false);

            pesquisarMatDid();
            //gridMatDid.update();
        }
        else
            // Seleciona os materiais didáticos:
            configuraMateriaisDidaticos(gridMatDid);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function sugereNomeEstagio(campo) {
    try{
        if (!hasValue(document.getElementById('cd_curso').value) && !hasValue(dojo.byId('nomeCurso').value)) // Se trata de uma inserção.
            dojo.byId('nomeCurso').value = campo.displayedValue;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraMateriaisDidaticos(gridMatDid) {
    require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/ready"], function (dom, xhr, ref, ready) {
        ready(function () {
            xhr.post(Endereco() + "/financeiro/linkItensRetorno", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    TipoRetorno: null,
                    Selecionados: null,
                    DadosRetorno: null
                })
            }).then(function (data) {
                try{
                    var gridName = 'gridCurso';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagemCurso', data);
                    data = data.retorno;

                    if (hasValue(data) && hasValue(data.Selecionados)) {
                        var dadosRetorno = data.DadosRetorno;
                        var itemOriginal = data.ItemOriginal;

                        //Seleciona o item na grade, para quando cancelar ele voltar para o original:
                        if(hasValue(itemOriginal))
                            grid.itemSelecionado = itemOriginal;

                        if (hasValue(dadosRetorno)) {
                            //Restaura os dados antigos da edição de grupo, antes de clicar no link:
                            dojo.byId('cd_curso').value = dadosRetorno.cd_curso;
                            dom.byId("nomeCurso").value = hasValue(dadosRetorno.no_curso) ? dadosRetorno.no_curso: "";
                            dom.byId("cargaHorariaTotal").value = hasValue(dadosRetorno.nm_carga_horaria) ? dadosRetorno.nm_carga_horaria: "";
                            dom.byId("cargaHorariaMaxima").value = hasValue(dadosRetorno.nm_carga_maxima) ? dadosRetorno.nm_carga_maxima: "";
                            dom.byId("vagas").value = hasValue(dadosRetorno.nm_vagas_curso) ? dadosRetorno.nm_vagas_curso: "";
                            dom.byId("pcCriterioAprovacao").value = hasValue(dadosRetorno.pc_criterio_aprovacao)? dadosRetorno.pc_criterio_aprovacao: "";
                            dom.byId("totalNota").value = hasValue(dadosRetorno.nm_total_nota) ? dadosRetorno.nm_total_nota : "";
                            dom.byId("faixaMinima").value = hasValue(dadosRetorno.nm_faixa_etaria_minima) ? dadosRetorno.nm_faixa_etaria_minima: "";
                            dom.byId("faixaMaxima").value = hasValue(dadosRetorno.nm_faixa_etaria_maxima) ? dadosRetorno.nm_faixa_etaria_maxima : "";
                            dijit.byId("ckCursoAtivo").set('value', dadosRetorno.id_curso_ativo);
                            dijit.byId("ckMatricula").set('value', dadosRetorno.id_permitir_matricula);

                            populaProdutos(dadosRetorno.cd_produto, 'cbProduto', CONSULTA_PRODUTO_HAS_ESTAGIO);
                            dijit.byId('cbProduto')._onChangeActive = false;
                            dijit.byId('cbProduto').set('value', dadosRetorno.cd_produto);
                            dijit.byId('cbProduto')._onChangeActive = true;

                            populaEstagios(dadosRetorno.cd_produto, dadosRetorno.cd_estagio, "cbEstagios", CONSULTA_ESTAGIO_HAS_ATIVO);
                            dijit.byId('cbEstagios').set('value', dadosRetorno.cd_estagio);

                            populaModalidades(dadosRetorno.cd_modalidade, 'cbModalidades', CONSULTA_MODALIDADE_HAS_ATIVO);
                            dijit.byId('cbModalidades').set('value', dadosRetorno.cd_modalidade);

                            populaProximosCursos(dadosRetorno.cd_proximo_curso);
                            dijit.byId('cbProximoCurso').set('value', dadosRetorno.cd_proximo_curso);
                        }
                        else {
                            populaProdutos(null, 'cbProduto', CONSULTA_PRODUTO_HAS_ESTAGIO);
                            populaModalidades(null, 'cbModalidades', CONSULTA_MODALIDADE_HAS_ATIVO);
                            populaProximosCursos(null);
                        }

                        // Aplica os itens do link:
                        //gridMatDid.itensSelecionados = data.Selecionados;

                        for (var i = 0; i < data.Selecionados.length; i++)
                            gridMatDid.store.newItem(data.Selecionados[i]);
                    }
                    else {
                        populaProdutos(null, 'cbProduto', CONSULTA_PRODUTO_HAS_ESTAGIO);
                        populaModalidades(null, 'cbModalidades', CONSULTA_MODALIDADE_HAS_ATIVO);
                        populaProximosCursos(null);

                        gridMatDid.itensSelecionados = [];
                        /*for (var i = 0; i < value.MateriaisDidaticos.length; i++)
                            gridMatDid.itensSelecionados.push(value.MateriaisDidaticos[i]);*/
                    }
                    quickSortObj(gridMatDid._by_idx, 'cd_item_curso');
                    quickSortObj(gridMatDid.itensSelecionados, 'cd_item_curso');
                    gridMatDid.store.save();
                    //gridMatDid.update();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                apresentaMensagem('apresentadorMensagemCurso', error.response.data);
            });
        });
    });
}

function formatCheckBoxMatDid(value, rowIndex, obj) {
    try{
        var gridName = 'gridMaterialDidatico';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMatDid');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_item_curso", grid._by_idx[rowIndex].item.cd_item_curso);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:10px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_item_curso', 'selecionadoMatDid', -1, 'selecionaTodosMatDid', 'selecionaTodosMatDid', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_item_curso', 'selecionadoMatDid', " + rowIndex + ", '" + id + "', 'selecionaTodosMatDid', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxCurso(value, rowIndex, obj) {
    try{
        var gridName = 'gridCurso';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosCurso');

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
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxPpt(value, rowIndex, obj) {
    try {
        var gridName = 'gridMaterialDidatico';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        //var teste = "configuraCheckBoxPpt(" + value + ", '" + id + "', '" + gridName +  "'," +rowIndex + ")";
        setTimeout("configuraCheckBoxPpt(" + value + ", '" + id + "', '" + gridName +  "'," +rowIndex + ")", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxPpt(value, id, gridName, rowIndex) {
    require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
        ready(function () {
            try {
                var dojoId = dojo.byId(id);
                var grid = dijit.byId(gridName);


                if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
                    dijit.byId(id).destroy();
                if (value == undefined)
                    value = false;
                if (hasValue(dojoId) && dojoId.type == 'text')
                    var checkBox = new dijit.form.CheckBox({
                        name: "checkBox",
                        checked: value,
                        disabled: false,
                        onChange: function (marcou) { atualizarPptMaterial(grid, rowIndex, marcou) }
                    }, id);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function montarCadastroCurso() {
    //Criação da Grade
    require([
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
        "dojo/dom"
    ], function (EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom) {
        ready(function () {
            try{
                //Para alinhar o painel do dojo, verificar se isso ocorre no desenvolvimento:
                dojo.byId('tabContainer_tablist').children[3].children[0].style.width = '100%';
                // Corrige o tamanho do pane que o dojo cria para o dialog com scroll no ie7:
                if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
                    var ieversion = new Number(RegExp.$1)
                    if (ieversion == 7)
                        // Para IE7
                        dojo.byId('cad').childNodes[1].style.height = '100%';
                }

                //Popula o filtro de pesquisa de produtos:
                populaProdutos(null, 'cbPesqProduto', CONSULTA_PRODUTO_HAS_CURSO);
                populaNiveis(null, 'cbPesqNivel', CONSULTA_NIVEL_HAS_ATIVO);
                populaModalidades(null, 'cbPesqModalidades', CONSULTA_MODALIDADE_HAS_CURSO);
                var myStore = Cache(
                    JsonRest({
                        target: !hasValue(document.getElementById("descCurso").value) ? Endereco() + "/api/curso/getCursoSearch?desc=&inicio=" + document.getElementById("inicioCurso").checked + "&status=1" + "&produto=" + dijit.byId('cbPesqProduto').value + "&estagio=" + dijit.byId('cbPesqEstagios').value + "&modalidade=" + dijit.byId('cbPesqModalidades').value + "&nivel=" + dijit.byId('cbPesqNivel').value : Endereco() + "/api/curso/getCursoSearch?desc=" + encodeURIComponent(document.getElementById("descCurso").value) + "&inicio=" + document.getElementById("inicioCurso").checked + "&status=1" + "&produto=" + dijit.byId('cbPesqProduto').value + "&estagio=" + dijit.byId('cbPesqEstagios').value + "&modalidade=" + dijit.byId('cbPesqModalidades').value + "&nivel=" + dijit.byId('cbPesqNivel').value,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                ), Memory({}));

                //*** Cria a grade de Cursos **\\
                var gridCurso = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosCurso' style='display:none'/>", field: "selecionadoCurso", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCurso },
                        //{ name: "Código", field: "cd_curso", width: "75px", styles: "width:75px; text-align: left;" },
                        { name: "Nome", field: "no_curso", width: "25%", styles: "min-width:80px;" },
                        { name: "Produto", field: "no_produto", width: "20%", styles: "min-width:80px;" },
                        { name: "Estágio", field: "no_estagio", width: "20%", styles: "min-width:80px;" },
                        { name: "Série", field: "no_modalidade", width: "20%", styles: "min-width:80px;" },
                        { name: "Nível", field: "dc_nivel", width: "20%", styles: "min-width:80px;" },
                        { name: "Ativo", field: "curso_ativo", width: "10%", styles: "text-align: center; min-width:40px; max-width: 50px;" }
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
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridCurso");
                gridCurso.canSort = function (col) { return Math.abs(col) != 1 };
                gridCurso.startup();
                gridCurso.on("RowDblClick", function (evt) {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        limparCurso();
                        dijit.byId("criterioAprovacao").set("open", false);
                        dijit.byId("faixaEtaria").set("open", false);
                        dijit.byId("material").set("open", false);
                        apresentaMensagem('apresentadorMensagem', '');
                        gridCurso.itemSelecionado = montaObjetoCurso(item);
                        keepValues(null, gridCurso, false);
                        dijit.byId("cad").show();
                        IncluirAlterar(0, 'divAlterarCurso', 'divIncluirCurso', 'divExcluirCurso', 'apresentadorMensagemCurso', 'divCancelarCurso', 'divLimparCurso');
                        dijit.byId('tabContainer').resize();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                IncluirAlterar(1, 'divAlterarCurso', 'divIncluirCurso', 'divExcluirCurso', 'apresentadorMensagemCurso', 'divCancelarCurso', 'divLimparCurso');

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridCurso, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosCurso').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_curso', 'selecionadoCurso', -1, 'selecionaTodosCurso', 'selecionaTodosCurso', 'gridCurso')", gridCurso.rowsPerPage * 3);
                    });
                });

                if (!hasValue(gridCurso.itensSelecionados))
                    gridCurso.itensSelecionados = [];

                //myStore = Cache(
                //    JsonRest({
                //        target: "/api/financeiro/getItemCurso?cdCurso=" + dojo.byId("cd_curso").value,
                //        handleAs: "json",
                //        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                //    }
                //), Memory({}));
                var data = new Array();

                var gridMaterialDidatico = new EnhancedGrid({
                    //store: ObjectStore({ objectStore: myStore }),
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data, idProperty: "cd_item" }) }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosMatDid' style='display:none'/>", field: "selecionadoMatDid", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMatDid },
                        { name: "Grupo", field: "no_grupo_estoque", width: "15%"},
                        { name: "Descrição", field: "no_item", width: "68%" },
                            { name: "Personalizadas", field: "id_ppt", width: "17%", styles: "text-align:center;", formatter: formatCheckBoxPpt }
                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridMaterialDidatico");
                gridMaterialDidatico.canSort = function (col) {  };
                gridMaterialDidatico.startup();

                if (!hasValue(gridMaterialDidatico.itensSelecionados))
                    gridMaterialDidatico.itensSelecionados = [];

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridMaterialDidatico, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosMatDid').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_item', 'selecionadoMatDid', -1, 'selecionaTodosMatDid', 'selecionaTodosMatDid', 'gridMaterialDidatico')", gridMaterialDidatico.rowsPerPage * 3);
                    });
                });

                var params = getParamterosURL();

                // Verifica pelo parametro get da URL se tem um link originado pela edição de aluno (curso pretendido):
                if (hasValue(params["tipoRetornoLinkAluno"]))
                    configuraCursosPretendidos(gridCurso);
                else
                    // Verifica pelo parametro get da URL se tem um link originado pela pesquisa ou edição de curso:
                    if (hasValue(params["tipoRetorno"])) {
                        limparCurso();
                        apresentaMensagem('apresentadorMensagem', null);
                        dijit.byId("cad").show();
                        if (params["tipoRetorno"] == TIPO_RETORNO_LINK_INCLUSAO)
                            configuraMateriaisDidaticos(gridMaterialDidatico);
                        else if (params["tipoRetorno"] == TIPO_RETORNO_LINK_EDICAO) {
                            // Edita o item:
                            keepValues(null, gridCurso, false);
                            IncluirAlterar(0, 'divAlterarCurso', 'divIncluirCurso', 'divExcluirCurso', 'apresentadorMensagemCurso', 'divCancelarCurso', 'divLimparCurso');
                        }
                        dijit.byId('tabContainer').resize();
                    }

                dijit.byId("faixaMinima").on("change", function (e) {
                    if (hasValue(e) && hasValue(dijit.byId("faixaMaxima").value)) {
                        if (parseInt(e) > parseInt(dijit.byId("faixaMaxima").value)) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroFaixaEtaria);
                            apresentaMensagem("apresentadorMensagemCurso", mensagensWeb);
                            return false;
                        }
                    }
                });

                dijit.byId("faixaMaxima").on("change", function (e) {
                    if (hasValue(e) && hasValue(dijit.byId("faixaMinima").value)) {
                        if (parseInt(e) < parseInt(dijit.byId("faixaMinima").value)) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroFaixaEtaria);
                            apresentaMensagem("apresentadorMensagemCurso", mensagensWeb);
                            return false;
                        }
                    }
                });
                //*** Cria os botões do link de ações **\\
                // Adiciona link de selecionados Curso:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridCurso, 'todosItensCurso', ['pesquisarCurso', 'relatorioCurso']);
                        pesquisarCurso(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridCurso', 'selecionadoCurso', 'cd_curso', 'selecionaTodosCurso', ['pesquisarCurso', 'relatorioCurso'], 'todosItensCurso'); }
                });
                menu.addChild(menuItensSelecionados);

                button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensCurso",
                    dropDown: menu,
                    id: "todosItensCurso"
                });
                dom.byId("linkSelecionadosCurso").appendChild(button.domNode);

                // Adiciona link de ações:
                var menuMatDid = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_item', dijit.byId("gridMaterialDidatico")); }
                });
                menuMatDid.addChild(acaoExcluir);

                //var acaoMatDidD = new MenuItem({
                //    label: "Material Didático",
                //    onClick: function () { redirecionaMaterialDidatico(document.getElementById('divIncluirCurso').style.display == "" ? TIPO_RETORNO_LINK_INCLUSAO : TIPO_RETORNO_LINK_EDICAO); }
                //});
                //menuMatDid.addChild(acaoMatDidD);

                menu = new DropDownMenu({ style: "height: 25px" });
                /*var acaoMatDid = new MenuItem({
                    label: "Material Didático",
                    onClick: function () { redirecionaMaterialDidatico(TIPO_RETORNO_LINK_PESQUISA) }
                });
                menu.addChild(acaoMatDid);*/

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasMatDid",
                    dropDown: menuMatDid,
                    id: "acoesRelacionadasMatDid"
                });
                dom.byId("linkAcoesMatDid").appendChild(button.domNode);

                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditar(gridCurso.itensSelecionados);
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
                        eventoRemover(gridCurso.itensSelecionados, 'deletarCurso(itensSelecionados)');
                    }
                });
                menu.addChild(acaoRemover);

                // Inclui o link de Aluno (retorno do link de cursos pretendidos):
                var parametros = getParamterosURL();

                if (hasValue(parametros['tipoRetornoLinkAluno'])) {
                    var acaoAluno = new MenuItem({
                        label: "Aluno",
                        onClick: function () { redirecionaAluno(true); }
                    });
                    menu.addChild(acaoAluno);
                }

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasCurso",
                    dropDown: menu,
                    id: "acoesRelacionadasCurso"
                });
                dom.byId("linkAcoesCurso").appendChild(button.domNode);

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar", iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                    onClick: function () { incluirCurso(); }
                }, "incluirCurso");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () { alterarCurso(); }
                }, "alterarCurso");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarCurso() }); }
                }, "deleteCurso");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () { limparCurso(); }
                }, "limparCurso");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () { keepValues(null, gridCurso, null) }
                }, "cancelarCurso");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("cad").hide(); }
                }, "fecharCurso");
                new Button({
                    label: "",
                    onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarCurso(true); },
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "pesquisarCurso");
                decreaseBtn(document.getElementById("pesquisarCurso"), '32px');

                new Button({
                    label: "",
                    onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarMatDid(true); },
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF'
                }, "pesquisarMat");
                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try{
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            dijit.byId("criterioAprovacao").set("open", false);
                            dijit.byId("faixaEtaria").set("open", false);
                            dijit.byId("material").set("open", false);
                            setTimeout('limparCurso()', 10);
                            dijit.byId("cad").show();
                            dijit.byId('tabContainer').resize();
                            gridCurso.itemSelecionado = null;
                            apresentaMensagem('apresentadorMensagem', null);
                            populaProdutos(null, 'cbProduto', CONSULTA_PRODUTO_HAS_ESTAGIO);
                            populaNiveis(null, 'cbNivel',CONSULTA_NIVEL_HAS_ATIVO);
                            populaProximosCursos(null);
                            populaModalidades(null, 'cbModalidades', CONSULTA_MODALIDADE_HAS_ATIVO);
                            IncluirAlterar(1, 'divAlterarCurso', 'divIncluirCurso', 'divExcluirCurso', 'apresentadorMensagemCurso', 'divCancelarCurso', 'divLimparCurso');
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoCurso");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        require(["dojo/_base/xhr"], function (xhr) {
                            xhr.get({
                                url: !hasValue(document.getElementById("descCurso").value) ? Endereco() + "/api/curso/geturlrelatoriocurso?" + getStrGridParameters('gridCurso') + "desc=&inicio=" + document.getElementById("inicioCurso").checked + "&status=" + retornaStatus("statusCurso") : Endereco() + "/api/curso/geturlrelatoriocurso?" + getStrGridParameters('gridCurso') + "desc=" + encodeURIComponent(document.getElementById("descCurso").value) + "&inicio=" + document.getElementById("inicioCurso").checked + "&status=" + retornaStatus("statusCurso"),
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        })
                    }
                }, "relatorioCurso");

                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("pesquisarItemFK")))
                                montargridPesquisaItem(function () {
                                    abrirItemFK();
                                    dojo.query("#pesquisaItemServico").on("keyup", function (e) { if (e.keyCode == 13) pesquisarItemFK(MATERIALDIDATICO); });
                                    dijit.byId("pesquisarItemFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                        pesquisarItemFK(MATERIALDIDATICO);
                                    });
                                    dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("proItem").hide(); });
                                }, true, true);
                            else
                                abrirItemFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirItemFK");

                var buttonFkArray = ['pesquisarMat'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }

                montarStatus("statusCurso");
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323018', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['descCurso', 'cbPesqProduto', 'cbPesqEstagios', 'cbPesqModalidades', 'statusCurso'], 'pesquisarCurso', ready);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}
function retornarItemFK() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try{
                var gridMaterialDidatico = dijit.byId("gridMaterialDidatico");
                var gridPesquisaItem = dijit.byId("gridPesquisaItem");
                var value = gridPesquisaItem.selection.getSelected();
                var cdCurso = dojo.byId("cd_curso").value;

                if (!hasValue(value) && (!hasValue(gridPesquisaItem.itensSelecionados) || gridPesquisaItem.itensSelecionados.length <= 0))
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                else {
                    var storeGridItem = (hasValue(gridMaterialDidatico) && hasValue(gridMaterialDidatico.store.objectStore.data)) ? gridMaterialDidatico.store.objectStore.data : [];
                    if (storeGridItem != null && storeGridItem.length > 0) {
                        if (hasValue(gridPesquisaItem.itensSelecionados))
                            $.each(gridPesquisaItem.itensSelecionados, function (idx, value) {
                                //gridMaterialDidatico.store.newItem(value);
                                insertObjSort(gridMaterialDidatico.store.objectStore.data, "cd_item_curso", { cd_item: value.cd_item, no_grupo_estoque: value.no_grupo_estoque, no_item: value.no_item, cd_curso: cdCurso, cd_item_curso: 0 });
                            });
                        else
                            insertObjSort(gridMaterialDidatico.store.objectStore.data, "cd_item_curso", { cd_item: value[0].cd_item, no_grupo_estoque: value[0].no_grupo_estoque, no_item: value[0].no_item, cd_curso: cdCurso, cd_item_curso: 0 });
                        gridMaterialDidatico.setStore(new ObjectStore({ objectStore: new Memory({ data: gridMaterialDidatico.store.objectStore.data }) }));
                        //gridMaterialDidatico.update();
                    } else {
                        var dados = [];
                        if (hasValue(gridPesquisaItem.itensSelecionados))
                            $.each(gridPesquisaItem.itensSelecionados, function (index, val) {
                                insertObjSort(dados, "cd_item_curso", { cd_item: val.cd_item, no_grupo_estoque: val.no_grupo_estoque, no_item: val.no_item, cd_curso: cdCurso, cd_item_curso: 0 });
                            });
                        else
                            insertObjSort(gridMaterialDidatico.store.objectStore.data, "cd_item_curso", { cd_item: value[0].cd_item, no_grupo_estoque: value[0].no_grupo_estoque, no_item: value[0].no_item, cd_curso: cdCurso, cd_item_curso: 0 });
                        //slice(0)
                        if(hasValue(dados))
                            gridMaterialDidatico.setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));
                        else
                            gridMaterialDidatico.setStore(new ObjectStore({ objectStore: new Memory({ data: gridMaterialDidatico.store.objectStore.data }) }));

                    }
                    dijit.byId("proItem").hide();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function redirecionaAluno(tipoRetorno) {
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        try{
            var selecionados = [];
            var selecionadosRetorno = [];
            var gridCurso = dijit.byId('gridCurso');
            var itensSelecionados = dijit.byId('gridCurso').itensSelecionados;

            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Selecione algum curso para relacioná-lo ao aluno como curso pretendido.');
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }

            if (hasValue(gridCurso.itensSelecionadosLink) && gridCurso.itensSelecionadosLink.length > 0) {
                selecionados = gridCurso.itensSelecionadosLink;
                for (var i = 0; i < selecionados.length; i++) {
                    var itemRetorno = {
                        cd_curso: selecionados[i].cd_curso,
                        no_curso: selecionados[i].no_curso,
                        no_produto: selecionados[i].no_produto
                    }
                    selecionadosRetorno.push(itemRetorno);
                }
            }

            for (var i = 0; i < itensSelecionados.length; i++) {
                var novoItem = {
                    cd_curso: itensSelecionados[i].cd_curso,
                    no_curso: itensSelecionados[i].no_curso,
                    no_produto: itensSelecionados[i].no_produto
                };
                //selecionados.push(novoItem);
                insertObjSort(selecionadosRetorno, 'cd_curso', novoItem, false);
            }
            xhr.post(Endereco() + "/aluno/linkCursoRetorno", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    TipoRetorno: tipoRetorno,
                    Selecionados: selecionadosRetorno,
                    DadosRetorno: null
                })
            }).then(function (data) {
                if (hasValue(data.retorno))
                    window.location = data.retorno;
                else
                    apresentaMensagem('apresentadorMensagem', data);
            }, function (error) {
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function configuraCursosPretendidos(gridCurso) { //Método de link proveniente de aluno - cursos pretendidos.
    require(["dojo/request/xhr", "dojox/json/ref", "dojo/ready"], function (xhr, ref, ready) {
        ready(function () {
            try{
                var parametros = getParamterosURL();
                var link = {
                    TipoRetorno: parametros['tipoRetornoLinkAluno'],
                    Selecionados: null,
                    DadosRetorno: null
                };
                var parametroLink = '';

                if (hasValue(parametros['tipoRetornoLinkAluno']))
                    parametroLink = '?tipoRetornoLinkAluno=' + parametros['tipoRetornoLinkAluno'];

                xhr.post(Endereco() + "/aluno/linkCurso" + parametroLink, {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson({
                        linkCursosPretendidos: link
                    })
                }).then(function (data) {
                    data = data.retorno;
                    if (hasValue(data.Selecionados))
                        gridCurso.itensSelecionadosLink = data.Selecionados;
                    else
                        gridCurso.itensSelecionadosLink = [];

                    // Atualiza com timeout para funcionar no IE:
                    setTimeout('atualizaGrid("gridCurso")', 10);
                }, function (error) {
                    apresentaMensagem('apresentadorMensagem', error.response.data);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function atualizaGrid(gridName) {
    try{
        dijit.byId(gridName).update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function redirecionaMaterialDidatico(tipoRetorno) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try{
            var dadosRetorno = {
                cd_curso: dojo.byId('cd_curso').value,
                cd_proximo_curso: dijit.byId("cbProximoCurso").value,
                cd_produto: dijit.byId("cbProduto").value,
                cd_estagio: dijit.byId("cbEstagios").value,
                cd_modalidade: dijit.byId("cbModalidades").value,
                no_curso: dom.byId("nomeCurso").value,
                nm_carga_horaria: dom.byId("cargaHorariaTotal").value,
                nm_carga_maxima: dom.byId("cargaHorariaMaxima").value,
                nm_vagas_curso: dom.byId("vagas").value,
                id_curso_ativo: domAttr.get("ckCursoAtivo", "checked"),
                id_permitir_matricula: domAttr.get("ckMatricula", "checked"),
                pc_criterio_aprovacao: dom.byId("pcCriterioAprovacao").value,
                nm_total_nota: dom.byId("totalNota").value,
                nm_faixa_etaria_minima: dom.byId("faixaMinima").value,
                nm_faixa_etaria_maxima: dom.byId("faixaMaxima").value
            };

            var selecionados = [];
            var itensSelecionados = dijit.byId('gridMaterialDidatico').store.objectStore.data;
            var gridCurso = dijit.byId('gridCurso');
            var itemOriginal = null;

            if(hasValue(gridCurso.itemSelecionado))
                itemOriginal = montaObjetoCurso(gridCurso.itemSelecionado);

            for(var i=0; i<itensSelecionados.length; i++){
                var newSelecionado = {
                    cd_item: itensSelecionados[i].cd_item,
                    cd_grupo_estoque: itensSelecionados[i].cd_grupo_estoque,
                    cd_integracao: itensSelecionados[i].cd_integracao,
                    cd_origem_fiscal: itensSelecionados[i].cd_origem_fiscal,
                    cd_tipo_item: itensSelecionados[i].cd_tipo_item,
                    cd_classificacao_fiscal: itensSelecionados[i].cd_classificacao_fiscal,
                    dc_codigo_barra: itensSelecionados[i].dc_codigo_barra,
                    dc_sgl_item: itensSelecionados[i].dc_sgl_item,
                    id_item_ativo: itensSelecionados[i].id_item_ativo,
                    id_permitir_matricula: itensSelecionados[i].id_permitir_matricula,
                    no_item: itensSelecionados[i].no_item,
                    pc_aliquota_icms: itensSelecionados[i].pc_aliquota_icms,
                    pc_aliquota_iss: itensSelecionados[i].pc_aliquota_iss,
                    id_material_didatico: itensSelecionados[i].id_material_didatico
                }
                selecionados.push(newSelecionado);
            }

            xhr.post(Endereco() + "/financeiro/linkItens", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    DadosRetorno: dadosRetorno,
                    TipoRetorno: tipoRetorno,
                    Selecionados: selecionados,
                    ItemOriginal: itemOriginal
                })
            }).then(function (data) {
                window.location = data.retorno;
            }, function (error) {
                apresentaMensagem('apresentadorMensagemCurso', error.response.data);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montaObjetoCurso(item) {
    try{
        var objeto =
            {
                cd_curso: item.cd_curso,
                cd_estagio: item.cd_estagio,
                cd_modalidade: item.cd_modalidade,
                cd_produto: item.cd_produto,
                cd_proximo_curso: item.cd_proximo_curso,
                curso_ativo: item.curso_ativo,
                id_curso_ativo: item.id_curso_ativo,
                id_permitir_matricula: item.id_permitir_matricula,
                nm_carga_horaria: item.nm_carga_horaria,
                nm_carga_maxima: item.nm_carga_maxima,
                nm_faixa_etaria_maxima: item.nm_faixa_etaria_maxima,
                nm_faixa_etaria_minima: item.nm_faixa_etaria_minima,
                nm_total_nota: item.nm_total_nota,
                nm_vagas_curso: item.nm_vagas_curso,
                no_curso: item.no_curso,
                no_estagio: item.no_estagio,
                no_modalidade: item.no_modalidade,
                no_produto: item.no_produto,
                dc_nivel: item.dc_nivel,
                cd_nivel: item.cd_nivel,
                pc_criterio_aprovacao: item.pc_criterio_aprovacao,
				selecionadoCurso: item.selecionadoCurso,
                id_certificado: item.id_certificado

            };
        return objeto;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadProduto(items, linkProduto, idProduto) {
    require(["dojo/store/Memory", "dojo/_base/array"],
	function (Memory, Array) {
        try{
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


function loadNivel(items, linkNivel, idNivel) {
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


function populaProximosCursos(idProximoCurso) {
    try{
        // Popula os próximos cursos:
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/curso/getCursos?cd_curso="+idProximoCurso,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataCursosAtivos) {
                loadCursos(dataCursosAtivos.retorno, 'cbProximoCurso');
                if(hasValue(idProximoCurso))
                    dijit.byId("cbProximoCurso").set("value", idProximoCurso);
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaModalidades(idModalidade, field, criterios) {
    try{
        // Popula as modalidades:
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/Coordenacao/getModalidades?criterios=" + criterios + "&cd_modalidade="+ idModalidade,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataModAtiva) {
                loadModalidades(jQuery.parseJSON(dataModAtiva).retorno, field, idModalidade);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaProdutos(idProduto, field, criterios) {
    try{
        // Popula os produtos:
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + criterios + "&cd_produto=" + idProduto,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataProdAtivo) {
                loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, field, idProduto);
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}


function populaNiveis(idNivel,field,criterios) {
    try {
        // Popula os niveis:
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/Coordenacao/getNivel?hasDependente=" + criterios,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataNivdAtivo) {
                loadNivel(jQuery.parseJSON(dataNivdAtivo).retorno, field, idNivel);
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaEstagios(idProduto, idEstagio, field, criterios) {
    try{
        var cbEstagios = dijit.byId(field);
        if(!hasValue(idEstagio) && hasValue(cbEstagios))
            cbEstagios.reset();

        if (hasValue(idProduto)) {
            // Popula os estágios:
            require(["dojo/_base/xhr"], function (xhr) {
                xhr.get({
                    url: Endereco() + "/api/Coordenacao/getEstagioOrdem?codP=" + idProduto + "&cd_estagio=" + idEstagio + "&tipoC=" + criterios,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dataEstagios) {
                    loadEstagios(dataEstagios.retorno, field, idEstagio);
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagemCurso', error);
                });
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadEstagios(items, link, idEstagio) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
            try{
		        var itemsCb = [];
		        var cbEstagios =  dijit.byId(link);

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

function loadModalidades(items, link, idModalidade) {
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

function loadCursos(items, link) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
            try{
		        var itemsCb = [];
		        Array.forEach(items, function (value, i) {
		            itemsCb.push({ id: value.cd_curso, name: value.no_curso });
		        });
		        var stateStore = new Memory({
		            data: itemsCb
		        });
		        dijit.byId(link).store = stateStore;
            }
		    catch (e) {
		        postGerarLog(e);
		    }
		})
}

function limparCurso() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
    function (Memory, ObjectStore) {
        try{
            var gridMatDid = dijit.byId('gridMaterialDidatico');
            gridMatDid.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
            //dijit.byId("gridCursoAval")._by_idx = [];
            apresentaMensagem('apresentadorMensagemCurso', null);
            getLimpar('#formCurso');
            clearForm('formCurso');
            //IncluirAlterar(1, 'divAlterarCurso', 'divIncluirCurso', 'divExcluirCurso', 'apresentadorMensagemCurso', 'divCancelarCurso', 'divLimparCurso');
            document.getElementById("cd_curso").value = '';
            gridMatDid.update();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function alterarCurso() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemCurso', null);
        if (!dijit.byId("formCurso").validate())
            return false;

        if (hasValue(dijit.byId("faixaMaxima").value) && hasValue(dijit.byId("faixaMinima").value) && parseInt(dijit.byId("faixaMaxima").value) < parseInt(dijit.byId("faixaMinima").value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroFaixaEtaria);
            apresentaMensagem("apresentadorMensagemCurso", mensagensWeb);
            return false;
        }
        var idPPT = 0;
        var Material = dijit.byId('gridMaterialDidatico').store.objectStore.data;
        if (Material.length > 2) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Somente podem ser escolhidos 2 materiais didáticos no máximo');
            apresentaMensagem("apresentadorMensagemCurso", mensagensWeb);
            return false;
        }
        if (Material.length == 2) {
            $.each(Material, function (index, val) {
                if (val.id_ppt)
                    idPPT = idPPT + 1;

            })
            if (idPPT != 1) {
                var mensagensWeb = new Array();
                var msg = '';
                if (idPPT == 2) msg = 'Se forem escolhidos 2 materiais, apenas um deles tem que estar marcado como turma personalizada'
                else msg = 'Se forem escolhidos 2 materiais, pelo menos um deles tem que estar marcado como turma personalizada'
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msg);
                apresentaMensagem("apresentadorMensagemCurso", mensagensWeb);
                return false;
            }
        }

        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/curso/posteditcurso",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    curso: {
                        cd_curso: dojo.byId('cd_curso').value,
                        cd_proximo_curso: dijit.byId("cbProximoCurso").value,
                        cd_nivel: dijit.byId("cbNivel").value,
                        cd_produto: dijit.byId("cbProduto").value,
                        cd_estagio: dijit.byId("cbEstagios").value,
                        cd_modalidade: dijit.byId("cbModalidades").value,
                        no_curso: dom.byId("nomeCurso").value,
                        nm_carga_horaria: dom.byId("cargaHorariaTotal").value,
                        nm_carga_maxima: dom.byId("cargaHorariaMaxima").value,
                        nm_vagas_curso: dom.byId("vagas").value,
                        id_curso_ativo: domAttr.get("ckCursoAtivo", "checked"),
                        id_permitir_matricula: domAttr.get("ckMatricula", "checked"),
                        pc_criterio_aprovacao: dom.byId("pcCriterioAprovacao").value,
                        nm_total_nota: dom.byId("totalNota").value,
                        nm_faixa_etaria_minima: dom.byId("faixaMinima").value,
                        nm_faixa_etaria_maxima: dom.byId("faixaMaxima").value,
                        id_certificado: domAttr.get("ckCertificado", "checked"),
                    },
                    materiaisDidaticos: dijit.byId('gridMaterialDidatico').store.objectStore.data
                })
            }).then(function (data) {
                try{
                    var itemAlterado = data.retorno;
                    var todos = dojo.byId("todosItensCurso_label");
                    var gridName = 'gridCurso';
                    var grid = dijit.byId(gridName);

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cad").hide();
                    removeObjSort(grid.itensSelecionados, "cd_curso", dom.byId("cd_curso").value);
                    insertObjSort(grid.itensSelecionados, "cd_curso", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoCurso', 'cd_curso', 'selecionaTodosCurso', ['pesquisarCurso', 'relatorioCurso'], 'todosItensCurso', 2);
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_curso");
                    showCarregando();
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirCurso() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemCurso', null);
        if (!dijit.byId("formCurso").validate())
            return false;
        var idPPT = 0;
        if (dijit.byId("faixaMaxima").value < dijit.byId("faixaMinima").value) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroFaixaEtaria);
            apresentaMensagem("apresentadorMensagemCurso", mensagensWeb);
            return false;
        }

        var Material = dijit.byId('gridMaterialDidatico').store.objectStore.data;
        if (Material.length > 2) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Somente podem ser escolhidos 2 materiais didáticos no máximo');
            apresentaMensagem("apresentadorMensagemCurso", mensagensWeb);
            return false;
        }
        if (Material.length == 2) {
            $.each(Material, function (index, val) {
                if (val[index].id_ppt)
                    idPPT = idPPT + 1;

            })
            if (idPPT != 1) {
                var mensagensWeb = new Array();
                var msg = '';
                if (idPPT == 2) msg = 'Se forem escolhidos 2 materiais, apenas um deles tem que estar marcado como turma personalizada'
                else msg = 'Se forem escolhidos 2 materiais, pelo menos um deles tem que estar marcado como turma personalizada'
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msg);
                apresentaMensagem("apresentadorMensagemCurso", mensagensWeb);
                return false;
            }
        }
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/curso/postcurso",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    curso: {
                        cd_proximo_curso: dijit.byId("cbProximoCurso").value,
                        cd_nivel: dijit.byId("cbNivel").value,
                        cd_produto: dijit.byId("cbProduto").value,
                        cd_estagio: dijit.byId("cbEstagios").value,
                        cd_modalidade: dijit.byId("cbModalidades").value,
                        no_curso: dom.byId("nomeCurso").value,
                        nm_carga_horaria: dom.byId("cargaHorariaTotal").value,
                        nm_carga_maxima: dom.byId("cargaHorariaMaxima").value,
                        nm_vagas_curso: dom.byId("vagas").value,
                        id_curso_ativo: domAttr.get("ckCursoAtivo", "checked"),
                        id_permitir_matricula: domAttr.get("ckMatricula", "checked"),
                        pc_criterio_aprovacao: dom.byId("pcCriterioAprovacao").value,
                        nm_total_nota: dom.byId("totalNota").value,
                        nm_faixa_etaria_minima: dom.byId("faixaMinima").value,
                        nm_faixa_etaria_maxima: dom.byId("faixaMaxima").value,
                        id_certificado: domAttr.get("ckCertificado", "checked"),
                    },
                    materiaisDidaticos: dijit.byId('gridMaterialDidatico').store.objectStore.data
                })
            }).then(function (data) {
                try{
                    var itemAlterado = data.retorno;
                    var gridName = 'gridCurso';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cad").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    insertObjSort(grid.itensSelecionados, "cd_curso", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoCurso', 'cd_curso', 'selecionaTodosCurso', ['pesquisarCurso', 'relatorioCurso'], 'todosItensCurso', 2);
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_curso");
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarCurso(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                    JsonRest({
                        target: !hasValue(document.getElementById("descCurso").value) ? Endereco() + "/api/curso/getCursoSearch?desc=&inicio=" + document.getElementById("inicioCurso").checked + "&status=" + retornaStatus("statusCurso") + "&produto=" + dijit.byId('cbPesqProduto').value + "&estagio=" + dijit.byId('cbPesqEstagios').value + "&modalidade=" + dijit.byId('cbPesqModalidades').value + "&nivel=" + dijit.byId('cbPesqNivel').value : Endereco() + "/api/curso/getCursoSearch?desc=" + encodeURIComponent(document.getElementById("descCurso").value) + "&inicio=" + document.getElementById("inicioCurso").checked + "&status=" + retornaStatus("statusCurso") + "&produto=" + dijit.byId('cbPesqProduto').value + "&estagio=" + dijit.byId('cbPesqEstagios').value + "&modalidade=" + dijit.byId('cbPesqModalidades').value + "&nivel=" + dijit.byId('cbPesqNivel').value,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridCurso");

            if (limparItens)
                grid.itensSelecionados = [];

            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisarMatDid(limparItens) {
    require([
          "dojo/_base/xhr",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (xhr, ObjectStore, Cache, Memory) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getItemCurso?cdCurso=" + dojo.byId("cd_curso").value,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_item_curso"
        }).then(function (data) {
            try{
                //dataProd = jQuery.parseJSON(dataProd);
                var grid = dijit.byId("gridMaterialDidatico");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: data, idProperty: "cd_item_curso" }) });
                grid.setStore(dataStore);
                if (limparItens)
                    grid.itensSelecionados = [];
                //else
                //    grid.itensSelecionados = dijit.byId("gridMaterialDidatico").store.objectStore.data;

                quickSortObj(grid.itensSelecionados, 'cd_item_curso');
                dijit.byId('material').set('open', true);
                dijit.byId('gridMaterialDidatico').update();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function deletarCurso(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_curso').value != 0)
                    itensSelecionados = [{
                        cd_curso: dom.byId("cd_curso").value,
                        no_curso: dom.byId("nomeCurso").value,
                        id_curso_ativo: domAttr.get("ckCursoAtivo", "checked"),
                        id_permitir_matricula: domAttr.get("ckMatricula", "checked")
            }];
            xhr.post({
                url: Endereco() + "/api/curso/postDeleteCurso",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensCurso_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cad").hide();

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridCurso').itensSelecionados, "cd_curso", itensSelecionados[r].cd_curso);

                    pesquisarCurso(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarCurso").set('disabled', false);
                    dijit.byId("relatorioCurso").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cad").style.display))
                    apresentaMensagem('apresentadorMensagemCurso', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    })
}

function eventoEditar(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            var gridCurso = dijit.byId('gridCurso');

            limparCurso();
            apresentaMensagem('apresentadorMensagem', '');
            gridCurso.itemSelecionado = montaObjetoCurso(itensSelecionados[0]);
            keepValues(null, gridCurso, true);
            dijit.byId("cad").show();
            IncluirAlterar(0, 'divAlterarCurso', 'divIncluirCurso', 'divExcluirCurso', 'apresentadorMensagemCurso', 'divCancelarCurso', 'divLimparCurso');
            dijit.byId('tabContainer').resize();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirItemFK() {
    try{
        limparPesquisaCursoFK(false);
        pesquisarItemFK(MATERIALDIDATICO);
        dijit.byId("proItem").show();
        dijit.byId("gridPesquisaItem").update();
        dijit.byId("tipo").set("disabled", true);
        dijit.byId("tipo").set("value", 1);
        dojo.byId("cdTipoItem").value = 1;
        dijit.byId("statusItemFK").set("disabled", true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarPptMaterial(grid, rowIndex, value) {
    try {
        grid.store.objectStore.data[rowIndex].id_ppt = value;
        grid.update();
    } catch (e) {
        postGerarLog(e);
    }
}
