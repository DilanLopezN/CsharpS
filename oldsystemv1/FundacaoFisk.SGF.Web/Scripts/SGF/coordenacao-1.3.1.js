
//#region declaração de constanates
var SOBE = 1, DESCE = 2;
var HAS_ATIVO = 0;
var HAS_CONCEITO = 1;
var HAS_HISTORICO = 2;
var HAS_ESTAGIO = 3;
var HAS_CAD_ESTAGIO = 6;

//Os tipos de formulário:
var FORM_SALA = 1;
var FORM_EVENTO = 2;
var FORM_PRODUTO = 3;
var FORM_DURACAO = 4;
var FORM_ATIVIDADEEXTRA = 5;
var FORM_MOTIVODESISTENCIA = 6;
var FORM_MOTIVOFALTA = 7;
var FORM_MODALIDADE = 8;
var FORM_REGIME = 9;
var FORM_CONCEITO = 10;
var FORM_FERIADO = 11;
var FORM_ESTAGIO = 12;
var FORM_PARTICIPACAO = 13;
var FORM_CARGAPROFESSOR = 14;
var FORM_NIVEL = 15;
var FORM_MENSAGEM_AVALIACAO = 16;

var alterarOrd = 0, ordemOri = 0;

var feriadoFinanceiro = new Array(
	{ name: "Financeiro", id: "1" },
	{ name: "Normal", id: "2" }
);

var EnumTipoPesquisaProduto = {
    HAS_ATIVO_CURSO: 9
}

var EnumTipoPesquisaCurso = {
	HAS_ATIVO: 0,
	HAS_PRODUTO: 2,
	HAS_ATIVOPROD: 4


}

var EnumTipoMensagemMultiSelect = {
	CURSO: 9
}

//#endregion

//#region  métodos auxiliares -  montarStatus - criarComponenteFiltering - montarSpinner -  VerificaData -  loadProduto -  validarDados
function montarComponenteStatus() {
    require([
           "dojo/ready",
           "dojo/store/Memory",
           "dijit/form/FilteringSelect"
    ], function (ready, Memory, FilteringSelect) {
        ready(function () {
            try {
                montarStatus("statusSala");
                montarStatus("statusEvento");
                montarStatus("statusProduto");
                montarStatus("statusDuracao");
                montarStatus("statusAtividadeExtra");
                montarStatus("statusMotivoDesistencia");
                montarStatus("statusMotivoFalta");
                montarStatus("statusModalidade");
                montarStatus("statusRegime");
                montarStatus("statusConceito");
                montarStatus("statusEstagio");
                montarStatus("statusParticipacao");
                montarStatus("statusNivel");
                montarStatus("statusMensagemAvaliacao");
                criarOuCarregarCompFiltering('statusFeriado', feriadoFinanceiro, 'width: 85px;', 0, ready, Memory, FilteringSelect, 'id', 'name', 2);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

// Retona o númeração do Núimero de Vagas na sala
function montarSpinner(spinner, minimo, maximo, def, pttn) {
    require(["dojo/ready", "dijit/form/NumberSpinner"], function (ready, NumberSpinner) {
        ready(function () {
            try {
                var mySpinner = new NumberSpinner({
                    value: def,
                    smallDelta: 1,
                    constraints: { min: minimo, max: maximo, places: 0, pattern: pttn },
                    id: spinner,
                    style: "width:100px"
                }, spinner);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}//function montarSpinner

function VerificaData(dia, mes, ano) {
    try {
        var bissexto = 0;
        var ok = false;
        //var data = digData;
        //var tam = data.length;
        //if (tam == 10) {
        //    var dia = data.substr(0, 2)
        //    var mes = data.substr(3, 2)
        //    var ano = data.substr(6, 4)
        if ((ano > 1900) || (ano < 2100) || (ano == 0)) {
            switch (mes) {
                case 01:
                case 03:
                case 05:
                case 07:
                case 08:
                case 10:
                case 12:
                    if (dia <= 31) {
                        // return true;
                        ok = true;
                    }
                    break

                case 04:
                case 06:
                case 09:
                case 11:
                    if (dia <= 30) {
                        //   return true;
                        ok = true;
                    }
                    break
                case 02:
                    /* Validando ano Bissexto / fevereiro / dia */
                    if (hasValue(ano)) {
                        if ((ano % 4 == 0) || (ano % 100 == 0) || (ano % 400 == 0)) {
                            bissexto = 1;
                        }
                        if ((bissexto == 1) && (dia <= 29)) {
                            // return true;
                            ok = true;
                        }
                        if ((bissexto != 1) && (dia <= 28)) {
                            //return true;
                            ok = true;
                        }
                    }
                    else
                        if (dia <= 29) {
                            //return true;
                            ok = true;
                        }
                    break
            }
            //}
        }
        return ok;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarTipoFeriado(ready, Memory, filteringSelect) {
    try {
        var statusTipo = new Memory({
            data: [
            { name: "Todos", id: 0 },
            { name: "Não Fixos", id: 1 },
            { name: "Fixos", id: 2 }
            ]
        });

        var _tipo = new filteringSelect({
            id: "somenteAnoPes",
            name: "somenteAnoPes",
            store: statusTipo,
            searchAttr: "name",
            value: 0,
            style: "width: 10%;"
        }, "somenteAno");
    }
    catch (e) {
        postGerarLog(e);
    }
}
function loadProduto(data, linkProduto) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try {
		        var items = [];
		        if ((linkProduto == 'codProduto') || (linkProduto == 'codProdutoE'))
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_produto, name: value.no_produto });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId(linkProduto).store = stateStore;
		    }
		    catch (e) {
		        postGerarLog(e);
		    }
		})
}


function loadCurso(items, field) {
	require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
			try {
				var itemsCb = [];
				var cbCurso = dijit.byId(field);
				Array.forEach(items, function (value, i) {
					itemsCb.push({ id: value.cd_curso, name: value.no_curso });
				});
				var stateStore = new Memory({
					data: itemsCb
				});
				cbCurso.store = stateStore;
			}
			catch (e) {
				postGerarLog(e);
			}
		});
}



// Valida os dados e Mostra a Mensagem
function validarDados(apresentadorMensagem, valor, tipoForm, tipoCrud) {
    try {
        if (valor == "") {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgDescObrigtoria);
            apresentaMensagem(apresentadorMensagem, mensagensWeb);
            setarFocus(tipoForm);
            return false
        }
        else {
            startCrud(tipoCrud, tipoForm);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region formatação dos chekbox
function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridSala';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_sala", grid._by_idx[rowIndex].item.cd_sala);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_sala', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_sala', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxProduto(value, rowIndex, obj) {
    try {
        var gridName = 'gridProduto';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosProdutos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_produto", grid._by_idx[rowIndex].item.cd_produto);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_produto', 'selecionadoProduto', -1, 'selecionaTodosProdutos', 'selecionaTodosProdutos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_produto', 'selecionadoProduto', " + rowIndex + ", '" + id + "', 'selecionaTodosProdutos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxEstagio(value, rowIndex, obj) {
    try {
        var gridName = 'gridEstagio';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEstagios');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_estagio", grid._by_idx[rowIndex].item.cd_estagio);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_estagio', 'selecionadoEstagio', -1, 'selecionaTodosEstagios', 'selecionaTodosEstagios', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_estagio', 'selecionadoEstagio', " + rowIndex + ", '" + id + "', 'selecionaTodosEstagios', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxDuracao(value, rowIndex, obj) {
    try {
        var gridName = 'gridDuracao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosDuracao');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_duracao", grid._by_idx[rowIndex].item.cd_duracao);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_duracao', 'selecionadoDuracao', -1, 'selecionaTodosDuracao', 'selecionaTodosDuracao', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_duracao', 'selecionadoDuracao', " + rowIndex + ", '" + id + "', 'selecionaTodosDuracao', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxModalidade(value, rowIndex, obj) {
    try {
        var gridName = 'gridModalidade';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodasModalidades');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_modalidade", grid._by_idx[rowIndex].item.cd_modalidade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_modalidade', 'selecionadoModalidade', -1, 'selecionaTodasModalidades', 'selecionaTodasModalidades', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_modalidade', 'selecionadoModalidade', " + rowIndex + ", '" + id + "', 'selecionaTodasModalidades', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxRegime(value, rowIndex, obj) {
    try {
        var gridName = 'gridRegime';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosRegimes');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_regime", grid._by_idx[rowIndex].item.cd_regime);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_regime', 'selecionadoRegime', -1, 'selecionaTodosRegimes', 'selecionaTodosRegimes', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_regime', 'selecionadoRegime', " + rowIndex + ", '" + id + "', 'selecionaTodosRegimes', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxConceito(value, rowIndex, obj) {
    try {
        var gridName = 'gridConceito';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosConceitos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_conceito", grid._by_idx[rowIndex].item.cd_conceito);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_conceito', 'selecionadoConceito', -1, 'selecionaTodosConceitos', 'selecionaTodosConceitos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_conceito', 'selecionadoConceito', " + rowIndex + ", '" + id + "', 'selecionaTodosConceitos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxFeriado(value, rowIndex, obj) {
    try {
        var gridName = 'gridFeriado';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosFeriados');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cod_feriado", grid._by_idx[rowIndex].item.cod_feriado);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cod_feriado', 'selecionadoFeriado', -1, 'selecionaTodosFeriados', 'selecionaTodosFeriados', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cod_feriado', 'selecionadoFeriado', " + rowIndex + ", '" + id + "', 'selecionaTodosFeriados', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxEvento(value, rowIndex, obj) {
    try {
        var gridName = 'gridEvento';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEventos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_evento", grid._by_idx[rowIndex].item.cd_evento);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_evento', 'selecionadoEvento', -1, 'selecionaTodosEventos', 'selecionaTodosEventos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_evento', 'selecionadoEvento', " + rowIndex + ", '" + id + "', 'selecionaTodosEventos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMotivosFalta(value, rowIndex, obj) {
    try {
        var gridName = 'gridMotivoFalta';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMotivosFalta');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_falta", grid._by_idx[rowIndex].item.cd_motivo_falta);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_falta', 'selecionadoMotivoFalta', -1, 'selecionaTodosMotivosFalta', 'selecionaTodosMotivosFalta', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_falta', 'selecionadoMotivoFalta', " + rowIndex + ", '" + id + "', 'selecionaTodosMotivosFalta', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMotivoDesistencia(value, rowIndex, obj) {
    try {
        var gridName = 'gridMotivoDesistencia';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMotivosDesistencia');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_desistencia", grid._by_idx[rowIndex].item.cd_motivo_desistencia);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_desistencia', 'selecionadoMotivoDesistencia', -1, 'selecionaTodosMotivosDesistencia', 'selecionaTodosMotivosDesistencia', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_desistencia', 'selecionadoMotivoDesistencia', " + rowIndex + ", '" + id + "', 'selecionaTodosMotivosDesistencia', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAtividadeExtra(value, rowIndex, obj) {
    try {
        var gridName = 'gridAtividadeExtra';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodasAtividadesExtras');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_atividade_extra", grid._by_idx[rowIndex].item.cd_tipo_atividade_extra);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_atividade_extra', 'selecionadaAtividadeExtra', -1, 'selecionaTodasAtividadesExtras', 'selecionaTodasAtividadesExtras', '" + gridName + "')", grid.rowsPerPage * 3);

        // Configura o check de todos:
        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_atividade_extra', 'selecionadaAtividadeExtra', " + rowIndex + ", '" + id + "', 'selecionaTodasAtividadesExtras', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxParticipacao(value, rowIndex, obj) {
    try {
        var gridName = 'gridParticipacao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodasParticipacao');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_participacao", grid._by_idx[rowIndex].item.cd_participacao);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_participacao', 'selecionadaParticipacao', -1, 'selecionaTodasParticipacao', 'selecionaTodasParticipacao', '" + gridName + "')", grid.rowsPerPage * 3);

        // Configura o check de todos:
        setTimeout("configuraCheckBox(" + value + ", 'cd_participacao', 'selecionadaParticipacao', " + rowIndex + ", '" + id + "', 'selecionaTodasParticipacao', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxCargaProfessor(value, rowIndex, obj) {
    try {
        var gridName = 'gridCargaProfessor';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodasCargaProfessor');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_carga_professor", grid._by_idx[rowIndex].item.cd_carga_professor);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_carga_professor', 'selecionadaCargaProfessor', -1, 'selecionaTodasCargaProfessor', 'selecionaTodasCargaProfessor', '" + gridName + "')", grid.rowsPerPage * 3);

        // Configura o check de todos:
        setTimeout("configuraCheckBox(" + value + ", 'cd_carga_professor', 'selecionadaCargaProfessor', " + rowIndex + ", '" + id + "', 'selecionaTodasCargaProfessor', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxNivel(value, rowIndex, obj) {
    try {
        var gridName = 'gridNivel';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosNiveis');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_nivel", grid._by_idx[rowIndex].item.cd_nivel);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_nivel', 'selecionadoNivel', -1, 'selecionaTodosNiveis', 'selecionaTodosNiveis', '" + gridName + "')", grid.rowsPerPage * 3);

        // Configura o check de todos:
        setTimeout("configuraCheckBox(" + value + ", 'cd_nivel', 'selecionadoNivel', " + rowIndex + ", '" + id + "', 'selecionaTodosNiveis', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMensagemAvaliacao(value, rowIndex, obj) {
	try {
		var gridName = 'gridMensagemAvaliacao';
		var grid = dijit.byId(gridName);
		var icon;
		var id = obj.field + '_Selected_' + rowIndex;
		var todos = dojo.byId('selecionaTodosMensagemAvaliacao');

		if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
			var indice = binaryObjSearch(grid.itensSelecionados, "cd_mensagem_avaliacao", grid._by_idx[rowIndex].item.cd_mensagem_avaliacao);

			value = value || indice != null; // Item está selecionado.
		}
		if (rowIndex != -1)
			icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

		// Configura o check de todos, para quando mudar de aba:
		if (hasValue(todos) && todos.type == 'text')
			setTimeout("configuraCheckBox(false, 'cd_mensagem_avaliacao', 'selecionadoMensagemAvaliacao', -1, 'selecionaTodosMensagemAvaliacao', 'selecionaTodosMensagemAvaliacao', '" + gridName + "')", grid.rowsPerPage * 3);

		// Configura o check de todos:
		setTimeout("configuraCheckBox(" + value + ", 'cd_mensagem_avaliacao', 'selecionadoMensagemAvaliacao', " + rowIndex + ", '" + id + "', 'selecionaTodosMensagemAvaliacao', '" + gridName + "')", 2);

		return icon;
	}
	catch (e) {
		postGerarLog(e);
	}
}

// ********** fim dos  formatCheckBox **********  \\

//#endregion

//#region keepvalues

//Pega os Antigos dados do Formulário
function keepValues(tipoForm, value, grid, ehLink) {
    try {
        showCarregando();
        var valorCacelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');

        if (!hasValue(ehLink, true)) {
            ehLink = eval(linkAnterior.value);
        }
        linkAnterior.value = ehLink;

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        //Quando for cancelamento
        if (!hasValue(value) && hasValue(valorCacelamento) && !ehLink) {
            value = valorCacelamento[0];
        }
        if (hasValue(value))
            switch (tipoForm) {
                case FORM_SALA: {
                    getLimpar('#formSala');
                    dojo.byId("cd_sala").value = value.cd_sala;
                    dojo.byId("no_sala").value = value.no_sala;
                    dojo.byId("id_sala_ativa").value = value.id_sala_ativa == true ? dijit.byId("id_sala_ativa").set("value", true) : dijit.byId("id_sala_ativa").set("value", false);
                    dojo.byId("nm_vaga_sala").value = value.nm_vaga_sala;
                    dojo.byId("ck_sala_online").value = value.id_sala_online == true ? dijit.byId("ck_sala_online").set("value", true) : dijit.byId("ck_sala_online").set("value", false);
                    dojo.byId("edzoom").value = value.id_zoom;
                    dojo.byId("eduser").value = value.dc_usuario_escola;
                    dojo.byId("edsenha").value = value.dc_senha_usuario_escola;
                    dojo.byId("edapikey").value = value.dc_usuario_adm;
                    dojo.byId("edapisenha").value = value.dc_senha_usuario_adm;
                    break;
                }//FORM_SALA
                case FORM_EVENTO: {
                    getLimpar('#formEvento');
                    dojo.byId("cd_evento").value = value.cd_evento;
                    dojo.byId("no_evento").value = value.no_evento;
                    dojo.byId("id_evento_ativo").value = value.id_evento_ativo == true ? dijit.byId("id_evento_ativo").set("value", true) : dijit.byId("id_evento_ativo").set("value", false);
                    break;
                }//FORM_EVENTO
                case FORM_PRODUTO: {
                    getLimpar('#formProduto');
                    dojo.byId("cd_produto").value = value.cd_produto;
                    dojo.byId("no_produto").value = value.no_produto;
                    dojo.byId("no_produto_abreviado").value = value.no_produto_abreviado;
                    dojo.byId("id_produto_ativo").value = value.id_produto_ativo == true ? dijit.byId("id_produto_ativo").set("value", true) : dijit.byId("id_produto_ativo").set("value", false);
                    break;
                }//FORM_PRODUTO
                case FORM_DURACAO: {
                    getLimpar('#formDuracao');
                    dojo.byId("cd_duracao").value = value.cd_duracao;
                    dojo.byId("dc_duracao").value = value.dc_duracao;
                    dojo.byId("id_duracao_ativa").value = value.id_duracao_ativa == true ? dijit.byId("id_duracao_ativa").set("value", true) : dijit.byId("id_duracao_ativa").set("value", false);
                    break;
                }//FORM_DURACAO
                case FORM_ATIVIDADEEXTRA: {
                    getLimpar('#formAtividadeExtra');
                    dojo.byId("cd_tipo_atividade_extra").value = value.cd_tipo_atividade_extra;
                    dojo.byId("no_tipo_atividade_extra").value = value.no_tipo_atividade_extra;
                    dojo.byId("id_tipo_atividade_extra_ativa").value = value.id_tipo_atividade_extra_ativa == true ? dijit.byId("id_tipo_atividade_extra_ativa").set("value", true) : dijit.byId("id_tipo_atividade_extra_ativa").set("value", false);
                    break;
                }//FORM_ATIVIDADEEXTRA
                case FORM_MOTIVODESISTENCIA: {
                    getLimpar('#formMotivoDesistencia');
                    dojo.byId("cd_motivo_desistencia").value = value.cd_motivo_desistencia;
                    dojo.byId("dc_motivo_desistencia").value = value.dc_motivo_desistencia;
                    dojo.byId("id_motivo_desistencia_ativo").value = value.id_motivo_desistencia_ativo == true ? dijit.byId("id_motivo_desistencia_ativo").set("value", true) : dijit.byId("id_motivo_desistencia_ativo").set("value", false);
                    dijit.byId("id_cancelamento").set("value", value.id_cancelamento);
                    break;
                }//FORM_MOTIVODESISTENCIA
                case FORM_MOTIVOFALTA: {
                    getLimpar('#formMotivoFalta');
                    dojo.byId("cd_motivo_falta").value = value.cd_motivo_falta;
                    dojo.byId("dc_motivo_falta").value = value.dc_motivo_falta;
                    dojo.byId("id_motivo_falta_ativa").value = value.id_motivo_falta_ativa == true ? dijit.byId("id_motivo_falta_ativa").set("value", true) : dijit.byId("id_motivo_falta_ativa").set("value", false);
                    break;
                }//FORM_MOTIVOFALTA
                case FORM_MODALIDADE: {
                    getLimpar('#formModalidade');
                    dojo.byId("cd_modalidade").value = value.cd_modalidade;
                    dojo.byId("no_modalidade").value = value.no_modalidade;
                    dojo.byId("id_modalidade_ativa").value = value.id_modalidade_ativa == true ? dijit.byId("id_modalidade_ativa").set("value", true) : dijit.byId("id_modalidade_ativa").set("value", false);
                    break;
                }//FORM_MODALIDADE
                case FORM_REGIME: {
                    getLimpar('#formRegime');
                    dojo.byId("cd_regime").value = value.cd_regime;
                    dojo.byId("no_regime").value = value.no_regime;
                    dojo.byId("no_regime_abreviado").value = value.no_regime_abreviado;
                    dojo.byId("id_regime_ativo").value = value.id_regime_ativo == true ? dijit.byId("id_regime_ativo").set("value", true) : dijit.byId("id_regime_ativo").set("value", false);
                    break;
                }//FORM_REGIME
                case FORM_CONCEITO: {
                    getLimpar('#formConceito');
                    dojo.byId("cd_conceito").value = value.cd_conceito;
                    dojo.byId("no_conceito").value = value.no_conceito;
                    dojo.byId("id_conceito_ativo").value = value.id_conceito_ativo == true ? dijit.byId("id_conceito_ativo").set("value", true) : dijit.byId("id_conceito_ativo").set("value", false);
                    dojo.byId("pc_inicial_conceito").value = value.pc_inicial;
                    dojo.byId("pc_final_conceito").value = value.pc_final;
                    dijit.byId("cd_produtoC").set("value", value.cd_produto);
                    dojo.byId("vl_nota_participacao").value = value.val_nota_participacao;
                    break;
                }//FORM_CONCEITO
                case FORM_FERIADO: {
                    getLimpar('#formFeriado');
                    dojo.byId("cod_feriado").value = value.cod_feriado;
                    dojo.byId("dc_feriado").value = value.dc_feriado;
                    dojo.byId("dd_feriado").value = value.dd_feriado;
                    dojo.byId("mm_feriado").value = value.mm_feriado;
                    dojo.byId("aa_feriado").value = value.ano;
                    dojo.byId("id_feriado_financeiro").value = value.id_feriado_financeiro == true ? dijit.byId("id_feriado_financeiro").set("value", true) : dijit.byId("id_feriado_financeiro").set("value", false);
                    dojo.byId("diaFim").value = value.dd_feriado_fim;
                    dojo.byId("mesFim").value = value.mm_feriado_fim;
                    dojo.byId("anoFim").value = value.aa_feriado_fim;
                    dijit.byId("id_novo_feriado_ativo").set("checked", value.id_feriado_ativo);
                    break;
                }//FORM_FERIADO
                case FORM_ESTAGIO: {                    
                    getLimpar('#formEstagio');
                    clearForm("formEstagio");
                    dojo.byId("cd_estagio").value = value.cd_estagio;
                    dojo.byId("no_estagio").value = value.no_estagio;
                    dojo.byId("no_estagio_abreviado").value = value.no_estagio_abreviado;
                    dojo.byId("id_estagio_ativo").value = value.id_estagio_ativo == true ? dijit.byId("id_estagio_ativo").set("value", true) : dijit.byId("id_estagio_ativo").set("value", false);
                    dojo.byId("num_ordem_estagio").value = value.nm_ordem_estagio;
                    //dijit.byId("cd_produtoCE").value = value.cd_produto// dijit.byId("cd_produtoCE").value;
                    dijit.byId("cd_produtoCE").set("value", value.cd_produto);
                    break;
                }//FORM_ESTAGIO
                case FORM_PARTICIPACAO: {
                    getLimpar('#formParticipacao');
                    clearForm("formParticipacao");
                    dojo.byId("cd_participacao").value = value.cd_participacao;
                    dojo.byId("noParticipacao").value = value.no_participacao;
                    dojo.byId("idParticipacaoAtiva").value = value.id_participacao_ativa == true ? dijit.byId("idParticipacaoAtiva").set("value", true) : dijit.byId("idParticipacaoAtiva").set("value", false);
                    break;
                }//FORM_PARTICIPACAO
                case FORM_CARGAPROFESSOR: {
                    getLimpar('#formCargaProfessor');
                    clearForm("formCargaProfessor");
                    IncluirAlterar(0, 'divAlterarCargaProfessor', 'divIncluirCargaProfessor', 'divExcluirCargaProfessor', 'apresentadorMensagem', 'divCancelarCargaProfessor', 'divClearCargaProfessor');
                    showCarregando();
                    dojo.xhr.get({
                        url: Endereco() + "/api/Coordenacao/getDuracoes?cd_duracao=" + value.cd_carga_horaria_duracao,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }).then(function (dataDuracaoAtiva) {
                        dojo.byId("cd_carga_professor").value = value.cd_carga_professor;
                        dojo.byId("nmCargaHoraria").value = value.nm_carga_horaria;
                        dojo.byId("nmCargaProfessor").value = value.nm_carga_professor;
                        dijit.byId("cadCargaProfessor").show();
                        showCarregando();
                    });
                    break;
                }//FORM_CARGAPROFESSOR
                case FORM_NIVEL: {
                    getLimpar('#formNivel');
                    clearForm("formNivel");
                    dojo.byId("cd_nivel").value = value.cd_nivel;
                    dojo.byId("dc_nivel").value = value.dc_nivel;
                    dojo.byId("nm_ordem").value = value.nm_ordem;
                    dojo.byId("id_nivel_ativo").value = value.id_ativo == true ? dijit.byId("id_nivel_ativo").set("value", true) : dijit.byId("id_nivel_ativo").set("value", false);
                    break;
                }//FORM_NIVEL
                case FORM_MENSAGEM_AVALIACAO: {
	                getLimpar('#formMensagemAvaliacaoCad');
	                clearForm("formMensagemAvaliacaoCad");
	                dojo.byId("cd_mensagem_avaliacao").value = value.cd_mensagem_avaliacao;
                    dojo.byId("txObs").value = value.tx_mensagem_avaliacao;
                    dojo.byId("nm_ordem").value = value.nm_ordem;
                    
                    dijit.byId('pesCadProduto')._onChangeActive = false;
                    hasValue(value.cd_produto) ? dijit.byId('pesCadProduto').set("value", value.cd_produto) : dijit.byId('pesCadProduto').reset();
                    dijit.byId('pesCadProduto')._onChangeActive = true;
                    dojo.byId("id_mensagem_avaliacao_ativa_pers").value = value.id_mensagem_ativa == true ? dijit.byId("id_mensagem_avaliacao_ativa_pers").set("value", true) : dijit.byId("id_mensagem_avaliacao_ativa_pers").set("value", false);

                    dijit.byId("cbCursos").set("disabled", false);
                    checkedValuesTrue(null, [value.cd_curso], "cbCursos");
                    dijit.byId("cbCursos").set("disabled", true);
                    dijit.byId("pesCadProduto").set("disabled", true);
	                break;
                }//FORM_NIVEL

            }
        showCarregando();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesEstagio(tipoForm, value, grid, ehLink) {
    try {
        showCarregando();
        var valorCacelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');

        if (!hasValue(ehLink, true)) {
            ehLink = eval(linkAnterior.value);
        }
        linkAnterior.value = ehLink;

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        //Quando for cancelamento
        if (!hasValue(value) && hasValue(valorCacelamento) && !ehLink) {
            value = valorCacelamento[0];
        }
        dojo.byId("cd_estagio").value = value.cd_estagio;
        dojo.byId("no_estagio").value = value.no_estagio;
        dojo.byId("no_estagio_abreviado").value = value.no_estagio_abreviado;
        dojo.byId("id_estagio_ativo").value = value.id_estagio_ativo == true ? dijit.byId("id_estagio_ativo").set("value", true) : dijit.byId("id_estagio_ativo").set("value", false);
        dojo.byId("num_ordem_estagio").value = value.nm_ordem_estagio;
        dojo.byId("widget_cor_legenda_estagio").style.backgroundColor = value.cor_legenda;
        dijit.byId("nm_paleta_cor").set("value", value.cor_legenda);
        showCarregando();
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region mostraTabs
function mostraTabs(Permissoes) {
    require([
		 "dijit/registry",
		 "dojo/ready",
		 "dojo/require"
    ], function (registry, ready) {
        ready(function () {
            try {
                if (!possuiPermissao('sala', Permissoes)) {
                    registry.byId('tabSala').set('disabled', !registry.byId('tabSala').get('disabled'));
                    document.getElementById('tabSala').style.visibility = "hidden";
                }

                if (!possuiPermissao('even', Permissoes)) {
                    registry.byId('tabEvento').set('disabled', !registry.byId('tabEvento').get('disabled'));
                    document.getElementById('tabEvento').style.visibility = "hidden";
                }

                if (!possuiPermissao('prod', Permissoes)) {
                    registry.byId('tabProduto').set('disabled', !registry.byId('tabProduto').get('disabled'));
                    document.getElementById('tabProduto').style.visibility = "hidden";
                }

                if (!possuiPermissao('dur', Permissoes)) {
                    registry.byId('tabDuracao').set('disabled', !registry.byId('tabDuracao').get('disabled'));
                    document.getElementById('tabDuracao').style.visibility = "hidden";
                }

                if (!possuiPermissao('tavex', Permissoes)) {
                    registry.byId('tabAtividadeExtra').set('disabled', !registry.byId('tabAtividadeExtra').get('disabled'));
                    document.getElementById('tabAtividadeExtra').style.visibility = "hidden";
                }

                if (!possuiPermissao('mtdes', Permissoes)) {
                    registry.byId('tabMotivoDesistencia').set('disabled', !registry.byId('tabMotivoDesistencia').get('disabled'));
                    document.getElementById('tabMotivoDesistencia').style.visibility = "hidden";
                }

                if (!possuiPermissao('mtfal', Permissoes)) {
                    registry.byId('tabMotivoFalta').set('disabled', !registry.byId('tabMotivoFalta').get('disabled'));
                    document.getElementById('tabMotivoFalta').style.visibility = "hidden";
                }

                if (!possuiPermissao('mod', Permissoes)) {
                    registry.byId('tabModalidade').set('disabled', !registry.byId('tabModalidade').get('disabled'));
                    document.getElementById('tabModalidade').style.visibility = "hidden";
                }

                if (!possuiPermissao('reg', Permissoes)) {
                    registry.byId('tabRegime').set('disabled', !registry.byId('tabRegime').get('disabled'));
                    document.getElementById('tabRegime').style.visibility = "hidden";
                }

                if (!possuiPermissao('conc', Permissoes)) {
                    registry.byId('tabConceito').set('disabled', !registry.byId('tabConceito').get('disabled'));
                    document.getElementById('tabConceito').style.visibility = "hidden";
                }

                if (!possuiPermissao('fer', Permissoes)) {
                    registry.byId('tabFeriado').set('disabled', !registry.byId('tabFeriado').get('disabled'));
                    document.getElementById('tabFeriado').style.visibility = "hidden";
                }

                if (!possuiPermissao('estag', Permissoes)) {
                    registry.byId('tabEstagio').set('disabled', !registry.byId('tabEstagio').get('disabled'));
                    document.getElementById('tabEstagio').style.visibility = "hidden";
                }

                if (!possuiPermissao('par', Permissoes)) {
                    registry.byId('tabParticipacao').set('disabled', !registry.byId('tabParticipacao').get('disabled'));
                    document.getElementById('tabParticipacao').style.visibility = "hidden";
                }

                if (!possuiPermissao('chpro', Permissoes)) {
                    registry.byId('tabCargaProfessor').set('disabled', !registry.byId('tabCargaProfessor').get('disabled'));
                    document.getElementById('tabCargaProfessor').style.visibility = "hidden";
                }

                if (!possuiPermissao('niv', Permissoes)) {
                    registry.byId('tabNivel').set('disabled', !registry.byId('tabNivel').get('disabled'));
                    document.getElementById('tabNivel').style.visibility = "hidden";
                }


            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
};
//#endregion

//#region montarCadastroSala
function montarCadastroSala() {
    //Criação da Grade de sala
    require([
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
        "dijit/MenuItem",
        "dijit/Dialog",
        "dojo/domReady!",
        "dojo/parser"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        ready(function () {
            try {
                dijit.byId('tabContainer').resize();
                montarComponenteStatus();
                myStore = Cache(
			    JsonRest({
			        target: Endereco() + "/api/coordenacao/getSalaSearch?desc=&inicio=false&status=1&online=false",
			        handleAs: "json",
			        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
			        idProperty: "cd_sala"
			    }), Memory({ idProperty: "cd_sala" }));

                var gridSala = new EnhancedGrid({
                    // store: ObjectStore({ objectStore: myStore }),
                    store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    structure: [
				    { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
				    //{ name: "Código", field: "cd_sala", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
			       // { name: "Escola", field: "no_pessoa", width: "30em" },
				    { name: "Sala", field: "no_sala", width: "75%" },
				    { name: "Vaga", field: "nm_vaga_sala", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
				    { name: "Ativa", field: "salaAtiva", width: "75px", styles: "text-align: center;" }
                    ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc,
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
                }, "gridSala");

                //Paginação
                gridSala.pagination.plugin._paginator.plugin.connect(gridSala.pagination.plugin._paginator, 'onSwitchPageSize',
				    function (evt) {
				        verificaMostrarTodos(evt, gridSala, 'cd_sala', 'selecionaTodos');
				    });
                //Seleciona todos e impede que a primeira coluna seja ordenada
                gridSala.canSort = function (col) { return Math.abs(col) != 1; };
                gridSala.startup();
                gridSala.on("RowDblClick", function (evt) {
                    var idx = evt.rowIndex,
					    item = this.getItem(idx),
					    store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formSala');
                    clearForm("formSala");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_SALA, item, gridSala, false);
                    validaEdicaoNomeSalaOnline(item);

                    dijit.byId("cadSala").show();
                    IncluirAlterar(0, 'divAlterarSala', 'divIncluirSala', 'divExcluirSala', 'apresentadorMensagemSala', 'divCancelarSala', 'divClearSala');
                }, true);

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridSala, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_sala', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridSala')", gridSala.rowsPerPage * 3);
                    });
                });

                /// Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        eventoEditar(gridSala.itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        eventoRemover(gridSala.itensSelecionados, 'deletarSala(itensSelecionados)');
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
                        buscarTodosItens(gridSala, 'todosItens', ['searchSala', 'relatorioSala']);
                        searchSala(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () {
                        buscarItensSelecionados('gridSala', 'selecionado', 'cd_sala', 'selecionaTodos', ['searchSala', 'relatorioSala'], 'todosItens');
                    }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);


                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        incluirSala();
                    }
                }, "incluirSala");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        alterarSala(false);
                    }
                }, "alterarSala");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarSala() });
                    }
                }, "deleteSala");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        clearForm("formSala")
                    }
                }, "limparSala");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () { keepValues(FORM_SALA, null, gridSala, null); }
                }, "cancelarSala");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("cadSala").hide(); }
                }, "fecharSala");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        searchSala(true);
                    }
                }, "searchSala");
                decreaseBtn(document.getElementById("searchSala"), '32px');
                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                    onClick: function () {
                        dijit.byId("no_sala").set("disabled", false);
                        dijit.byId("cadSala").show();
                        getLimpar('#formSala');
                        clearForm("formSala");
                        IncluirAlterar(1, 'divAlterarSala', 'divIncluirSala', 'divExcluirSala', 'apresentadorMensagemSala', 'divCancelarSala', 'divClearSala');
                    }
                }, "novaSala");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        require(["dojo/_base/xhr"], function (xhr) {
                            xhr.get({
                                url: !hasValue(document.getElementById("salaDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatoriosala?" + getStrGridParameters('gridSala') + "desc=&inicio=" + document.getElementById("inicioSala").checked + "&status=" + retornaStatus("statusSala") + "&cdEscola=" + Escola() + "&online=" + dijit.byId("ckOnline").checked : Endereco() + "/api/coordenacao/geturlrelatoriosala?" + getStrGridParameters('gridSala') + "desc=" + encodeURIComponent(document.getElementById("salaDesc").value) + "&inicio=" + document.getElementById("inicioSala").checked + "&status=" + retornaStatus("statusSala") + "&cdEscola=" + Escola() + "&online=" + dijit.byId("ckOnline").checked,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                            },
						    function (error) {
						        apresentaMensagem('apresentadorMensagem', error);
						    });
                        })
                    }
                }, "relatorioSala");
                if (hasValue(dijit.byId("menuManual"))) {
                    var menuManual = dijit.byId("menuManual");
                    if (hasValue(menuManual.handler))
                        menuManual.handler.remove();
                    menuManual.handler = menuManual.on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322982', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['salaDesc', 'statusSala'], 'searchSala', ready);

                if (!eval(MasterGeral())) 
                    dojo.byId('tagAulaOnLine').style.display = 'none'
                else
                    dojo.byId('tagAulaOnLine').style.display = 'block'

                // Final do carregamento dos componentes do dojo, volta o carregando:
                showCarregando();
                dijit.byId("tabContainer_tablist_menuBtn").on("click", function () {
                    try {
                        //dijit.byId("tabContainer_menu").on("_create", function () {
                        if (hasValue(dijit.byId("tabContainer_menu")) && dijit.byId("tabContainer_menu")._created) {

                            dijit.byId("tabSala_stcMi").on("click", function () {
                                abrirTabSala();
                            });
                            dijit.byId("tabProduto_stcMi").on("click", function () {
                                abrirTabProduto();
                            });
                            dijit.byId("tabEstagio_stcMi").on("click", function () {
                                abrirTabEstagio();
                            });
                            dijit.byId("tabDuracao_stcMi").on("click", function () {
                                abrirTabDuracao();
                            });
                            dijit.byId("tabModalidade_stcMi").on("click", function () {
                                abrirTabModalidade();
                            });
                            dijit.byId("tabRegime_stcMi").on("click", function () {
                                abrirTabRegime();
                            });
                            dijit.byId("tabConceito_stcMi").on("click", function () {
                                abrirTabConceito();
                            });
                            dijit.byId("tabFeriado_stcMi").on("click", function () {
                                abrirTabFeriado();
                            });
                            dijit.byId("tabEvento_stcMi").on("click", function () {
                                abrirTabEvento();
                            });
                            dijit.byId("tabMotivoFalta_stcMi").on("click", function () {
                                AbrirMotivoFalta();
                            });
                            dijit.byId("tabMotivoDesistencia_stcMi").on("click", function () {
                                abrirTabMotivoDesistencia();
                            });
                            dijit.byId("tabAtividadeExtra_stcMi").on("click", function () {
                                abrirTabAtividadeExtra();
                            });
                            dijit.byId("tabParticipacao_stcMi").on("click", function () {
                                abrirTabParticipacao();
                            });
                            dijit.byId("tabCargaProfessor_stcMi").on("click", function () {
                                abrirTabCargaProfessor();
                            });

                            dijit.byId("tabNivel_stcMi").on("click", function () {
                                abrirTabNivel();
                            });

                            dijit.byId("tabMensagemAvaliacao_stcMi").on("click", function () {
                                abrirTabMensagemAvaliacao();
                            });
                        }
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });

    // ** fim da grade de sala **\\
}
//#endregion

//#region montarCadastroProduto
function montarCadastroProduto() {
    require([
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
        "dijit/MenuItem",
        "dijit/Dialog"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getProdutoSearch?desc=&abrev&inicio=false&status=1",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_produto"
		    }), Memory({ idProperty: "cd_produto" }));

            var gridProduto = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
			    { name: "<input id='selecionaTodosProdutos' style='display:none'/>", field: "selecionadoProduto", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxProduto },
			    //{ name: "Código", field: "cd_produto", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
			    { name: "Produto", field: "no_produto", width: "85%" },
                { name: "Abreviado", field: "no_produto_abreviado", width: "10%" },
			    { name: "Ativo", field: "produtoAtivo", width: "5%", styles: "text-align: center;" }
                ],
                canSort: true,
                noDataMessage: msgNotRegEnc,
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
            }, "gridProduto");
            //Paginação
            gridProduto.pagination.plugin._paginator.plugin.connect(gridProduto.pagination.plugin._paginator, 'onSwitchPageSize',
			    function (evt) {
			        verificaMostrarTodos(evt, gridProduto, 'cd_produto', 'selecionaTodosProdutos');
			    });
            //Seleciona todos e impede que a primeira coluna seja ordenada
            gridProduto.canSort = function (col) { return Math.abs(col) != 1; };
            apresentaMensagem('apresentadorMensagem', null);
            gridProduto.startup();
            gridProduto.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
			        item = this.getItem(idx),
			        store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formProduto');
                    clearForm("formProduto");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_PRODUTO, item, gridProduto, false);
                    dijit.byId("cadProduto").show();
                    IncluirAlterar(0, 'divAlterarProduto', 'divIncluirProduto', 'divExcluirProduto', 'apresentadorMensagem', 'divCancelarProduto', 'divClearProduto');
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridProduto, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosProdutos').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_produto', 'selecionadoProduto', -1, 'selecionaTodosProdutos', 'selecionaTodosProdutos', 'gridProduto')", gridProduto.rowsPerPage * 3);
                });
            });

            /// Adiciona link de ações:
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
                    eventoEditarProduto(gridProduto.itensSelecionados);
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
                    eventoRemover(gridProduto.itensSelecionados, 'deletarProduto(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadas",
                dropDown: menu,
                id: "acoesRelacionadasProduto"
            });
            dom.byId("linkAcoesProduto").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridProduto, 'todosItensProduto', ['searchProduto', 'relatorioProduto']);
                    searchProduto(false);
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionadosProduto = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () {
                    buscarItensSelecionados('gridProduto', 'selecionadoProduto', 'cd_produto', 'selecionaTodosProdutos', ['searchProduto', 'relatorioSala'], 'todosItensProduto');
                }
            });
            menu.addChild(menuItensSelecionadosProduto);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItens",
                dropDown: menu,
                id: "todosItensProduto"
            });
            dom.byId("linkSelecionadosProduto").appendChild(button.domNode);


            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { incluirProduto(); }
            }, "incluirProduto");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    alterarProduto(false)
                }
            }, "alterarProduto");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarProduto(); });
                }
            }, "deleteProduto");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formProduto') }
            }, "limparProduto");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () { keepValues(FORM_PRODUTO, null, gridProduto, null); }
            }, "cancelarProduto");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadProduto").hide();
                }
            }, "fecharProduto");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchProduto(true);
                }
            }, "searchProduto");
            decreaseBtn(document.getElementById("searchProduto"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadProduto").show();
                        getLimpar('#formProduto');
                        clearForm("formProduto");
                        IncluirAlterar(1, 'divAlterarProduto', 'divIncluirProduto', 'divExcluirProduto', 'apresentadorMensagem', 'divCancelarProduto', 'divClearProduto');
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                }
            }, "novoProduto");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("produtoDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatorioproduto?" + getStrGridParameters('gridProduto') + "desc=&abrev=" + encodeURIComponent(document.getElementById("produtoAbrev").value) + "&inicio=" + document.getElementById("inicioProduto").checked + "&status=" + retornaStatus("statusProduto") : Endereco() + "/api/coordenacao/geturlrelatorioevento?" + getStrGridParameters('gridProduto') + "desc=" + encodeURIComponent(document.getElementById("produtoDesc").value) + "&abrev=" + encodeURIComponent(document.getElementById("produtoAbrev").value) + "&inicio=" + document.getElementById("inicioProduto").checked + "&status=" + retornaStatus("statusProduto"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioProduto");
            adicionarAtalhoPesquisa(['produtoDesc', 'produtoAbrev', 'statusProduto'], 'searchProduto', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region montarCadastroEvento
function montarCadastroEvento() {
    // ** Inicio da criação da grade de Eventos **\\
    require([
		   "dojo/dom",
		   "dojox/grid/EnhancedGrid",
		   "dojox/grid/enhanced/plugins/Pagination",
		   "dojo/store/JsonRest",
		   "dojo/data/ObjectStore",
		   "dojo/store/Cache",
		   "dojo/store/Memory",
		   "dijit/form/Button",
		   "dijit/form/DropDownButton",
		   "dijit/DropDownMenu",
		   "dijit/MenuItem",
           "dojo/ready"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getEventoSearch?desc=&inicio=false&status=1",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_evento"
		    }), Memory({ idProperty: "cd_evento" }));

            var gridEvento = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
			    { name: "<input id='selecionaTodosEventos' style='display:none'/>", field: "selecionadoEvento", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxEvento },
			    //{ name: "Código", field: "cd_evento", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
			    { name: "Evento", field: "no_evento", width: "90%" },
			    { name: "Ativo", field: "eventoAtivo", width: "10%", styles: "text-align: center;" }
                ],
                canSort: true,
                noDataMessage: msgNotRegEnc,
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
            }, "gridEvento");

            gridEvento.pagination.plugin._paginator.plugin.connect(gridEvento.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridEvento, 'cd_evento', 'selecionaTodosEventos');
            });
            gridEvento.startup();
            gridEvento.canSort = function (col) { return Math.abs(col) != 1; };

            gridEvento.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
				        item = this.getItem(idx),
				        store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formEvento');
                    clearForm("formEvento");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_EVENTO, item, gridEvento, false);
                    dijit.byId("cadEvento").show();
                    IncluirAlterar(0, 'divAlterarEvento', 'divIncluirEvento', 'divExcluirEvento', 'apresentadorMensagem', 'divCancelarEvento', 'divClearEvento');
                }
                catch (er) {
                    postGerarLog(er);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridEvento, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosEventos').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_evento', 'selecionadoEvento', -1, 'selecionaTodosEventos', 'selecionaTodosEventos', 'gridEvento')", gridEvento.rowsPerPage * 3);
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
                    eventoEditarEvento(gridEvento.itensSelecionados);
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
                    eventoRemover(gridEvento.itensSelecionados, 'deletarEvento(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasEvento",
                dropDown: menu,
                id: "acoesRelacionadasEvento"
            });
            dom.byId("linkAcoesEvento").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridEvento, 'todosItensEvento', ['searchEvento', 'relatorioEvento']); searchEvento(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridEvento', 'selecionadoEvento', 'cd_evento', 'selecionaTodosEvento', ['gridEvento', 'relatorioEvento'], 'todosItensEvento'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensEvento",
                dropDown: menu,
                id: "todosItensEvento"
            });
            dom.byId("linkSelecionadosEventos").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { incluirEvento() }
            }, "incluirEvento");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { alterarEvento(false) }
            }, "alterarEvento");

            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarEvento(); });
                }
            }, "deleteEvento");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formEvento') }
            }, "limparEvento");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () { keepValues(FORM_EVENTO, null, gridEvento, null); }
            }, "cancelarEvento");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadEvento").hide();
                }
            }, "fecharEvento");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchEvento(true);
                }
            }, "searchEvento");
            decreaseBtn(document.getElementById("searchEvento"), '32px');
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }

                        dijit.byId("cadEvento").show();
                        getLimpar('#formEvento');
                        clearForm("formEvento");
                        IncluirAlterar(1, 'divAlterarEvento', 'divIncluirEvento', 'divExcluirEvento', 'apresentadorMensagem', 'divCancelarEvento', 'divClearEvento');
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                }
            }, "novoEvento");

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("eventoDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatorioevento?" + getStrGridParameters('gridEvento') + "desc=&inicio=" + document.getElementById("inicioEvento").checked + "&status=" + retornaStatus("statusEvento") : Endereco() + "/api/coordenacao/geturlrelatorioevento?" + getStrGridParameters('gridEvento') + "desc=" + encodeURIComponent(document.getElementById("eventoDesc").value) + "&inicio=" + document.getElementById("inicioEvento").checked + "&status=" + retornaStatus("statusEvento"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
				        function (error) {
				            apresentaMensagem('apresentadorMensagem', error);
				        });
                    })
                }
            }, "relatorioEvento");

            adicionarAtalhoPesquisa(['eventoDesc', 'statusEvento'], 'searchEvento', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//#endregion

//#region montarCadastroDuracao
function montarCadastroDuracao() {
    // ** Inicio da criação da grade de Duração **\\
    require([
		   "dojo/dom",
		   "dojox/grid/EnhancedGrid",
		   "dojox/grid/enhanced/plugins/Pagination",
		   "dojo/store/JsonRest",
		   "dojo/data/ObjectStore",
		   "dojo/store/Cache",
		   "dojo/store/Memory",
		   "dijit/form/Button",
		   "dijit/form/DropDownButton",
		   "dijit/DropDownMenu",
		   "dijit/MenuItem",
           "dojo/ready"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getDuracaoSearch?desc=&inicio=false&status=1",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_duracao"
		    }), Memory({ idProperty: "cd_duracao" }));

            var gridDuracao = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
			     { name: "<input id='selecionaTodosDuracao' style='display:none'/>", field: "selecionadoDuracao", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxDuracao },
			    // { name: "Código", field: "cd_duracao", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
			     { name: "Carga Horária", field: "dc_duracao", width: "90%" },
			     { name: "Ativa", field: "duracaoAtiva", width: "10%", styles: "text-align: center;" }
                ],
                canSort: true,
                noDataMessage: msgNotRegEnc,
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
            }, "gridDuracao");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridDuracao.pagination.plugin._paginator.plugin.connect(gridDuracao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridDuracao, 'cd_duracao', 'selecionaTodosDuracao');
            });
            gridDuracao.startup();
            gridDuracao.canSort = function (col) { return Math.abs(col) != 1; };

            gridDuracao.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
					          item = this.getItem(idx),
					          store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formDuracao');
                    clearForm("formDuracao");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_DURACAO, item, gridDuracao, false);
                    dijit.byId("cadDuracao").show();
                    IncluirAlterar(0, 'divAlterarDuracao', 'divIncluirDuracao', 'divExcluirDuracao', 'apresentadorMensagem', 'divCancelarDuracao', 'divClearDuracao');
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridDuracao, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosDuracao').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_duracao', 'selecionadoDuracao', -1, 'selecionaTodosDuracao', 'selecionaTodosDuracao', 'gridDuracao')", gridDuracao.rowsPerPage * 3);
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
                    eventoEditarDuracao(gridDuracao.itensSelecionados);
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
                    eventoRemover(gridDuracao.itensSelecionados, 'deletarDuracao(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasMid",
                dropDown: menu,
                id: "acoesRelacionadasDuracao"
            });
            dom.byId("linkAcoesDuracao").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridDuracao, 'todosItensDuracao', ['searchDuracao', 'relatorioDuracao']); searchDuracao(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridDuracao', 'selecionadoDuracao', 'cd_duracao', 'selecionaTodosDuracao', ['searchDuracao', 'relatorioDuracao'], 'todosItensDuracao'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensDuracao",
                dropDown: menu,
                id: "todosItensDuracao"
            });
            dom.byId("linkSelecionadosDuracao").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { incluirDuracao() }
            }, "incluirDuracao");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { alterarDuracao() }
            }, "alterarDuracao");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarDuracao() });
                }
            }, "deleteDuracao");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formDuracao') }
            }, "limparDuracao");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () { keepValues(FORM_DURACAO, null, gridDuracao, null); }
            }, "cancelarDuracao");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadDuracao").hide();
                }
            }, "fecharDuracao");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchDuracao(true);
                }
            }, "searchDuracao");
            decreaseBtn(document.getElementById("searchDuracao"), '32px');
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadDuracao").show();
                        getLimpar('#formDuracao');
                        clearForm("formDuracao");
                        IncluirAlterar(1, 'divAlterarDuracao', 'divIncluirDuracao', 'divExcluirDuracao', 'apresentadorMensagem', 'divCancelarDuracao', 'divClearDuracao');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novaDuracao");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("duracaoDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatorioduracao?" + getStrGridParameters('gridDuracao') + "desc=&inicio=" + document.getElementById("inicioDuracao").checked + "&status=" + retornaStatus("statusDuracao") : Endereco() + "/api/coordenacao/geturlrelatorioduracao?" + getStrGridParameters('gridDuracao') + "desc=" + encodeURIComponent(document.getElementById("duracaoDesc").value) + "&inicio=" + document.getElementById("inicioDuracao").checked + "&status=" + retornaStatus("statusDuracao"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioDuracao");
            adicionarAtalhoPesquisa(['duracaoDesc', 'statusDuracao'], 'searchDuracao', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region montarCadastroAtividadeExtra
function montarCadastroAtividadeExtra() {
    // ** Inicio da criação da grade de Atividade Extra**\\
    require([
		   "dojo/dom",
		   "dojox/grid/EnhancedGrid",
		   "dojox/grid/enhanced/plugins/Pagination",
		   "dojo/store/JsonRest",
		   "dojo/data/ObjectStore",
		   "dojo/store/Cache",
		   "dojo/store/Memory",
		   "dijit/form/Button",
		   "dijit/form/TextBox",
		   "dijit/form/DropDownButton",
		   "dijit/DropDownMenu",
		   "dijit/MenuItem",
           "dojo/ready"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, TextBox, DropDownButton, DropDownMenu, MenuItem, ready) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getTpAtividadeExtraSearch?desc=&inicio=false&status=1",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_atividade_extra"
		    }), Memory({ idProperty: "cd_atividade_extra" }));

            var gridAtividadeExtra = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
			    { name: "<input id='selecionaTodasAtividadesExtras' style='display:none'/>", field: "selecionadaAtividadeExtra", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAtividadeExtra },
			    //{ name: "Código", field: "cd_tipo_atividade_extra", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
			    { name: "Tipo de Atividade Extra", field: "no_tipo_atividade_extra", width: "90%" },
     		    { name: "Ativa", field: "atividadeExtraAtiva", width: "10%", styles: "text-align: center;" }
                ],
                canSort: true,
                noDataMessage: msgNotRegEnc,
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
            }, "gridAtividadeExtra");
            gridAtividadeExtra.pagination.plugin._paginator.plugin.connect(gridAtividadeExtra.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridAtividadeExtra, 'cd_tipo_atividade_extra', 'selecionaTodasAtividadesExtras');
            });
            gridAtividadeExtra.startup();
            gridAtividadeExtra.canSort = function (col) { return Math.abs(col) != 1; };
            gridAtividadeExtra.on("RowDblClick", function (evt) {
                try {

                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }

                    var idx = evt.rowIndex,
					         item = this.getItem(idx),
					         store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formAtividadeExtra');
                    clearForm("formAtividadeExtra");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_ATIVIDADEEXTRA, item, gridAtividadeExtra);
                    dijit.byId("cadAtividadeExtra").show();
                    IncluirAlterar(0, 'divAlterarAtividadeExtra', 'divIncluirAtividadeExtra', 'divExcluirAtividadeExtra', 'apresentadorMensagem', 'divCancelarAtividadeExtra', 'divClearAtividadeExtra');
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridAtividadeExtra, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodasAtividadesExtras').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_tipo_atividade_extra', 'selecionadaAtividadeExtra', -1, 'selecionaTodasAtividadesExtras', 'selecionaTodasAtividadesExtras', 'gridAtividadeExtra')", gridAtividadeExtra.rowsPerPage * 3);
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
                    eventoEditarAtividadeExtra(gridAtividadeExtra.itensSelecionados);
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
                    eventoRemover(gridAtividadeExtra.itensSelecionados, 'deletarAtividadeExtra(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasAtividadeExtra",
                dropDown: menu,
                id: "acoesRelacionadasAtividadeExtra"
            });
            dom.byId("linkAcoesAtividadeExtra").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridAtividadeExtra, 'todosItensAtividadeExtra', ['searchAtividadeExtra', 'relatorioAtividadeExtra']); searchAtividadeExtra(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridAtividadeExtra', 'selecionadaAtividadeExtra', 'cd_tipo_atividade_extra', 'selecionaTodasAtividadesExtras', ['searchAtividadeExtra', 'relatorioAtividadeExtra'], 'todosItensAtividadeExtra'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensAtividadeExtra",
                dropDown: menu,
                id: "todosItensAtividadeExtra"
            });
            dom.byId("linkSelecionadosAtividadeExtra").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { incluirAtividadeExtra(); }
            }, "incluirAtividadeExtra");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { alterarAtividadeExtra(); }
            }, "alterarAtividadeExtra");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarAtividadeExtra() }); }
            }, "deleteAtividadeExtra");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formAtividadeExtra') }
            }, "limparAtividadeExtra");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () { keepValues(FORM_ATIVIDADEEXTRA, null, gridAtividadeExtra, null); }
            }, "cancelarAtividadeExtra");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadAtividadeExtra").hide();
                }
            }, "fecharAtividadeExtra");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    searchAtividadeExtra(true);
                }
            }, "searchAtividadeExtra");
            decreaseBtn(document.getElementById("searchAtividadeExtra"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }

                    dijit.byId("cadAtividadeExtra").show();
                    getLimpar('#formAtividadeExtra');
                    clearForm("formAtividadeExtra");
                    IncluirAlterar(1, 'divAlterarAtividadeExtra', 'divIncluirAtividadeExtra', 'divExcluirAtividadeExtra', 'apresentadorMensagem', 'divCancelarAtividadeExtra', 'divClearAtividadeExtra');
                }
            }, "novaAtividadeExtra");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("atividadeExtraDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatoriotipoatividadeextra?" + getStrGridParameters('gridAtividadeExtra') + "desc=&inicio=" + document.getElementById("inicioAtividadeExtra").checked + "&status=" + retornaStatus("statusAtividadeExtra") : Endereco() + "/api/coordenacao/geturlrelatoriotipoatividadeextra?" + getStrGridParameters('gridAtividadeExtra') + "desc=" + encodeURIComponent(document.getElementById("atividadeExtraDesc").value) + "&inicio=" + document.getElementById("inicioAtividadeExtra").checked + "&status=" + retornaStatus("statusAtividadeExtra"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioAtividadeExtra");

            adicionarAtalhoPesquisa(['atividadeExtraDesc', 'statusAtividadeExtra'], 'searchAtividadeExtra', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region montarCadastroMotivoDesistencia
function montarCadastroMotivoDesistencia() {
    // ** Inicio da criação da grade de Motivo da Desitência**\\
    require([
			"dojo/dom",
			"dojox/grid/EnhancedGrid",
			"dojox/grid/enhanced/plugins/Pagination",
			"dojo/store/JsonRest",
			"dojo/data/ObjectStore",
			"dojo/store/Cache",
			"dojo/store/Memory",
			"dijit/form/Button",
			"dijit/form/DropDownButton",
			"dijit/DropDownMenu",
			"dijit/MenuItem",
            "dojo/ready"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try {
            var myStore = Cache(
		    JsonRest({//dijit.byId("statusMotivoDesistencia").get("value")
		        target: Endereco() + "/api/coordenacao/getMotivoDesistenciaSearch?desc=&inicio=false&status=1&isCancelamento=false",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_motivo_desistencia"
		    }), Memory({ idProperty: "cd_motivo_desistencia" }));

            var gridMotivoDesistencia = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
				    { name: "<input id='selecionaTodosMotivosDesistencia' style='display:none'/>", field: "selecionadoMotivoDesistencia", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMotivoDesistencia },
				    //{ name: "Código", field: "cd_motivo_desistencia", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
				    { name: "Motivo Desistência", field: "dc_motivo_desistencia", width: "80%" },
                    { name: "Cancelamento", field: "isCancelamento", width: "10%", styles: "text-align: center;" },
				    { name: "Ativo", field: "motivoDesistenciaAtivo", width: "10%", styles: "text-align: center;" }
                ],
                canSort: true,
                noDataMessage: msgNotRegEnc,
                plugins: {
                    pagination: {
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
            }, "gridMotivoDesistencia");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridMotivoDesistencia.pagination.plugin._paginator.plugin.connect(gridMotivoDesistencia.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridMotivoDesistencia, 'cd_motivo_desistencia', 'selecionadoMotivoDesistencia');
            });
            gridMotivoDesistencia.startup();
            gridMotivoDesistencia.canSort = function (col) { return Math.abs(col) != 1; };
            gridMotivoDesistencia.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
				        item = this.getItem(idx),
				        store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formMotivoDesistencia');
                    clearForm("formMotivoDesistencia");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_MOTIVODESISTENCIA, item, gridMotivoDesistencia);
                    IncluirAlterar(0, 'divAlterarMotivoDesistencia', 'divIncluirMotivoDesistencia', 'divExcluirMotivoDesistencia', 'apresentadorMensagem', 'divCancelarMotivoDesistencia', 'divClearMotivoDesistencia');
                    dijit.byId("cadMotivoDesistencia").show();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridMotivoDesistencia, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosMotivosDesistencia').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_motivo_desistencia', 'selecionadoMotivoDesistencia', -1, 'selecionaTodosMotivosDesistencia', 'selecionaTodosMotivosDesistencia', 'gridMotivoDesistencia')", gridMotivoDesistencia.rowsPerPage * 3);
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
                    eventoEditarMotivoDesistencia(gridMotivoDesistencia.itensSelecionados);
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
                    eventoRemover(gridMotivoDesistencia.itensSelecionados, 'deletarMotivoDesistencia(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasMotivoDesistencia",
                dropDown: menu,
                id: "acoesRelacionadasMotivoDesistencia"
            });
            dom.byId("linkAcoesMotivoDesistencia").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridMotivoDesistencia, 'todosItensMotivosDesistencia', ['searchMotivoDesistencia', 'relatorioMotivoDesistencia']); searchMotivoDesistencia(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridMotivoDesistencia', 'selecionaTodosMotivosDesistencia', 'cd_motivo_desistencia', 'selecionaTodosMotivosDesistencia', ['searchMotivoDesistencia', 'relatorioMotivoDesistencia'], 'todosItensMotivosDesistencia'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensMotivosDesistencia",
                dropDown: menu,
                id: "todosItensMotivosDesistencia"
            });
            dom.byId("linkSelecionadosMotivosDesistencia").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { incluirMotivoDesistencia(); }
            }, "incluirMotivoDesistencia");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { alterarMotivoDesistencia(); }
            }, "alterarMotivoDesistencia");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarMotivoDesistencia(); });
                }
            }, "deleteMotivoDesistencia");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formMotivoDesistencia') }
            }, "limparMotivoDesistencia");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () { keepValues(FORM_MOTIVODESISTENCIA, null, gridMotivoDesistencia, null); }
            }, "cancelarMotivoDesistencia");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadMotivoDesistencia").hide();
                }
            }, "fecharMotivoDesistencia");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchMotivoDesistencia(true);
                }
            }, "searchMotivoDesistencia");
            decreaseBtn(document.getElementById("searchMotivoDesistencia"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }

                        getLimpar('#formMotivoDesistencia');
                        clearForm("formMotivoDesistencia");
                        IncluirAlterar(1, 'divAlterarMotivoDesistencia', 'divIncluirMotivoDesistencia', 'divExcluirMotivoDesistencia', 'apresentadorMensagem', 'divCancelarMotivoDesistencia', 'divClearMotivoDesistencia');
                        dijit.byId("cadMotivoDesistencia").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novaMotivoDesistencia");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("motivoDesistenciaDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatoriomotivodesistencia?" + getStrGridParameters('gridMotivoDesistencia') + "desc=&inicio=" + document.getElementById("inicioMotivoDesistencia").checked + "&status=" + retornaStatus("statusMotivoDesistencia") + '&isCancelamento=' + dijit.byId('isCancelamento').checked : Endereco() + "/api/coordenacao/geturlrelatoriomotivodesistencia?" + getStrGridParameters('gridMotivoDesistencia') + "desc=" + encodeURIComponent(document.getElementById("motivoDesistenciaDesc").value) + "&inicio=" + document.getElementById("inicioMotivoDesistencia").checked + "&status=" + retornaStatus("statusMotivoDesistencia") + '&isCancelamento=' + dijit.byId('isCancelamento').checked,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioMotivoDesistencia");

            adicionarAtalhoPesquisa(['motivoDesistenciaDesc', 'statusMotivoDesistencia'], 'searchMotivoDesistencia', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region montarCadastroMotivoFalta
function montarCadastroMotivoFalta() {
    // ** Inicio da criação da grade de Motivo da Falta**\\
    require([
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
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getMotivoFaltaSearch?desc=&inicio=false&status=1",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_motivo_falta"
		    }), Memory({ idProperty: "cd_motivo_falta" }));

            var gridMotivoFalta = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
			       { name: "<input id='selecionaTodosMotivosFalta' style='display:none'/>", field: "selecionadoMotivoFalta", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMotivosFalta },
			      // { name: "Código", field: "cd_motivo_falta", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
			       { name: "Motivo Falta", field: "dc_motivo_falta", width: "90%" },
			       { name: "Ativo", field: "motivoFaltaAtiva", width: "10%", styles: "text-align: center;" }
                ],
                canSort: true,
                noDataMessage: msgNotRegEnc,
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
            }, "gridMotivoFalta");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridMotivoFalta.pagination.plugin._paginator.plugin.connect(gridMotivoFalta.pagination.plugin._paginator, 'onSwitchPageSize',
			    function (evt) {
			        verificaMostrarTodos(evt, gridMotivoFalta, 'cd_motivo_falta', 'selecionaTodosMotivosFalta');
			    });
            gridMotivoFalta.canSort = function (col) { return Math.abs(col) != 1; };
            gridMotivoFalta.startup();
            gridMotivoFalta.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
					          item = this.getItem(idx),
					          store = this.store;

                    // Limpar os dados do form
                    getLimpar('#formMotivoFalta');
                    clearForm("formMotivoFalta");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_MOTIVOFALTA, item, gridMotivoFalta, false);
                    dijit.byId("cadMotivoFalta").show();
                    IncluirAlterar(0, 'divAlterarMotivoFalta', 'divIncluirMotivoFalta', 'divExcluirMotivoFalta', 'apresentadorMensagem', 'divCancelarMotivoFalta', 'divClearMotivoFalta');
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridMotivoFalta, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosMotivosFalta').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_motivo_falta', 'selecionadoMotivoFalta', -1, 'selecionaTodosMotivosFalta', 'selecionaTodosMotivosFalta', 'gridMotivoFalta')", gridMotivoFalta.rowsPerPage * 3);
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
                    eventoEditarMotivoFalta(gridMotivoFalta.itensSelecionados);
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
                    eventoRemover(gridMotivoFalta.itensSelecionados, 'deletarMotivoFalta(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasMotivoFalta",
                dropDown: menu,
                id: "acoesRelacionadasMotivoFalta"
            });
            dom.byId("linkAcoesMotivoFalta").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridMotivoFalta, 'todosItensMotivoFalta', ['searchMotivoFalta', 'relatorioMotivoFalta']); searchMotivoFalta(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridMotivoFalta', 'selecionadoMotivoFalta', 'cd_motivo_falta', 'selecionaTodosMotivosFalta', ['searchMotivoFalta', 'relatorioMotivoFalta'], 'todosItensMotivoFalta'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensMotivoFalta",
                dropDown: menu,
                id: "todosItensMotivoFalta"
            });
            dom.byId("linkSelecionadosMotivosFalta").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { incluirMotivoFalta(); }
            }, "incluirMotivoFalta");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { alterarMotivoFalta(); }
            }, "alterarMotivoFalta");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarMotivoFalta(); });
                }
            }, "deleteMotivoFalta");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formMotivoFalta') }
            }, "limparMotivoFalta");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () { keepValues(FORM_MOTIVOFALTA, null, gridMotivoFalta, null); }
            }, "cancelarMotivoFalta");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadMotivoFalta").hide();
                }
            }, "fecharMotivoFalta");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchMotivoFalta(true);
                }
            }, "searchMotivoFalta");
            decreaseBtn(document.getElementById("searchMotivoFalta"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadMotivoFalta").show();
                        getLimpar('#formMotivoFalta');
                        clearForm("formMotivoFalta");
                        IncluirAlterar(1, 'divAlterarMotivoFalta', 'divIncluirMotivoFalta', 'divExcluirMotivoFalta', 'apresentadorMensagem', 'divCancelarMotivoFalta', 'divClearMotivoFalta');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoMotivoFalta");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("motivoFaltaDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatoriomotivofalta?" + getStrGridParameters('gridMotivoFalta') + "desc=&inicio=" + document.getElementById("inicioMotivoFalta").checked + "&status=" + retornaStatus("statusMotivoFalta") : Endereco() + "/api/coordenacao/geturlrelatoriomotivofalta?" + getStrGridParameters('gridMotivoFalta') + "desc=" + encodeURIComponent(document.getElementById("motivoFaltaDesc").value) + "&inicio=" + document.getElementById("inicioMotivoFalta").checked + "&status=" + retornaStatus("statusMotivoFalta"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioMotivoFalta");
            adicionarAtalhoPesquisa(['motivoFaltaDesc', 'statusMotivoFalta'], 'searchMotivoFalta', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region montarCadastroModalidade
function montarCadastroModalidade() {
    // ** Inicio da criação da grade de Modalidade**\\
    require([
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
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try {
            var myStore = Cache(
		    JsonRest({//dijit.byId("statusMotivoDesistencia").get("value")
		        target: Endereco() + "/api/coordenacao/getModalidadeSearch?desc=&inicio=false&status=1",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_modalidade"
		    }), Memory({ idProperty: "cd_modalidade" }));

            var gridModalidade = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
			    { name: "<input id='selecionaTodasModalidades' style='display:none'/>", field: "selecionadoModalidade", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxModalidade },
			    //{ name: "Código", field: "cd_modalidade", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
			    { name: "Série", field: "no_modalidade", width: "90%" },
			    { name: "Ativa", field: "modalidadeAtiva", width: "10%", styles: "text-align: center;" }
                ],
                canSort: true,
                noDataMessage: msgNotRegEnc,
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
            }, "gridModalidade");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridModalidade.pagination.plugin._paginator.plugin.connect(gridModalidade.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridModalidade, 'cd_modalidade', 'selecionaTodasModalidades'); });
            gridModalidade.canSort = function (col) { return Math.abs(col) != 1; };
            gridModalidade.startup();
            gridModalidade.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
					          item = this.getItem(idx),
					          store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formModalidade');
                    clearForm("formModalidade");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_MODALIDADE, item, gridModalidade, null);
                    dijit.byId("cadModalidade").show();
                    IncluirAlterar(0, 'divAlterarModalidade', 'divIncluirModalidade', 'divExcluirModalidade', 'apresentadorMensagem', 'divCancelarModalidade', 'divClearModalidade');
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridModalidade, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodasModalidades').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_modalidade', 'selecionadoModalidade', -1, 'selecionaTodasModalidades', 'selecionaTodasModalidades', 'gridModalidade')", gridModalidade.rowsPerPage * 3);
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
                    eventoEditarModalidade(gridModalidade.itensSelecionados);
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
                    eventoRemover(gridModalidade.itensSelecionados, 'deletarModalidade(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadas",
                dropDown: menu,
                id: "acoesRelacionadasModalidade"
            });
            dom.byId("linkAcoesModalidade").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridModalidade, 'todosItensModalidade', ['searchModalidade', 'relatorioModalidade']);
                    searchModalidade(false);
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridModalidade', 'selecionadoModalidade', 'cd_modalidade', 'selecionaTodasModalidades', ['searchModalidade', 'relatorioModalidade'], 'todosItensModalidade'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItens",
                dropDown: menu,
                id: "todosItensModalidade"
            });
            dom.byId("linkSelecionadasModalidades").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { incluirModalidade() }
            }, "incluirModalidade");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { alterarModalidade() }
            }, "alterarModalidade");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarModalidade(); });
                }
            }, "deleteModalidade");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formModalidade') }
            }, "limparModalidade");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () { keepValues(FORM_MODALIDADE, null, gridModalidade, null) }
            }, "cancelarModalidade");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadModalidade").hide();
                }
            }, "fecharModalidade");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchModalidade(true);
                }
            }, "searchModalidade");
            decreaseBtn(document.getElementById("searchModalidade"), '32px');
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadModalidade").show();
                        getLimpar('#formModalidade');
                        clearForm("formModalidade");
                        IncluirAlterar(1, 'divAlterarModalidade', 'divIncluirModalidade', 'divExcluirModalidade', 'apresentadorMensagem', 'divCancelarModalidade', 'divClearModalidade');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novaModalidade");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("modalidadeDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatoriomodalidade?" + getStrGridParameters('gridModalidade') + "desc=&inicio=" + document.getElementById("inicioModalidade").checked + "&status=" + retornaStatus("statusModalidade") : Endereco() + "/api/coordenacao/geturlrelatoriomodalidade?" + getStrGridParameters('gridModalidade') + "desc=" + encodeURIComponent(document.getElementById("modalidadeDesc").value) + "&inicio=" + document.getElementById("inicioModalidade").checked + "&status=" + retornaStatus("statusModalidade"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioModalidade");

            adicionarAtalhoPesquisa(['modalidadeDesc', 'statusModalidade'], 'searchModalidade', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region montarCadastroRegime
function montarCadastroRegime() {
    // ** Inicio da criação da grade de Regime**\\
    require([
		   "dojo/dom",
		   "dojox/grid/EnhancedGrid",
		   "dojox/grid/enhanced/plugins/Pagination",
		   "dojo/store/JsonRest",
		   "dojo/data/ObjectStore",
		   "dojo/store/Cache",
		   "dojo/store/Memory",
		   "dijit/form/Button",
		   "dijit/form/DropDownButton",
		   "dijit/DropDownMenu",
		   "dijit/MenuItem",
           "dojo/ready"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try {
            var myStore = Cache(
		    JsonRest({//dijit.byId("statusMotivoDesistencia").get("value")
		        target: Endereco() + "/api/coordenacao/getRegimeSearch?desc=&abrev=&inicio=false&status=1&cd_regime=",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_regime"
		    }), Memory({ idProperty: "cd_regime" }));

            var gridRegime = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
				    { name: "<input id='selecionaTodosRegimes' style='display:none'/>", field: "selecionadoRegime", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxRegime },
				    //{ name: "Código", field: "cd_regime", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
				    { name: "Modalidade", field: "no_regime", width: "60%" },
                    { name: "Abreviado", field: "no_regime_abreviado", width: "20%" },
				    { name: "Ativo", field: "regimeAtivo", width: "10%", styles: "text-align: center;" }
                ],
                canSort: true,
                noDataMessage: msgNotRegEnc,
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
            }, "gridRegime");
            gridRegime.pagination.plugin._paginator.plugin.connect(gridRegime.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridRegime, 'cd_regime', 'selecionaTodosRegimes');
            });
            gridRegime.startup();
            gridRegime.canSort = function (col) { return Math.abs(col) != 1; };
            gridRegime.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
			        item = this.getItem(idx),
			        store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formRegime');
                    clearForm("formRegime");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_REGIME, item, gridRegime, null);
                    dijit.byId("cadRegime").show();
                    IncluirAlterar(0, 'divAlterarRegime', 'divIncluirRegime', 'divExcluirRegime', 'apresentadorMensagem', 'divCancelarRegime', 'divClearRegime');
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridRegime, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosRegimes').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_regime', 'selecionadoRegime', -1, 'selecionaTodosRegimes', 'selecionaTodosRegimes', 'gridRegime')", gridRegime.rowsPerPage * 3);
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
                    eventoEditarRegime(gridRegime.itensSelecionados);
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
                    eventoRemover(gridRegime.itensSelecionados, 'deletarRegime(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasRegime",
                dropDown: menu,
                id: "acoesRelacionadasRegime"
            });
            dom.byId("linkAcoesRegime").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridRegime, 'todosItensRegime', ['searchRegime', 'relatorioRegime']); searchRegime(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridRegime', 'selecionadoRegime', 'cd_regime', 'selecionaTodosRegimes', ['searchRegime', 'relatorioRegime'], 'todosItensRegime'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensRegime",
                dropDown: menu,
                id: "todosItensRegime"
            });
            dom.byId("linkSelecionadosRegimes").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { incluirRegime(); }
            }, "incluirRegime");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { alterarRegime(); }
            }, "alterarRegime");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarRegime(); });
                }
            }, "deleteRegime");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formRegime') }
            }, "limparRegime");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () { keepValues(FORM_REGIME, null, gridRegime, null); }
            }, "cancelarRegime");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadRegime").hide();
                }
            }, "fecharRegime");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchRegime(true);
                }
            }, "searchRegime");
            decreaseBtn(document.getElementById("searchRegime"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadRegime").show();
                        getLimpar('#formRegime');
                        clearForm("formRegime");
                        IncluirAlterar(1, 'divAlterarRegime', 'divIncluirRegime', 'divExcluirRegime', 'apresentadorMensagem', 'divCancelarRegime', 'divClearRegime');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novaRegime");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("regimeDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatorioregime?" + getStrGridParameters('gridRegime') + "desc=&abrev=" + encodeURIComponent(document.getElementById("regimeAbrev").value) + "&inicio=" + document.getElementById("inicioRegime").checked + "&status=" + retornaStatus("statusRegime") : Endereco() + "/api/coordenacao/geturlrelatorioregime?" + getStrGridParameters('gridRegime') + "desc=" + encodeURIComponent(document.getElementById("regimeDesc").value) + "&abrev=" + encodeURIComponent(document.getElementById("regimeAbrev").value) + "&inicio=" + document.getElementById("inicioRegime").checked + "&status=" + retornaStatus("statusRegime"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioRegime");
            adicionarAtalhoPesquisa(['regimeDesc', 'regimeAbrev', 'statusRegime'], 'searchRegime', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region montarCadastroConceito
function montarCadastroConceito() {
    // ** Inicio da criação da grade de Conceito**\\
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
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/ready"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getConceitoSearch?desc=&inicio=false&status=1&CodP=0",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_conceito"
		    }), Memory({ idProperty: "cd_conceito" }));

            var gridConceito = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
				    { name: "<input id='selecionaTodosConceitos' style='display:none'/>", field: "selecionadoConceito", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxConceito },
				    //{ name: "Código", field: "cd_conceito", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
				    { name: "Produto", field: "no_produto", width: "20%" },
				    { name: "Conceito", field: "no_conceito", width: "40%" },
				    { name: "Perc.Inicial", field: "pc_inicial", width: "10%", styles: "text-align: right;" },
				    { name: "Perc.Final", field: "pc_final", width: "10%", styles: "text-align: right;" },
                    { name: "Participação", field: "val_nota_participacao", width: "10%", styles: "text-align: right;" },
				    { name: "Ativo", field: "conceito_ativo", width: "10%", styles: "text-align: center; min-width:40px; max-width: 50px;" }],
                canSort: true,
                noDataMessage: msgNotRegEnc,
                //keepSelection: true,
                plugins: {//indirectSelection: { headerSelector: true, width: "40px", styles: "text-align: center;" },
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
            }, "gridConceito");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridConceito.pagination.plugin._paginator.plugin.connect(gridConceito.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridConceito, 'cd_conceito', 'selecionaTodosConceitos');
            });

            gridConceito.startup();
            gridConceito.canSort = function (col) { return Math.abs(col) != 1; };
            gridConceito.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
					          item = this.getItem(idx),
					          store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formConceito');
                    clearForm("formConceito");
                    apresentaMensagem('apresentadorMensagem', '');
                    xhr.get({
                        url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_ATIVO + "&cd_produto=" + item.cd_produto,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }).then(function (dataProdAtivo) {
                        loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, 'cd_produtoC');
                        dijit.byId("cd_produtoC").set("value", item.cd_produto);
                    });
                    //            var storeProd = dijit.byId("codProduto").store;
                    //            dijit.byId("cd_produtoC").store = storeProd;
                    keepValues(FORM_CONCEITO, item, gridConceito, null);
                    dijit.byId("cadConceito").show();
                    IncluirAlterar(0, 'divAlterarConceito', 'divIncluirConceito', 'divExcluirConceito', 'apresentadorMensagem', 'divCancelarConceito', 'divClearConceito');
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridConceito, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosConceitos').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_conceito', 'selecionadoConceito', -1, 'selecionaTodosConceitos', 'selecionaTodosConceitos', 'gridConceito')", gridConceito.rowsPerPage * 3);
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
                    eventoEditarConceito(gridConceito.itensSelecionados);
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
                    eventoRemover(gridConceito.itensSelecionados, 'deletarConceito(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasConceito",
                dropDown: menu,
                id: "acoesRelacionadasConceito"
            });
            dom.byId("linkAcoesConceito").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridConceito, 'todosItensConceito', ['searchConceito', 'relatorioConceito']); searchConceito(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridConceito', 'selecionadoConceito', 'cd_conceito', 'selecionaTodosConceitos', ['searchConceito', 'relatorioConceito'], 'todosItensConceito'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensConceito",
                dropDown: menu,
                id: "todosItensConceito"
            });
            dom.byId("linkSelecionadosConceitos").appendChild(button.domNode);
            //*** Cria os botões de persistência **\\

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { incluirConceito(); }
            }, "incluirConceito");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { alterarConceito(); }
            }, "alterarConceito");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarConceito(); });
                }
            }, "deleteConceito");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                onClick: function () {
                    dijit.byId("cd_produtoC").reset();
                    getLimpar('#formConceito');
                    clearForm("formConceito");
                }
            }, "limparConceito");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () { keepValues(FORM_CONCEITO, null, gridConceito, null); }
            }, "cancelarConceito");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadConceito").hide();
                }
            }, "fecharConceito");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchConceito(true);
                }
            }, "searchConceito");
            decreaseBtn(document.getElementById("searchConceito"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cd_produtoC").reset();
                        dijit.byId("cadConceito").show();
                        getLimpar('#formConceito');
                        clearForm("formConceito");
                        xhr.get({
                            url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_ATIVO + "&cd_produto=null",
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }).then(function (dataProdAtivo) {
                            loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, 'cd_produtoC');
                            dijit.byId("cd_produtoC").set("value", item.cd_produto);
                        });

                        IncluirAlterar(1, 'divAlterarConceito', 'divIncluirConceito', 'divExcluirConceito', 'apresentadorMensagem', 'divCancelarConceito', 'divClearConceito');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoConceito");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("conceitoDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatorioconceito?" + getStrGridParameters('gridConceito') + "desc=&inicio=" + encodeURIComponent(document.getElementById("inicioConceito").checked) + "&status=" + retornaStatus("statusConceito") + "&CodP=" + dijit.byId("codProduto").get("value") : Endereco() + "/api/coordenacao/geturlrelatorioconceito?" + getStrGridParameters('gridConceito') + "desc=" + encodeURIComponent(document.getElementById("conceitoDesc").value) + "&inicio=" + document.getElementById("inicioConceito").checked + "&status=" + retornaStatus("statusConceito") + "&CodP=" + dijit.byId("codProduto").get("value"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '865px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioConceito");
            xhr.get({
                url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_CONCEITO + "&cd_produto=null",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Authorization": Token() }
            }).then(function (dataProd) {
                loadProduto(jQuery.parseJSON(dataProd).retorno, 'codProduto');
                dijit.byId("codProduto").set("value", 0);
            });
            dijit.byId("pc_inicial_conceito").on("change", function (e) {
                if (e >= 100) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPercInicialForaIntervalo);
                    apresentaMensagem("apresentadorMensagemConceito", mensagensWeb);
                }
            });
            adicionarAtalhoPesquisa(['conceitoDesc', 'statusConceito', 'codProduto'], 'searchConceito', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region montarCadastroFeriado
function montarCadastroFeriado() {
    try {
        require([
		      "dojo/dom",
		      "dojox/grid/EnhancedGrid",
		      "dojox/grid/enhanced/plugins/Pagination",
		      "dojo/store/JsonRest",
		      "dojo/data/ObjectStore",
		      "dojo/store/Cache",
		      "dojo/store/Memory",
		      "dijit/form/Button",
		      "dijit/form/DropDownButton",
		      "dijit/DropDownMenu",
		      "dijit/MenuItem",
              "dojo/ready",
               "dijit/form/FilteringSelect"
        ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready, filteringSelect) {
            try {
                montarTipoFeriado(ready, Memory, filteringSelect);
                var myStore = Cache(
		        JsonRest({
		            target: Endereco() + "/api/coordenacao/getFeriadoSearch?desc=&inicio=false&status=0&Ano=0&Mes=0&Dia=0&AnoFim=0&MesFim=0&DiaFim=0&somenteAno=0" + "&idFeriadoAtivo=" + document.getElementById("feriadoAtivoPesquisa").checked,

		            handleAs: "json",
		            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		            idProperty: "cod_Feriado"
		        }), Memory({ idProperty: "cod_feriado" }));

                var gridFeriado = new EnhancedGrid({
                    //store: ObjectStore({ objectStore: myStore }),
                    store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    structure: [
			        { name: "<input id='selecionaTodosFeriados' style='display:none'/>", field: "selecionadoFeriado", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxFeriado },
			        //{ name: "Código", field: "cod_feriado", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
			        { name: "Feriado", field: "dc_feriado", width: "22%" },
			        { name: "Dia Início", field: "dd_feriado", width: "8%", styles: "text-align: right; min-width:25px; max-width:25px;" },
			        { name: "Mês Início", field: "mm_feriado", width: "8%", styles: "text-align: right; min-width:25px; max-width:25px;" },
			        { name: "Ano Início", field: "aa_feriado", width: "8%", styles: "text-align: right; min-width:25px; max-width:25px;" },
                    { name: "Dia Final", field: "dd_feriado_fim", width: "7%", styles: "text-align: right; min-width:25px; max-width:25px;" },
			        { name: "Mês Final", field: "mm_feriado_fim", width: "7%", styles: "text-align: right; min-width:25px; max-width:25px;" },
			        { name: "Ano Final", field: "aa_feriado_fim", width: "7%", styles: "text-align: right; min-width:25px; max-width:25px;" },
			        { name: "Feriado Escola", field: "TipoFeriado", width: "12%", styles: "text-align: center;" },
			        { name: "Financeiro", field: "isFeriadoFinanceiro", width: "8%", styles: "text-align: center;" },
                    { name: "Ativo", field: "id_feriado_ativo", width: "8%", styles: "text-align: center;", formatter: statusAtivoFormatado }
                    ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc,
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
                }, "gridFeriado");
                // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                gridFeriado.pagination.plugin._paginator.plugin.connect(gridFeriado.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridFeriado, 'cod_feriado', 'selecionaTodosFeriados');
                });
                gridFeriado.startup();
                gridFeriado.canSort = function (col) { return Math.abs(col) != 1; };

                gridFeriado.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
				            item = this.getItem(idx),
				            store = this.store;
                        // Limpar os dados do form
                        getLimpar('#formFeriado');
                        clearForm("formFeriado");
                        apresentaMensagem('apresentadorMensagem', '');
                        keepValues(FORM_FERIADO, item, gridFeriado, false);
                        dijit.byId("cadFeriado").show();
                        IncluirAlterar(0, 'divAlterarFeriado', 'divIncluirFeriado', 'divExcluirFeriado', 'apresentadorMensagem', 'divCancelarFeriado', 'divClearFeriado');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridFeriado, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosFeriados').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cod_feriado', 'selecionadoFeriado', -1, 'selecionaTodosFeriados', 'selecionaTodosFeriados', 'gridFeriado')", gridFeriado.rowsPerPage * 3);
                    });
                });

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        eventoEditarFeriado(gridFeriado.itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemover(gridFeriado.itensSelecionados, 'deletarFeriado(itensSelecionados)'); }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasFeriado",
                    dropDown: menu,
                    id: "acoesRelacionadasFeriado"
                });
                dom.byId("linkAcoesFeriado").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () { buscarTodosItens(gridFeriado, 'todosItensFeriado', ['searchFeriado', 'relatorioFeriado']); searchFeriado(false); }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridFeriado', 'selecionadoFeriado', 'cod_feriado', 'selecionaTodosFeriados', ['searchFeriado', 'relatorioFeriado'], 'todosItensFeriado'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensFeriado",
                    dropDown: menu,
                    id: "todosItensFeriado"
                });
                dom.byId("linkSelecionadosFeriados").appendChild(button.domNode);

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () { incluirFeriado(); }
                }, "incluirFeriado");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () { alterarFeriado(); }
                }, "alterarFeriado");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarFeriado() }); }
                }, "deletarFeriado");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset", onClick: function () { clearForm('formFeriado') }
                }, "limparFeriado");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () { keepValues(FORM_FERIADO, null, gridFeriado, null); }
                }, "cancelarFeriado");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cadFeriado").hide();
                    }
                }, "fecharFeriado");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        searchFeriado(true);
                    }
                }, "searchFeriado");
                decreaseBtn(document.getElementById("searchFeriado"), '32px');

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                    onClick: function () {
                        dijit.byId("cadFeriado").show();
                        getLimpar('#formFeriado');
                        clearForm("formFeriado");
                        IncluirAlterar(1, 'divAlterarFeriado', 'divIncluirFeriado', 'divExcluirFeriado', 'apresentadorMensagem', 'divCancelarFeriado', 'divClearFeriado');
                        dijit.byId('id_novo_feriado_ativo').set('checked', true);
                    }
                }, "novoFeriado");
                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        require(["dojo/_base/xhr"], function (xhr) {
                            try {
                                var Ano = dijit.byId("aaferiado").valueNode.value == '' ? 0 : dijit.byId("aaferiado").value;
                                var Mes = dijit.byId("mmferiado").valueNode.value == '' ? 0 : dijit.byId("mmferiado").value;
                                var Dia = dijit.byId("ddferiado").valueNode.value == '' ? 0 : dijit.byId("ddferiado").value;

                                var AnoFim = dijit.byId("aaferiadoFim").valueNode.value == '' ? 0 : dijit.byId("aaferiadoFim").value;
                                var MesFim = dijit.byId("mmferiadoFim").valueNode.value == '' ? 0 : dijit.byId("mmferiadoFim").value;
                                var DiaFim = dijit.byId("ddferiadoFim").valueNode.value == '' ? 0 : dijit.byId("ddferiadoFim").value;
                                xhr.get({
                                    url: Endereco() + "/api/coordenacao/geturlrelatorioferiado?" + getStrGridParameters('gridFeriado') + "desc=" +
                                        encodeURIComponent(document.getElementById("feriadoDesc").value) + "&inicio=" +
                                        document.getElementById("inicioFeriado").checked + "&status=" + retornaStatus("statusFeriado") + "&Ano=" +
                                        Ano + "&Mes=" + Mes + "&Dia=" + Dia + "&AnoFim=" + AnoFim + "&MesFim=" + MesFim + "&DiaFim=" +
                                        DiaFim + "&somenteAno=" + retornaStatus("somenteAnoPes") + 
                                        "&idFeriadoAtivo=" + document.getElementById("feriadoAtivoPesquisa").checked,
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (data) {
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                                },
					            function (error) {
					                apresentaMensagem('apresentadorMensagem', error);
					            });
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });
                    }
                }, "relatorioFeriado");
                adicionarAtalhoPesquisa(['feriadoDesc', 'statusFeriado', 'somenteAnoPes', 'aaferiado', 'mmferiado', 'ddferiado', 'aaferiadoFim', 'mmferiadoFim', 'ddferiadoFim'], 'searchFeriado', ready);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        dijit.byId("dd_feriado").on("change", function (e) {
            try {
                var diaFim = hasValue(dijit.byId("diaFim").value) ? parseInt(dojo.byId("diaFim").value) : 0;
                var mesFim = hasValue(dijit.byId("mesFim").value) ? parseInt(dojo.byId("mesFim").value) : 0;
                var anoFim = hasValue(dojo.byId("anoFim").value) ? parseInt(dojo.byId("anoFim").value) : 0;
                if (diaFim == 1 && mesFim == 1 && anoFim == 0)
                    dojo.byId("diaFim").value = e;
            }
            catch (e) {
                postGerarLog(e);
            }
        });

        dijit.byId("mm_feriado").on("change", function (e) {
            try {
                var mesFim = hasValue(dijit.byId("mesFim").value) ? parseInt(dojo.byId("mesFim").value) : 0;
                var anoFim = hasValue(dojo.byId("anoFim").value) ? parseInt(dojo.byId("anoFim").value) : 0;
                if (mesFim == 1 && anoFim == 0)
                    dojo.byId("mesFim").value = e;
            }
            catch (e) {
                postGerarLog(e);
            }
        });

        dijit.byId("aa_feriado").on("change", function (e) {
            try {
                var anoFim = hasValue(dojo.byId("anoFim").value) ? parseInt(dojo.byId("anoFim").value) : 0;
                if (anoFim == 0 && !isNaN(e))
                    dojo.byId("anoFim").value = e;
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region montarCadastroEstagio
function montarCadastroEstagio() {
    // ** Inicio da criação da grade de Estagio**\\
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
		"dijit/MenuItem",
        "dijit/ColorPalette"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, ColorPalette) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getEstagioSearch?desc=&abrev=&inicio=false&status=1&CodP=0",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_estagio"
		    }), Memory({ idProperty: "cd_estagio" }));

            var gridEstagio = new EnhancedGrid({
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
			    { name: "<input id='selecionaTodosEstagios' style='display:none'/>", field: "selecionadoEstagio", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxEstagio },
			    //{ name: "Código", field: "cd_estagio", width: "50px", styles: "text-align: right; min-width:50px; max-width:75px;" },
			    { name: "Produto", field: "no_produto", width: "20%" },
			    { name: "Estágio", field: "no_estagio", width: "50%" },
                { name: "Abreviado", field: "no_estagio_abreviado", width: "10%" },
			    { name: "Ordem", field: "ordem", width: "10%", styles: "text-align: right;" },
			    { name: "Ativo", field: "estagio_ativo", width: "10%", styles: "text-align: center; min-width:50px; max-width:75px;" }],
                canSort: true,
                noDataMessage: msgNotRegEnc,
                keepSelection: true,
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
            }, "gridEstagio");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridEstagio.pagination.plugin._paginator.plugin.connect(gridEstagio.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridEstagio, 'cd_estagio', 'selecionaTodosEstagios'); });
            gridEstagio.canSort = function (col) { return Math.abs(col) != 1; };
            gridEstagio.startup();
            gridEstagio.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    dijit.byId("incluirEstagio").set("disabled", false);
                    var idx = evt.rowIndex,
				        item = this.getItem(idx),
				        store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formEstagio');
                    clearForm("formEstagio");
                    //document.getElementById('divClearEstagio').style.display = "none";
                    apresentaMensagem('apresentadorMensagem', '');
                    xhr.get({
                        url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_CAD_ESTAGIO + "&cd_produto=null",
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }).then(function (dataProdAtivo) {
                        loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, 'cd_produtoCE');
                        dijit.byId("cd_produtoCE").set("value", item.cd_produto);
                    });
                    //            var storeProd = dijit.byId("codProduto").store;
                    //            dijit.byId("cd_produtoC").store = storeProd;
                    keepValues(FORM_ESTAGIO, item, gridEstagio, false);
                    dijit.byId("cadEstagio").show();
                    dijit.byId("gridEstagioOrdem").itensSelecionados = [];
                    //IncluirAlterar(0, 'divAlterarEstagio', 'divIncluirEstagio', 'divExcluirEstagio', 'apresentadorMensagem', 'divCancelarEstagio', 'divClearEstagio');

                    //document.getElementById('ordem').style.display = "none";
                    //document.getElementById('num_ordem_estagio').style.display = "none";
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridEstagio, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosEstagios').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_estagio', 'selecionadoEstagio', -1, 'selecionaTodosEstagios', 'selecionaTodosEstagios', 'gridEstagio')", gridEstagio.rowsPerPage * 3);
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
                    eventoEditarEstagio(gridEstagio.itensSelecionados);
                    //document.getElementById('ordem').style.display = "none";
                    //document.getElementById('num_ordem_estagio').style.display = "none";
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
                    eventoRemover(gridEstagio.itensSelecionados, 'deletarEstagio(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadas",
                dropDown: menu,
                id: "acoesRelacionadasEstagio"
            });
            dom.byId("linkAcoesEstagio").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridEstagio, 'todosItensEstagio', ['searchEstagio', 'relatorioEstagio']);
                    searchEstagio(false);
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridEstagio', 'selecionadoEstagio', 'cd_estagio', 'selecionaTodosEstagio', ['searchEstagio', 'relatorioEstagio'], 'todosItensEstagio'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItens",
                dropDown: menu,
                id: "todosItensEstagio"
            });
            dom.byId("linkSelecionadosEstagio").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Incluir",
                iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                onClick: function () {
                    try {
                        alterarOrd = 0;
                        dijit.byId("dialogEst").show();
                        document.getElementById("cd_estagio").value = '';
                        document.getElementById("no_estagio").value = '';
                        document.getElementById("no_estagio_abreviado").value = '';
                        dojo.byId("incluirEst_label").innerHTML = "Incluir";
                        document.getElementById('divCancelarEst').style.display = "none";
                        document.getElementById('divLimparEst').style.display = "";
                        dijit.byId("id_estagio_ativo").set("value", true);
                        if (dijit.byId("gridEstagioOrdem")._by_idx.length > 0)
                            document.getElementById("num_ordem_estagio").value = dijit.byId("gridEstagioOrdem")._by_idx.length + 1;
                        else
                            document.getElementById("num_ordem_estagio").value = 1;
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "incluirEstagio");
            new Button({
                label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () {
                    if (alterarOrd == 1 || dojo.byId("cd_estagio").value > 0)
                        alterarEstagioGrid(dijit.byId("gridEstagioOrdem"));
                    else
                        incluiEstagioGrid();
                    //document.getElementById('divCancelarEst').style.display = "none";
                    //document.getElementById('divLimparEst').style.display = "";
                }
            }, "incluirEst");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset",
                onClick: function () {
                    try {
                        document.getElementById("cd_estagio").value = '';
                        document.getElementById("no_estagio").value = '';
                        document.getElementById("no_estagio_abreviado").value = '';
                        dijit.byId("id_estagio_ativo").set("value", true);
                        if (dijit.byId("gridEstagioOrdem")._by_idx.length > 0)
                            document.getElementById("num_ordem_estagio").value = dijit.byId("gridEstagioOrdem")._by_idx[0].item.nm_ordem_estagio + 1;
                        else
                            document.getElementById("num_ordem_estagio").value = 1;
                        dijit.byId("nm_paleta_cor").reset();
                        dojo.byId("widget_cor_legenda_estagio").style.backgroundColor = "";
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }

            }, "limparEst");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    keepValuesEstagio(FORM_ESTAGIO, null, dijit.byId("gridEstagioOrdem"), null);
                }
            }, "cancelarEst");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("dialogEst").hide();
                }
            }, "fecharEst");

            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                onClick: function () {
                    try {
                        dijit.byId("cd_produtoCE").reset();
                        loadGridEstagioOrdem(0);
                        getLimpar('#formEstagio');
                        clearForm("formEstagio");
                        document.getElementById('num_ordem_estagio').value = "";
                        dijit.byId("incluirEstagio").set("disabled", false);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "limparEstagio");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    keepValues(FORM_ESTAGIO, null, gridEstagio, null);
                    loadGridEstagioOrdem(dijit.byId("cd_produtoCE").value);
                    dijit.byId("gridEstagioOrdem").itensSelecionados = [];
                }
            }, "cancelarEstagio");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadEstagio").hide();
                }
            }, "fecharEstagio");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchEstagio(true);
                }
            }, "searchEstagio");
            decreaseBtn(document.getElementById("searchEstagio"), '32px');
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
                        dijit.byId("incluirEstagio").set("disabled", false);
                        dijit.byId("cd_produtoCE").reset();
                        document.getElementById('num_ordem_estagio').value = "";
                        //dijit.byId("cd_estagio").reset();
                        document.getElementById("cd_estagio").value = "";
                        loadGridEstagioOrdem(0);
                        apresentaMensagem('apresentadorMensagemEstagio', null);
                        dijit.byId("cadEstagio").show();
                        getLimpar('#formEstagio');
                        clearForm("formEstagio");
                        xhr.get({
                            url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_CAD_ESTAGIO + "&cd_produto=null",
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }).then(function (dataProdAtivo) {
                            loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, 'cd_produtoCE');
                            dijit.byId("cd_produtoCE").set("value", 0);
                        });
                        dijit.byId("gridEstagioOrdem").itensSelecionados = [];

                        dijit.byId("incluirEstagio").set("disabled", true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoEstagio");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                        xhr.get({
                            url: !hasValue(document.getElementById("estagioDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatorioestagio?" + getStrGridParameters('gridEstagio') + "desc=&abrev=" + encodeURIComponent(document.getElementById("estagioAbrev").value) + "&inicio=" + document.getElementById("inicioEstagio").checked + "&status=" + retornaStatus("statusEstagio") + "&CodP=" + dijit.byId("codProdutoE").get("value") : Endereco() + "/api/coordenacao/geturlrelatorioestagio?" + getStrGridParameters('gridEstagio') + "desc=" + encodeURIComponent(document.getElementById("estagioDesc").value) + "&abrev=" + encodeURIComponent(document.getElementById("estagioAbrev").value) + "&inicio=" + document.getElementById("inicioEstagio").checked + "&status=" + retornaStatus("statusEstagio") + "&CodP=" + dijit.byId("codProdutoE").get("value"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioEstagio");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { salvarEstagio(); }
            }, "gravarEstagio");

            // Adiciona link de ações:
            var menuEst = new DropDownMenu({ style: "height: 25px" });
            var acaoExcluir = new MenuItem({
                label: "Excluir",
                onClick: function () { deletarOrdenacao(); }
            });
            menuEst.addChild(acaoExcluir);
            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasEst",
                dropDown: menuEst,
                id: "acoesRelacionadasEst"
            });
            dom.byId("linkAcoesEst").appendChild(button.domNode);
            loadFiltroProduto(xhr);
            montarEstagioOrdem();
            dijit.byId("cd_produtoCE").on("change", function (e) {
                try {
                    apresentaMensagem('apresentadorMensagemEstagio', null);
                    if (eval(e) >= 0) {
                        e == null || e == "" ? e = 0 : e;
                        //destroyGridEstagioOrdem();
                        loadGridEstagioOrdem(e);
                        dijit.byId("gridEstagioOrdem").itensSelecionados = [];
                        dijit.byId("incluirEstagio").set("disabled", false);
                    }
                    else
                        dijit.byId("incluirEstagio").set("disabled", true);
                }
                catch (e) {
                    postGerarLog(e);
                }
            })
            dijit.byId("id_estagio_ativo").on("click", function (e) {
                if (this.checked == true)
                    document.getElementById('num_ordem_estagio').value = hasValue(dijit.byId("gridEstagioOrdem")._by_idx[0]) ? dijit.byId("gridEstagioOrdem")._by_idx[0].item.nm_ordem_estagio + 1 : 1;
                else
                    document.getElementById('num_ordem_estagio').value = 0;
            });
            var myPalette = new ColorPalette({
                palette: "7x10",
                onChange: function (val) {
                    dijit.byId("nm_paleta_cor").set("value", val);
                }
            }, "paletaCor").startup();
            dijit.byId("nm_paleta_cor").on("change", function (val) {
                if (hasValue(val)) {
                    dijit.byId("nm_paleta_cor").set("value", val);
                    dojo.byId("widget_cor_legenda_estagio").style.backgroundColor = val;
                }
                else
                    dojo.byId("widget_cor_legenda_estagio").style.backgroundColor = "";
            });
            adicionarAtalhoPesquisa(['estagioDesc', 'estagioAbrev', 'statusEstagio', 'codProdutoE'], 'searchEstagio', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region montarCadastroParticipacao
function montarCadastroParticipacao() {
    // ** Inicio da criação da grade de Participação**\\
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
		"dijit/MenuItem",
        "dijit/ColorPalette"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, ColorPalette) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getParticipacaoSearch?desc=&inicio=false&status=1",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_participacao"
		    }), Memory({ idProperty: "cd_participacao" }));

            var gridParticipacao = new EnhancedGrid({
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
			    { name: "<input id='selecionaTodosParticipacao' style='display:none'/>", field: "selecionadoParticipacao", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxParticipacao },
			    { name: "Descrição", field: "no_participacao", width: "80%" },
			    { name: "Ativa", field: "participacao_ativa", width: "20%", styles: "text-align: center; min-width:50px; max-width:75px;" }],
                canSort: true,
                noDataMessage: msgNotRegEnc,
                keepSelection: true,
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
            }, "gridParticipacao");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridParticipacao.pagination.plugin._paginator.plugin.connect(gridParticipacao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridParticipacao, 'cd_participacao', 'selecionaTodosgridParticipacao'); });
            gridParticipacao.canSort = function (col) { return Math.abs(col) != 1; };
            gridParticipacao.startup();
            gridParticipacao.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    dijit.byId("incluirParticipacao").set("disabled", false);
                    var idx = evt.rowIndex,
				        item = this.getItem(idx),
				        store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formParticipacao');
                    clearForm("formParticipacao");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_PARTICIPACAO, item, gridParticipacao, false);
                    dijit.byId("cadParticipacao").show();
                    IncluirAlterar(0, 'divAlterarParticipacao', 'divIncluirParticipacao', 'divExcluirParticipacao', 'apresentadorMensagem', 'divCancelarParticipacao', 'divClearParticipacao');
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridParticipacao, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosParticipacao').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_participacao', 'selecionadoParticipacao', -1, 'selecionaTodosParticipacao', 'selecionaTodosParticipacao', 'gridParticipacao')", gridParticipacao.rowsPerPage * 3);
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
                    eventoEditarParticipacao(gridParticipacao.itensSelecionados);
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
                    eventoRemover(gridParticipacao.itensSelecionados, 'deletarParticipacao(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadas",
                dropDown: menu,
                id: "acoesRelacionadasParticipacao"
            });
            dom.byId("linkAcoesParticipacao").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridParticipacao, 'todosItensParticipacao', ['pesquisarParticipacao', 'relatorioParticipacao']);
                    searchParticipacao(false);
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridParticipacao', 'selecionadoParticipacao', 'cd_participacao', 'selecionaTodosParticipacao', ['pesquisarParticipacao', 'relatorioParticipacao'], 'todosItensParticipacao'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItens",
                dropDown: menu,
                id: "todosItensParticipacao"
            });
            dom.byId("linkSelecionadosParticipacao").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () { incluirParticipacao(); }
            }, "incluirParticipacao");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    alterarParticipacao(false)
                }
            }, "alterarParticipacao");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarParticipacao(); });
                }
            }, "deletarParticipacao");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formParticipacao') }
            }, "limparParticipacao");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () { keepValues(FORM_PARTICIPACAO, null, gridParticipacao, null); }
            }, "cancelarParticipacao");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadParticipacao").hide();
                }
            }, "fecharParticipacao");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchParticipacao(true);
                }
            }, "pesquisarParticipacao");
            decreaseBtn(document.getElementById("pesquisarParticipacao"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadParticipacao").show();
                        getLimpar('#formParticipacao');
                        clearForm("formParticipacao");
                        IncluirAlterar(1, 'divAlterarParticipacao', 'divIncluirParticipacao', 'divExcluirParticipacao', 'apresentadorMensagem', 'divCancelarParticipacao', 'divClearParticipacao');
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                }
            }, "novaParticipacao");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: Endereco() + "/api/coordenacao/geturlrelatorioparticipacao?" + getStrGridParameters('gridParticipacao') + "desc=" + encodeURIComponent(document.getElementById("no_participacao").value) + "&inicio=" + document.getElementById("cb_inicio").checked + "&status=" + retornaStatus("statusParticipacao"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioParticipacao");
            adicionarAtalhoPesquisa(['no_participacao', 'statusParticipacao'], 'pesquisarParticipacao', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region montarCadastroCargaProfessor
function montarCadastroCargaProfessor() {
    // ** Inicio da criação da grade de Carga Professor**\\
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
		"dijit/MenuItem",
        "dijit/ColorPalette"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, ColorPalette) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getCargaProfessorSearch?qtd_minutos_duracao=0",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_carga_professor"
		    }), Memory({ idProperty: "cd_carga_professor" }));

            var gridCargaProfessor = new EnhancedGrid({
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
			    { name: "<input id='selecionaTodosCargaProfessor' style='display:none'/>", field: "selecionadoCargaProfessor", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCargaProfessor },
                { name: "Carga Horária(min)", field: "nm_carga_horaria", width: "50%", styles: "min-width:80px;text-align:center;" },
                { name: "Carga Professor(min)", field: "nm_carga_professor", width: "45%", styles: "min-width:80px;text-align:center;" }],
                canSort: true,
                noDataMessage: msgNotRegEnc,
                keepSelection: true,
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
            }, "gridCargaProfessor");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridCargaProfessor.pagination.plugin._paginator.plugin.connect(gridCargaProfessor.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridCargaProfessor, 'cd_carga_professor', 'selecionaTodosgridCargaProfessor'); });
            gridCargaProfessor.canSort = function (col) { return Math.abs(col) != 1; };
            gridCargaProfessor.startup();
            gridCargaProfessor.on("RowDblClick", function (evt) {
                try {
                    dijit.byId("incluirCargaProfessor").set("disabled", false);
                    var idx = evt.rowIndex,
				        item = this.getItem(idx),
				        store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formCargaProfessor');
                    clearForm("formCargaProfessor");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_CARGAPROFESSOR, item, gridCargaProfessor, false);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridCargaProfessor, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosCargaProfessor').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_carga_professor', 'selecionadoCargaProfessor', -1, 'selecionaTodosCargaProfessor', 'selecionaTodosCargaProfessor', 'gridCargaProfessor')", gridCargaProfessor.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    eventoEditarCargaProfessor(gridCargaProfessor.itensSelecionados);
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    eventoRemover(gridCargaProfessor.itensSelecionados, 'deletarCargaProfessor(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadas",
                dropDown: menu,
                id: "acoesRelacionadasCargaProfessor"
            });
            dom.byId("linkAcoesCargaProfessor").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridCargaProfessor, 'todosItensCargaProfessor', ['pesquisarCargaProfessor', 'relatorioCargaProfessor']);
                    searchCargaProfessor(false);
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridCargaProfessor', 'selecionadoCargaProfessor', 'cd_carga_professor', 'selecionaTodosCargaProfessor', ['pesquisarCargaProfessor', 'relatorioCargaProfessor'], 'todosItensCargaProfessor'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItens",
                dropDown: menu,
                id: "todosItensCargaProfessor"
            });
            dom.byId("linkSelecionadosCargaProfessor").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    incluirCargaProfessor();
                }
            }, "incluirCargaProfessor");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    alterarCargaProfessor(false);
                }
            }, "alterarCargaProfessor");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarCargaProfessor(); });
                }
            }, "deletarCargaProfessor");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formCargaProfessor') }
            }, "limparCargaProfessor");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    getLimpar('#formCargaProfessor');
                    clearForm("formCargaProfessor");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_CARGAPROFESSOR, null, gridCargaProfessor, null);
                }
            }, "cancelarCargaProfessor");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadCargaProfessor").hide();
                }
            }, "fecharCargaProfessor");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchCargaProfessor(true);
                }
            }, "pesquisarCargaProfessor");
            decreaseBtn(document.getElementById("pesquisarCargaProfessor"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        dijit.byId("cadCargaProfessor").show();
                        getLimpar('#formCargaProfessor');
                        clearForm("formCargaProfessor");
                       
                        IncluirAlterar(1, 'divAlterarCargaProfessor', 'divIncluirCargaProfessor', 'divExcluirCargaProfessor', 'apresentadorMensagem', 'divCancelarCargaProfessor', 'divClearCargaProfessor');
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                }
            }, "novaCargaProfessor");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    var qtdMinutosDuracao = hasValue(dijit.byId("nmCargaHorariaMin").value) ? dijit.byId("nmCargaHorariaMin").value : 0;
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: Endereco() + "/api/coordenacao/geturlrelatoriocargaprofessor?" + getStrGridParameters('gridCargaProfessor') + "&qtd_minutos_duracao=" + parseInt(qtdMinutosDuracao),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioCargaProfessor");
            adicionarAtalhoPesquisa(['nmCargaHorariaMin'], 'pesquisarCargaProfessor', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion


//#region montarCadastroNivel
function montarCadastroNivel() {
    // ** Inicio da criação da grade de Nivel**\\
    require([
		   "dojo/dom",
		   "dojox/grid/EnhancedGrid",
		   "dojox/grid/enhanced/plugins/Pagination",
		   "dojo/store/JsonRest",
		   "dojo/data/ObjectStore",
		   "dojo/store/Cache",
		   "dojo/store/Memory",
		   "dijit/form/Button",
		   "dijit/form/DropDownButton",
		   "dijit/DropDownMenu",
		   "dijit/MenuItem",
           "dojo/ready"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getNivelSearch?desc=&inicio=false&status=1",
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_nivel"
		    }), Memory({ idProperty: "cd_nivel" }));

            var gridNivel = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosNiveis' style='display:none'/>", field: "selecionadoNivel", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxNivel },
                    { name: "Descrição", field: "dc_nivel", width: "70%", styles: "min-width:80px;" },
                    { name: "Ordem", field: "nm_ordem", width: "10%", styles: "min-width:80px;text-align:center;" },
                    { name: "Ativo", field: "nivelAtivo", width: "10%", styles: "min-width:80px;text-align:center;" }
                ],
                canSort: true,          
                noDataMessage: msgNotRegEnc,
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
                        position: "button"
                    }
                }
            }, "gridNivel");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridNivel.pagination.plugin._paginator.plugin.connect(gridNivel.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridNivel, 'cd_nivel', 'selecionaTodosNiveis');
            });
            gridNivel.startup();
            gridNivel.canSort = function (col) { return Math.abs(col) != 1; };
            
            gridNivel.on("RowDblClick", function (evt) {
                try {
                   
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
				        item = this.getItem(idx),
				        store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formNivel');
                    clearForm("formNivel");
                    apresentaMensagem('apresentadorMensagem', '');
                    dijit.byId("cadNivel").show();
                    IncluirAlterar(0, 'divAlterarNivel', 'divIncluirNivel', 'divExcluirNivel', 'apresentadorMensagemNivel', 'divCancelarNivel', 'divClearNivel');
                    keepValues(FORM_NIVEL, item, gridNivel, false);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);
            // Corrige o tamanho do pane que o dojo cria para o dialog com scroll no ie7:
            if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
                var ieversion = new Number(RegExp.$1)
                if (ieversion == 7)
                    // Para IE7
                    dojo.byId('cadNivel').childNodes[1].style.height = '100%';
            }

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridNivel, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosNiveis').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_nivel', 'selecionadoNivel', -1, 'selecionaTodosNiveis', 'selecionaTodosNiveis', 'gridNivel')", gridNivel.rowsPerPage * 3);
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

                    eventoEditarNivel(gridNivel.itensSelecionados);
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
                    eventoRemover(gridNivel.itensSelecionados, 'deletarNivel(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadas",
                dropDown: menu,
                id: "acoesRelacionadasNivel"
            });
            dom.byId("linkAcoesNivel").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridNivel, 'todosItensNivel', ['pesquisarNivel', 'relatorioNivel']);
                    searchNivel(false);
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridNivel', 'selecionadoNivel', 'cd_nivel', 'selecionaTodosNiveis', ['pesquisarNivel', 'relatorioNivel'], 'todosItensNivel'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItens",
                dropDown: menu,
                id: "todosItensNivel"
            });
            dom.byId("linkSelecionadosNiveis").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    incluirNivel();
                }
            }, "incluirNivel");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    alterarNivel(false);
                }
            }, "alterarNivel");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarNivel(); });
                }
            }, "deletarNivel");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formNivel') }
            }, "limparNivel");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    getLimpar('#formNivel');
                    clearForm("formNivel");
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(FORM_NIVEL, null, gridNivel, null);
                }
            }, "cancelarNivel");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadNivel").hide();
                }
            }, "fecharNivel");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchNivel(true);
                }
            }, "pesquisarNivel");
            decreaseBtn(document.getElementById("pesquisarNivel"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }

                        dijit.byId("cadNivel").show();
                        getLimpar('#formNivel');
                        clearForm("formNivel");

                        IncluirAlterar(1, 'divAlterarNivel', 'divIncluirNivel', 'divExcluirNivel', 'apresentadorMensagemNivel', 'divCancelarNivel', 'divClearNivel');
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                }
            }, "novaNivel");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {                    
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("nivelDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatorionivel?" + getStrGridParameters('gridNivel') + "desc=&inicio=" + document.getElementById("cb_inicio_nivel").checked + "&status=" + retornaStatus("statusNivel") : Endereco() + "/api/coordenacao/geturlrelatorioNivel?" + getStrGridParameters('gridNivel') + "desc=" + encodeURIComponent(document.getElementById("nivelDesc").value) + "&inicio=" + document.getElementById("cb_inicio_nivel").checked + "&status=" + retornaStatus("statusNivel"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
					    function (error) {
					        apresentaMensagem('apresentadorMensagem', error);
					    });
                    })
                }
            }, "relatorioNivel");
            adicionarAtalhoPesquisa(['nivelDesc', 'statusNivel'], 'pesquisarNivel', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion


//#region montarCadastroMensagemAvaliacao
function montarCadastroMensagemAvaliacao() {
    // ** Inicio da criação da grade de mensagemAvaliacao**\\
    require([
        "dojo/_base/xhr",
        "dojo/data/ItemFileReadStore",
        "dojox/json/ref",
        "dojo/dom",
        "dojo/dom-attr",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/ready"
    ], function (xhr, ItemFileReadStore, ref, dom, domAttr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try {

	        dijit.byId("cbCursos").dropDownMenu.domNode.style.maxHeight = "300px";
	          
            setMenssageMultiSelect(EnumTipoMensagemMultiSelect.CURSO, 'cbCursos');
	         dijit.byId("cbCursos").on("change", function (e) {
                 setMenssageMultiSelect(EnumTipoMensagemMultiSelect.CURSO, 'cbCursos', true, 20);
	         });
            dijit.byId("cbCursos").set("disabled", true);	
	        var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/coordenacao/getMensagemAvaliacaoSearch?desc=&inicio=false&status=1&produto=0&curso=0",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_mensagem_avaliacao"
                }), Memory({ idProperty: "cd_mensagem_avaliacao" }));

            var gridMensagemAvaliacao = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosMensagemAvaliacao' style='display:none'/>", field: "selecionadoMensagemAvaliacao", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMensagemAvaliacao },
                    { name: "Produto", field: "no_produto", width: "20%", styles: "min-width:80px;" },
                    { name: "Curso", field: "no_curso", width: "20%", styles: "min-width:80px;text-align:left;" },
                    { name: "Mensagem", field: "tx_mensagem_avaliacao", width: "70%", styles: "min-width:80px;text-align:left;" },
                    { name: "Ativo", field: "mensagemAtiva", width: "10%", styles: "min-width:80px;text-align:center;" }
                ],
                canSort: true,
                noDataMessage: msgNotRegEnc,
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
                        position: "button"
                    }
                }
            }, "gridMensagemAvaliacao");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridMensagemAvaliacao.pagination.plugin._paginator.plugin.connect(gridMensagemAvaliacao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridMensagemAvaliacao, 'cd_mensagem_avaliacao', 'selecionaTodosMensagemAvaliacao');
            });
            gridMensagemAvaliacao.startup();
            gridMensagemAvaliacao.canSort = function (col) { return Math.abs(col) != 1; };

            gridMensagemAvaliacao.on("RowDblClick", function (evt) {
                try {

                    //if (!eval(MasterGeral())) {
                    //    var mensagensWeb = new Array();
                    //    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                    //    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    //    return;
                    //}
                    var idx = evt.rowIndex,
                        item = this.getItem(idx),
                        store = this.store;
                    // Limpar os dados do form
                    getLimpar('#formMensagemAvaliacaoCad');
                    clearForm("formMensagemAvaliacaoCad");
                    apresentaMensagem('apresentadorMensagem', '');
                    dijit.byId("cadMensagemAvaliacao").show();
                    var mensagemAvaliacaoPesq = new MensagemAvaliacao(item);

                    xhr.post({
                        url: Endereco() + "/api/coordenacao/obterRecursosMensagemAvaliacao",
	                    headers: {
		                    "Accept": "application/json",
		                    "Content-Type": "application/json",
		                    "Authorization": Token()
	                    },
	                    handleAs: "json",
                        postData: ref.toJson(mensagemAvaliacaoPesq)
                    }).then(function (dataMensagemAvaliacao) {
                            loadProduto(jQuery.parseJSON(dataMensagemAvaliacao).retorno.listaProdutos, 'pesCadProduto');
                            loadMultiCursoMensagemAvaliacao(jQuery.parseJSON(dataMensagemAvaliacao).retorno.listaCursos, "cbCursos");
                            //checkedValuesTrue(cursos, gridAtividadeExtra.itemSelecionado.cd_cursos, "cbCursos");
                            //dijit.byId("pesCadProduto").set("value", item.cd_curso);

                            IncluirAlterar(0, 'divAlterarMensagemAvaliacao', 'divIncluirMensagemAvaliacao', 'divExcluirMensagemAvaliacao', 'apresentadorMensagemMensagemAvaliacao', 'divCancelarMensagemAvaliacao', 'divClearMensagemAvaliacao');
                            keepValues(FORM_MENSAGEM_AVALIACAO, item, gridMensagemAvaliacao, false);
                    });

                    
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);
            // Corrige o tamanho do pane que o dojo cria para o dialog com scroll no ie7:
            if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
                var ieversion = new Number(RegExp.$1)
                if (ieversion == 7)
                    // Para IE7
                    dojo.byId('cadMensagemAvaliacao').childNodes[1].style.height = '100%';
            }

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridMensagemAvaliacao, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosMensagemAvaliacao').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_mensagem_avaliacao', 'selecionadoMensagemAvaliacao', -1, 'selecionaTodosMensagemAvaliacao', 'selecionaTodosMensagemAvaliacao', 'gridMensagemAvaliacao')", gridMensagemAvaliacao.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    //if (!eval(MasterGeral())) {
                    //    var mensagensWeb = new Array();
                    //    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                    //    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    //    return;
                    //}

                    eventoEditarMensagemAvaliacao(gridMensagemAvaliacao.itensSelecionados, xhr, ref);
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    //if (!eval(MasterGeral())) {
                    //    var mensagensWeb = new Array();
                    //    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                    //    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    //    return;
                    //}
                    eventoRemover(gridMensagemAvaliacao.itensSelecionados, 'deletarMensagemAvaliacao(itensSelecionados)');
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadas",
                dropDown: menu,
                id: "acoesRelacionadasMensagemAvaliacao"
            });
            dom.byId("linkAcoesMensagemAvaliacao").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridMensagemAvaliacao, 'todosItensMensagemAvaliacao', ['pesquisarMensagemAvaliacao', 'relatorioMensagemAvaliacao']);
                    searchMensagemAvaliacao(false);
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridMensagemAvaliacao', 'selecionadoMensagemAvaliacao', 'cd_mensagem_avaliacao', 'selecionaTodosMensagemAvaliacao', ['pesquisarMensagemAvaliacao', 'relatorioMensagemAvaliacao'], 'todosItensMensagemAvaliacao'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItens",
                dropDown: menu,
                id: "todosItensMensagemAvaliacao"
            });
            dom.byId("linkSelecionadosMensagemAvaliacao").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    incluirMensagemAvaliacao();
                }
            }, "incluirMensagemAvaliacao");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    alterarMensagemAvaliacao(false);
                }
            }, "alterarMensagemAvaliacao");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarMensagemAvaliacao(); });
                }
            }, "deletarMensagemAvaliacao");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset", onClick: function () { clearForm('formMensagemAvaliacaoCad');
                    dijit.byId("pesCadProduto").reset();
	                if (hasValue(dojo.byId('cbCursos').value, true))
		                dijit.byId("cbCursos").reset();
	                if (hasValue(dijit.byId('cbCursos')) && hasValue(dijit.byId('cbCursos').store))
		                dijit.byId('cbCursos').setStore(dijit.byId('cbCursos').store, [0]);
	                dijit.byId('cbCursos').reset();
                }
            }, "limparMensagemAvaliacao");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    

                    var mensagemAvaliacaoPesq = new MensagemAvaliacaoCancelar(dom, domAttr);
                    getLimpar('#formMensagemAvaliacaoCad');
                    clearForm("formMensagemAvaliacaoCad");
                    apresentaMensagem('apresentadorMensagem', '');
                    dijit.byId("pesCadProduto").set("disabled", false);
                    xhr.post({
	                    url: Endereco() + "/api/coordenacao/obterRecursosMensagemAvaliacao",
	                    headers: {
		                    "Accept": "application/json",
		                    "Content-Type": "application/json",
		                    "Authorization": Token()
	                    },
	                    handleAs: "json",
	                    postData: ref.toJson(mensagemAvaliacaoPesq)
                    }).then(function (dataMensagemAvaliacao) {
	                    

	                    loadProduto(jQuery.parseJSON(dataMensagemAvaliacao).retorno.listaProdutos, 'pesCadProduto');
	                    loadMultiCursoMensagemAvaliacao(jQuery.parseJSON(dataMensagemAvaliacao).retorno.listaCursos,
                            "cbCursos");
	                    
	                    keepValues(FORM_MENSAGEM_AVALIACAO, null, gridMensagemAvaliacao, null);
                    });
                }
            }, "cancelarMensagemAvaliacao");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadMensagemAvaliacao").hide();
                }
            }, "fecharMensagemAvaliacao");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    searchMensagemAvaliacao(true);
                }
            }, "pesquisarMensagemAvaliacao");
            decreaseBtn(document.getElementById("pesquisarMensagemAvaliacao"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',//dijitEditorIconNewSGF
                onClick: function () {
                    try {
                        //if (!eval(MasterGeral())) {
                        //    var mensagensWeb = new Array();
                        //    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                        //    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        //    return;
                        //}
                        dijit.byId("pesCadProduto").set("disabled", false);
                        xhr.get({
	                        url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + EnumTipoPesquisaProduto.HAS_ATIVO_CURSO + "&cd_produto=null",
	                        preventCache: true,
	                        handleAs: "json",
	                        headers: { "Accept": "application/json", "Authorization": Token() }
                        }).then(function (dataProd) {
                            if (hasValue(dataProd)) {
                                loadProduto(jQuery.parseJSON(dataProd).retorno, 'pesCadProduto');
                                dijit.byId("cadMensagemAvaliacao").show();
                                getLimpar('#formMensagemAvaliacaoCad');
                                clearForm("formMensagemAvaliacaoCad");

                                IncluirAlterar(1, 'divAlterarMensagemAvaliacao', 'divIncluirMensagemAvaliacao', 'divExcluirMensagemAvaliacao', 'apresentadorMensagemMensagemAvaliacao', 'divCancelarMensagemAvaliacao', 'divClearMensagemAvaliacao');
                            }
	                        
                        });


                        
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                }
            }, "novaMensagemAvaliacao");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/_base/xhr"], function (xhr) {
                        xhr.get({
                            url: !hasValue(document.getElementById("mensagemAvaliacaoDesc").value) ? Endereco() + "/api/coordenacao/geturlrelatoriomensagemAvaliacao?" + getStrGridParameters('gridMensagemAvaliacao') + "desc=&inicio=" + document.getElementById("cb_inicio_mensagem_avaliacao").checked + "&status=" + retornaStatus("statusMensagemAvaliacao") + "&produto=" + dijit.byId("cd_produtoM").get("value") + "&curso=" + dijit.byId("cbCurso").get("value") : Endereco() + "/api/coordenacao/geturlrelatorioMensagemAvaliacao?" + getStrGridParameters('gridMensagemAvaliacao') + "desc=" + encodeURIComponent(document.getElementById("mensagemAvaliacaoDesc").value) + "&inicio=" + document.getElementById("cb_inicio_mensagem_avaliacao").checked + "&status=" + retornaStatus("statusMensagemAvaliacao") + "&produto=" + dijit.byId("cd_produtoM").get("value") + "&curso=" + dijit.byId("cbCurso").get("value"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                    })
                }
            }, "relatorioMensagemAvaliacao");


            dijit.byId("pesCadProduto").on("change", function (e) {
	            if (hasValue(e)) {
		            loadCursoByProdutoMensagemAvaliacao(e);
	            } else {
                    dijit.byId("pesCadProduto").reset();
		            dijit.byId("cbCursos").reset();

		            if (hasValue(dojo.byId('cbCursos').value, true))
			            dijit.byId("cbCursos").reset();
		            if (hasValue(dijit.byId('cbCursos')) && hasValue(dijit.byId('cbCursos').store))
			            dijit.byId('cbCursos').setStore(dijit.byId('cbCursos').store, [0]);
		            dijit.byId('cbCursos').reset();
		            dijit.byId("cbCursos").set('disabled', true);
	            }

            });


            xhr.get({
                url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + EnumTipoPesquisaProduto.HAS_ATIVO_CURSO + "&cd_produto=null",
	            preventCache: true,
	            handleAs: "json",
	            headers: { "Accept": "application/json", "Authorization": Token() }
            }).then(function (dataProd) {
                loadProduto(jQuery.parseJSON(dataProd).retorno, 'cd_produtoM');
            });

            xhr.get({
                url: Endereco() + "/api/curso/getPesCursos?hasDependente=" + EnumTipoPesquisaCurso.HAS_ATIVO + "&cd_curso=&cd_produto=",
	            preventCache: true,
	            handleAs: "json",
	            headers: { "Accept": "application/json", "Authorization": Token() }
            }).then(function (dataCurs) {
                loadCurso(dataCurs.retorno, 'cbCurso');
            });

            

            adicionarAtalhoPesquisa(['mensagemAvaliacaoDesc', 'statusMensagemAvaliacao'], 'pesquisarMensagemAvaliacao', ready);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
function MensagemAvaliacao(item) {
    try {
        

        if (item != null) {

	        this.cd_mensagem_avaliacao = item.cd_mensagem_avaliacao;
	        this.tx_mensagem_avaliacao = item.tx_mensagem_avaliacao;
	        this.id_mensagem_ativa = item.id_mensagem_ativa;
            this.cd_produto = item.cd_produto;
            this.cd_curso = item.cd_curso;
        } else {
	        this.cd_mensagem_avaliacao = 0;
	        this.tx_mensagem_avaliacao = "";
	        this.id_mensagem_ativa = false;
	        this.cd_produto = 0;
	        this.cd_curso = 0;
        }

    }
	catch (e) {
		postGerarLog(e);
	}
}

function MensagemAvaliacaoCancelar(dom, domAttr) {
	try {

        this.cd_mensagem_avaliacao = dom.byId("cd_mensagem_avaliacao").value;
        this.tx_mensagem_avaliacao = dom.byId("txObs").value;
        this.id_mensagem_ativa = domAttr.get("id_mensagem_avaliacao_ativa_pers", "checked");
        this.cd_produto = dijit.byId("pesCadProduto").value;
        this.cd_curso = dijit.byId("cbCursos").value;
		

	}
	catch (e) {
		postGerarLog(e);
	}
}

//** Método de Incluir MensagemAvaliacao
function incluirMensagemAvaliacao() {
    try {
        apresentaMensagem('apresentadorMensagemMensagemAvaliacao', null);
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(dijit.byId("txObs").value) && !hasValue(dojo.byId("txObs").value)) {
	        var mensagensWeb = new Array();
	        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "È obrigatório preencher o campo mensagem para salvar o registro.");
            apresentaMensagem('apresentadorMensagemMensagemAvaliacao', mensagensWeb);
            return false;
        }
        if (!dijit.byId("formMensagemAvaliacaoCad").validate())
            return false;
        var cdMensagemAvaliacao = 0;
            require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                showCarregando();
                xhr.post(Endereco() + "/api/coordenacao/postIncluirMensagemAvaliacao", {
	                headers: {
		                "Accept": "application/json",
		                "Content-Type": "application/json",
		                "Authorization": Token()
	                },
	                handleAs: "json",
	                data: ref.toJson({
		                cd_mensagem_avaliacao: 0,
		                tx_mensagem_avaliacao: dom.byId("txObs").value,
		                id_mensagem_ativa: domAttr.get("id_mensagem_avaliacao_ativa_pers", "checked"),
		                cd_produto: dijit.byId("pesCadProduto").value,
		                cd_curso: 0,
		                cursos: dijit.byId("cbCursos").value
                    })
                }).then(function (data) {  
                    try {
                        var itemsAlterados = jQuery.parseJSON(data).retorno;
                        var gridName = 'gridMensagemAvaliacao';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId('cadMensagemAvaliacao').hide();
                        grid.itensSelecionados = [];
                        
                        
                        buscarTodosItens(gridMensagemAvaliacao, 'todosItensMensagemAvaliacao', ['pesquisarMensagemAvaliacao', 'relatorioMensagemAvaliacao']);
                        searchMensagemAvaliacao(false);
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        
                        showCarregando();
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                }, function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemMensagemAvaliacao', error);
                });
            });
            
        
       
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarMensagemAvaliacao() {

    var gridName = 'gridMensagemAvaliacao';
    var grid = dijit.byId(gridName);

    if (!hasValue(dijit.byId("txObs").value) && !hasValue(dojo.byId("txObs").value)) {
	    var mensagensWeb = new Array();
	    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "È obrigatório preencher o campo mensagem para salvar o registro.");
	    apresentaMensagem('apresentadorMensagemMensagemAvaliacao', mensagensWeb);
	    return false;
    }

    if (!dijit.byId("formMensagemAvaliacaoCad").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditMensagemAvaliacao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_mensagem_avaliacao: dom.byId("cd_mensagem_avaliacao").value,
	            tx_mensagem_avaliacao: dom.byId("txObs").value,
	            id_mensagem_ativa: domAttr.get("id_mensagem_avaliacao_ativa_pers", "checked"),
	            cd_produto: dijit.byId("pesCadProduto").value,
                cd_curso: dijit.byId("cbCursos").value
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensMensagemAvaliacao");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadMensagemAvaliacao').hide();

                removeObjSort(grid.itensSelecionados, "cd_mensagem_avaliacao", dom.byId("cd_mensagem_avaliacao").value);
                insertObjSort(grid.itensSelecionados, "cd_mensagem_avaliacao", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoMensagemAvaliacao', 'cd_mensagemAvaliacao', 'selecionaTodosMensagemAvaliacao', ['pesquisarMensagemAvaliacao', 'relatorioMensagemAvaliacao'], 'todosItensMensagemAvaliacao');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_mensagem_avaliacao");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemMensagemAvaliacao', error);
        });
    });
}


function loadCursoByProdutoMensagemAvaliacao(cdProduto) {
	require([
		"dojo/_base/xhr"
	], function (xhr) {
		xhr.get({
            url: Endereco() + "/api/curso/getPesCursos?hasDependente=" + EnumTipoPesquisaCurso.HAS_ATIVOPROD + "&cd_curso=null" + "&cd_produto=" + cdProduto,
			preventCache: true,
			handleAs: "json",
			headers: { "Accept": "application/json", "Authorization": Token() },
			idProperty: "cd_curso"
		}).then(function (dataReturn) {
				try {
					apresentaMensagem('apresentadorMensagemAtividadeExtra', null);

                    if (dataReturn != null && dataReturn.retorno.length > 0) {
	                    dijit.byId("cbCursos").set('disabled', false);
						dijit.byId("cbCursos").reset();
						loadMultiCursoMensagemAvaliacao(dataReturn.retorno, "cbCursos");
					} else {
						dijit.byId("cbCursos").set('disabled', true);
						var mensagensWeb = new Array();
						mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Nenhum curso foi encontrado.");
						apresentaMensagem("apresentadorMensagemAtividadeExtra", mensagensWeb);
					}
				}
				catch (e) {
					postGerarLog(e);
				}
			},
			function (error) {
				apresentaMensagem('apresentadorMensagemAtividadeExtra', error);
			});
	});
}



function loadMultiCursoMensagemAvaliacao(items, field) {
	require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
			try {
				var dados = [];
				var cbCurso = [];
				$.each(items, function (index, val) {
					dados.push({
						"name": val.no_curso,
						"id": val.cd_curso
					});
					cbCurso.push({
						"cd_curso": val.cd_curso + "",
						"no_curso": val.no_curso
					});
				});

				var w = dijit.byId(field);
				//Adiciona a opção todos no checkedmultiselect
				cbCurso.unshift({
					"cd_curso": -1 + "",
					"no_curso": "Todos"
				});
				var storeMCurso = new dojo.data.ItemFileReadStore({
					data: {
						identifier: "cd_curso",
						label: "no_curso",
						items: cbCurso
					}
				});
				w.setStore(storeMCurso, []);
			}
			catch (e) {
				postGerarLog(e);
			}
		});
}

function checkedValuesTrue(cursos, cd_cursos, field) {
	require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
			try {
				dijit.byId('cbCursos').reset();
				getValuesMensagemAvaliacao();

				for (var i = 0; i < dijit.byId("cbCursos").options.length; i++) {
					for (var j = 0; j < cd_cursos.length; j++) {
						if (dijit.byId("cbCursos").options[i].item.cd_curso[0] == cd_cursos[j]) {
							console.log(dijit.byId("cbCursos").options[i].item.cd_curso[0] == cd_cursos[j]);
							dijit.byId("cbCursos").options[i].selected = true;
							dijit.byId("cbCursos").onChange(true);
						}
					}
				}
			}
			catch (e) {
				//showCarregando();
				postGerarLog(e);
			}
		});
}

var cbCursos = false;
function getValuesMensagemAvaliacao() {
	try {
		selected = dijit.byId("cbCursos").get('value');

		//se o item todos foi selecionado
		if (selected.indexOf(selected, "-1") == -1 && (!cbCursos || (hasValue(cbCursos) && cbCursos.value == "-1")) && dijit.byId("cbCursos").options[0].selected == true) {
			console.log('clicou');
			for (var i = 0; i < dijit.byId("cbCursos").options.length; i++) {
				dijit.byId("cbCursos").options[i].selected = true;
			}
			cbCursos = true; //marcou o check de todos          
		}

		//conta a quantidade de itens selecionados
		var contSelected = 0;
		for (var i = 0; i < dijit.byId("cbCursos").options.length; i++) {
			if (dijit.byId("cbCursos").options[i].selected == true) {
				contSelected++;
			}
		}

		//se já clicou em todos mas nao esta selecionado(desmarca todos)
		if (cbCursos && dijit.byId("cbCursos").options[0].selected == false) {

			for (var i = 0; i < dijit.byId("cbCursos").options.length; i++) {
				dijit.byId("cbCursos").options[i].selected = false;
			}
			cbCursos = false;//desmarcou o check de todos(click) 

		}

		//se ja clicou em todos mas desmarcou algum (desmarca o check todos)
		if (dijit.byId("cbCursos").options.length != contSelected && cbCursos == true) {
			dijit.byId("cbCursos").options[0].selected = false;
			cbCursos = false;//desmarcou o check de todos(tem itens selecionados ) 
		}

	}
	catch (e) {
		postGerarLog(e);
	}

}

function searchMensagemAvaliacao(limparItens) {
	require([
		"dojo/_base/xhr",
		"dojo/data/ObjectStore",
		"dojo/store/Cache",
		"dojo/store/Memory"
	], function (xhr, ObjectStore, Cache, Memory) {
		try {
			var mensagemAvaliacao = {};

            if (!hasValue(document.getElementById("mensagemAvaliacaoDesc").value)) {
				mensagemAvaliacao = {
					desc: "",
                    inicio: document.getElementById("cb_inicio_mensagem_avaliacao").checked,
                    status: retornaStatus("statusMensagemAvaliacao"),
                    produto: dijit.byId("cd_produtoM").get("value"),
                    curso: dijit.byId("cbCurso").get("value")
				};
			} else {
				mensagemAvaliacao = {
                    desc: encodeURIComponent(document.getElementById("mensagemAvaliacaoDesc").value),
                    inicio: document.getElementById("cb_inicio_mensagem_avaliacao").checked,
                    status: retornaStatus("statusMensagemAvaliacao"),
                    produto: dijit.byId("cd_produtoM").get("value"),
                    curso: dijit.byId("cbCurso").get("value")
				}

			}

			xhr.get({
                url: Endereco() + "/api/coordenacao/getMensagemAvaliacaoSearch",
				preventCache: true,
                content: mensagemAvaliacao,
				handleAs: "json",
				headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataMensagemAvaliacao) {

				var gridMensagemAvaliacao = dijit.byId("gridMensagemAvaliacao");

				if (limparItens)
					gridMensagemAvaliacao.itensSelecionados = [];

				gridMensagemAvaliacao.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: dataMensagemAvaliacao }) }));
			});
		}
		catch (e) {
			postGerarLog(e);
		}
	});
}
//#endregion

//#region selecionaTab
function selecionaTab(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) // Clicou na borda da aba
            tab = dojo.query(e.target)[0];
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabSala')
            abrirTabSala();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabProduto') {
            abrirTabProduto();
        }// else if produto ----- Fim da grade de Produtos
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabEstagio') {
            abrirTabEstagio();
        }
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabDuracao') {
            abrirTabDuracao();
        }// else if --- fim da grade de Duração
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabModalidade') {
            abrirTabModalidade();
        } // else if --- fim da grade de Modalidade
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabRegime') {
            abrirTabRegime();
        }// else if --- fim da grade de Regime
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabFeriado') {
            abrirTabFeriado();
        }//else if do Feriado
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabConceito') {
            abrirTabConceito();
        }  //fim da grade Conceito
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabEvento') {
            abrirTabEvento();
        }// else if evento
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMotivoFalta') {
            AbrirMotivoFalta();
        }// else if --- fim da grade de Motivo da Desistência
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMotivoDesistencia') {
            abrirTabMotivoDesistencia();
        }// else if --- fim da grade de Motivo da Desistência
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabAtividadeExtra') {
            abrirTabAtividadeExtra();
        }// else if --- fim da grade de Atividade Extra
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabParticipacao') {
            abrirTabParticipacao();
        }// else if --- fim da grade de Participação
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabCargaProfessor') {
            abrirTabCargaProfessor();
        }
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabNivel') {
            abrirTabNivel();
        }
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMensagemAvaliacao') {
	        abrirTabMensagemAvaliacao();
        }
        // else if --- fim da grade de Carga Professor
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region abrir Tabs
function abrirTabSala() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322982', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTabProduto() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirProduto').className)) {
            montarCadastroProduto();
        }
        apresentaMensagem('apresentadorMensagem', null);
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322983', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTabEstagio() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirEstagio').className)) {
            montarCadastroEstagio();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322984', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTabParticipacao() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirParticipacao').className)) {
            montarCadastroParticipacao();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTabCargaProfessor() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirCargaProfessor').className)) {
            montarCadastroCargaProfessor();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTabNivel() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirNivel').className)) {
            montarCadastroNivel();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTabMensagemAvaliacao() {
	try {
		apresentaMensagem('apresentadorMensagem', null);
		if (!hasValue(document.getElementById('incluirNivel').className)) {
			montarCadastroMensagemAvaliacao();
		}
		if (hasValue(dijit.byId("menuManual"))) {
			var menuManual = dijit.byId("menuManual");
			if (hasValue(menuManual.handler))
				menuManual.handler.remove();
			menuManual.handler = menuManual.on("click",
				function (e) {
					abrePopUp(Endereco() + '/Content/manual/manual.htm', '765px', '771px');
				});
		}
	}
	catch (e) {
		postGerarLog(e);
	}
}

function abrirTabDuracao() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirDuracao').className)) {
            montarCadastroDuracao();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322985', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTabModalidade() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirModalidade').className)) {
            montarCadastroModalidade();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322987', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTabRegime() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirRegime').className)) {
            montarCadastroRegime();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322987', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTabFeriado() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirFeriado').className)) {
            montarCadastroFeriado();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322989', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTabConceito() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirConceito').className)) {
            montarCadastroConceito();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322988', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
function abrirTabEvento() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirEvento').className)) {
            montarCadastroEvento();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322990', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
function AbrirMotivoFalta() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirMotivoFalta').className)) {
            montarCadastroMotivoFalta();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322991', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
function abrirTabMotivoDesistencia() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirMotivoDesistencia').className)) {
            montarCadastroMotivoDesistencia()
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322992', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
function abrirTabAtividadeExtra() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirAtividadeExtra').className)) {
            montarCadastroAtividadeExtra();
        }
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322993', '765px', '771px');
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region Persistência Sala

// Procura sala pelo nome
function searchSala(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        myStore = Cache(
		JsonRest({
            target: Endereco() + "/api/coordenacao/getSalaSearch?desc=" + encodeURIComponent(document.getElementById("salaDesc").value) + "&inicio=" + document.getElementById("inicioSala").checked + "&status=" + retornaStatus("statusSala") + "&online=" + dijit.byId("ckOnline").checked,
		    handleAs: "json",
		    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		    idProperty: "cd_sala"
		}), Memory({ idProperty: "cd_sala" }));
        dataStore = new ObjectStore({ objectStore: myStore });
        var gridSala = dijit.byId("gridSala");

        if (limparItens)
            gridSala.itensSelecionados = [];

        gridSala.setStore(dataStore);
    });
}
//** fim da função searchSala

//** Método de Incluir Sala
function incluirSala() {
    apresentaMensagem('apresentadorMensagemSala', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formSala").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertSala", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                no_sala: dom.byId("no_sala").value,
                id_sala_ativa: domAttr.get("id_sala_ativa", "checked"),
                nm_vaga_sala: dijit.byId("nm_vaga_sala").get("value"),
                id_sala_online: domAttr.get("ck_sala_online", "checked"),
                id_zoom: dijit.byId("edzoom").get("value"),
                dc_usuario_escola: dijit.byId("eduser").get("value"),
                dc_senha_usuario_escola: dijit.byId("edsenha").get("value"),
                dc_usuario_adm: dijit.byId("edapikey").get("value"),
                dc_senha_usuario_adm: dijit.byId("edapisenha").get("value")
            })
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridSala';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadSala').hide();
                    var ativo = dom.byId("id_sala_ativa").checked ? 1 : 2;
                    dijit.byId("statusSala").set("value", ativo);

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    insertObjSort(grid.itensSelecionados, "cd_sala", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionado', 'cd_sala', 'selecionaTodos', ['searchSala', 'relatorioSala'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_sala");
                }
                else
                    apresentaMensagem('apresentadorMensagemSala', data);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemSala', error.response.data);
        });
    });
}

//** Método de Editar Sala
function alterarSala() {
    var gridName = 'gridSala';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formSala").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditSala",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_sala: dom.byId("cd_sala").value,
                no_sala: dom.byId("no_sala").value,
                id_sala_ativa: domAttr.get("id_sala_ativa", "checked"),
                nm_vaga_sala: dijit.byId("nm_vaga_sala").get("value"),
                id_sala_online: domAttr.get("ck_sala_online", "checked"),
                id_zoom: dijit.byId("edzoom").get("value"),
                dc_usuario_escola: dijit.byId("eduser").get("value"),
                dc_senha_usuario_escola: dijit.byId("edsenha").get("value"),
                dc_usuario_adm: dijit.byId("edapikey").get("value"),
                dc_senha_usuario_adm: dijit.byId("edapisenha").get("value")
            })
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItens");
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadSala').hide();
                    //dijit.byId("salaDesc").set("value", String(dom.byId("no_sala").value));
                    var ativo = dom.byId("id_sala_ativa").checked ? 1 : 2;
                    dijit.byId("statusSala").set("value", ativo);
                    removeObjSort(grid.itensSelecionados, "cd_sala", dom.byId("cd_sala").value);
                    insertObjSort(grid.itensSelecionados, "cd_sala", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionado', 'cd_sala', 'selecionaTodos', ['searchSala', 'relatorioSala'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_sala");

                } else
                    apresentaMensagem('apresentadorMensagemSala', data);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemSala', error);
        });
    });
}

//** Método para Deletar a Sala
function deletarSala(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            if (dojo.byId('cd_sala').value != 0) {
                itensSelecionados = [{
                    cd_sala: dom.byId("cd_sala").value,
                    no_sala: dom.byId("no_sala").value,
                    id_sala_ativa: domAttr.get("id_sala_ativa", "checked"),
                    nm_vaga_sala: dijit.byId('nm_vaga_sala').get("value"),
                    id_sala_online: domAttr.get("ck_sala_online", "checked"),
                    id_zoom: dijit.byId("edzoom").get("value"),
                    dc_usuario_escola: dijit.byId("eduser").get("value"),
                    dc_senha_usuario_escola: dijit.byId("edsenha").get("value"),
                    dc_usuario_adm: dijit.byId("edapikey").get("value"),
                    dc_senha_usuario_adm: dijit.byId("edapisenha").get("value")
                }];
            }
        }
        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteSala",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItens");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadSala").hide();
                    dijit.byId("salaDesc").set("value", '');
                    searchSala(true);
                    dijit.byId('gridSala').itensSelecionados = null;

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("searchSala").set('disabled', false);
                    dijit.byId("relatorioSala").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                else
                    apresentaMensagem('apresentadorMensagemSala', data);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
		function (error) {
		    if (!hasValue(dojo.byId("cadSala").style.display))
		        apresentaMensagem('apresentadorMensagemSala', error);
		    else
		        apresentaMensagem('apresentadorMensagem', error);
		});
    })
}
//#endregion

//#region Persistência produto

function searchProduto(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: !hasValue(document.getElementById("produtoDesc").value) ? Endereco() + "/api/coordenacao/getProdutoSearch?desc=&abrev=" + encodeURIComponent(document.getElementById("produtoAbrev").value) + "&inicio=" + document.getElementById("inicioProduto").checked + "&status=" + retornaStatus("statusProduto") : Endereco() + "/api/coordenacao/getProdutoSearch?desc=" + encodeURIComponent(document.getElementById("produtoDesc").value) + "&abrev=" + encodeURIComponent(document.getElementById("produtoAbrev").value) + "&inicio=" + document.getElementById("inicioProduto").checked + "&status=" + retornaStatus("statusProduto"),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
		    }), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridProduto = dijit.byId("gridProduto");

            if (limparItens)
                gridProduto.itensSelecionados = [];

            gridProduto.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//** fim da função searchProduto

//** Método de Incluir Produto
function incluirProduto() {
    apresentaMensagem('apresentadorMensagemProduto', null);
    apresentaMensagem('apresentadorMensagem', null);

    if (!dijit.byId("formProduto").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertProduto", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                no_produto: dom.byId("no_produto").value,
                no_produto_abreviado: dom.byId("no_produto_abreviado").value,
                id_produto_ativo: domAttr.get("id_produto_ativo", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridProduto';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadProduto').hide();
                var ativo = dom.byId("id_produto_ativo").checked ? 1 : 2;
                dijit.byId("statusProduto").set("value", ativo);
                if (!hasValue(grid.itensSelecionados)) { grid.itensSelecionados = []; }
                insertObjSort(grid.itensSelecionados, "cd_produto", itemAlterado);
                buscarItensSelecionados(gridName, 'selecionadoProduto', 'cd_produto', 'selecionaTodosProduto', ['searchProduto', 'relatorioProduto'], 'todosItensProduto');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_produto");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemProduto', error);
        });
    });
}
//** fim da inserção de produto

//** Método de Editar produto
function alterarProduto() {
    var gridName = 'gridProduto';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formProduto").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditProduto",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_produto: dom.byId("cd_produto").value,
                no_produto: dom.byId("no_produto").value,
                no_produto_abreviado: dom.byId("no_produto_abreviado").value,
                id_produto_ativo: domAttr.get("id_produto_ativo", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensProduto");
                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadProduto').hide();
                var ativo = dom.byId("id_produto_ativo").checked ? 1 : 2;
                dijit.byId("statusProduto").set("value", ativo);
                removeObjSort(grid.itensSelecionados, "cd_produto", dom.byId("cd_produto").value);
                insertObjSort(grid.itensSelecionados, "cd_produto", itemAlterado);
                buscarItensSelecionados(gridName, 'selecionadoProduto', 'cd_produto', 'selecionaTodosProdutos', ['searchProduto', 'relatorioProduto'], 'todosItens');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_produto");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemProduto', error);
        });
    });
}
// fim da edição de produto

//** Método para Deletar o Produto
function deletarProduto(itensSelecionados) {
    if (!dijit.byId("formProduto").validate()) {
        // return false;
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgNotSelectReg);
        apresentaMensagem('apresentadorMensagemProduto', mensagensWeb);
    }
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                if (dojo.byId('cd_produto').value != 0) {
                    itensSelecionados = [{
                        cd_produto: dom.byId("cd_produto").value,
                        no_produto: dom.byId("no_produto").value,
                        no_produto_abreviado: dom.byId("no_produto_abreviado").value,
                        id_produto_ativo: domAttr.get("id_produto_ativo", "checked")
                    }];
                }
            }
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteProduto",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensProduto");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadProduto').hide();
                    dijit.byId("produtoDesc").set("value", '');
                    dijit.byId("produtoAbrev").set("value", '');
                    dijit.byId('gridProduto').itensSelecionados = null;
                    searchProduto(true);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("searchProduto").set('disabled', false);
                    dijit.byId("relatorioProduto").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                else
                    apresentaMensagem('apresentadorMensagemProduto', data);
                showCarregando();
            }, function (error) {
                showCarregando();
                if (!hasValue(dojo.byId("cadProduto").style.display))
                    apresentaMensagem('apresentadorMensagemProduto', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region Persistência Evento
function searchEvento(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: !hasValue(document.getElementById("eventoDesc").value) ? Endereco() + "/api/coordenacao/getEventoSearch?desc=&inicio=" + document.getElementById("inicioEvento").checked + "&status=" + retornaStatus("statusEvento") : Endereco() + "/api/coordenacao/getEventoSearch?desc=" + encodeURIComponent(document.getElementById("eventoDesc").value) + "&inicio=" + document.getElementById("inicioEvento").checked + "&status=" + retornaStatus("statusEvento"),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_evento"
		    }), Memory({ idProperty: "cd_evento" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridEvento = dijit.byId("gridEvento");

            if (limparItens)
                gridEvento.itensSelecionados = [];

            gridEvento.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//** fim da função searchEvento

//** Método de Incluir Evento
function incluirEvento() {
    apresentaMensagem('apresentadorMensagemEvento', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formEvento").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertEvento", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                no_evento: dom.byId("no_evento").value,
                id_evento_ativo: domAttr.get("id_evento_ativo", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridEvento';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadEvento').hide();

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                insertObjSort(grid.itensSelecionados, "cd_evento", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoEvento', 'cd_evento', 'selecionaTodosEventos', ['searchEvento', 'relatorioEvento'], 'todosItensEvento');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_evento");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemEvento', error);
        });
    });
}

//** Método de Editar Evento
function alterarEvento() {

    var gridName = 'gridEvento';
    var grid = dijit.byId(gridName);

    if (!dijit.byId("formEvento").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditEvento",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_evento: dom.byId("cd_evento").value,
                no_evento: dom.byId("no_evento").value,
                id_evento_ativo: domAttr.get("id_evento_ativo", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensEvento");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadEvento').hide();

                removeObjSort(grid.itensSelecionados, "cd_evento", dom.byId("cd_evento").value);
                insertObjSort(grid.itensSelecionados, "cd_evento", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoEvento', 'cd_evento', 'selecionaTodosEventos', ['searchEvento', 'relatorioEvento'], 'todosItensEvento');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_evento");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemEvento', error);
        });
    });
}

//** Método para Deletar a Evento
function deletarEvento(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_evento').value != 0)
                    itensSelecionados = [{
                        cd_evento: dom.byId("cd_evento").value,
                        no_evento: dom.byId("no_evento").value,
                        id_evento_ativo: domAttr.get("id_evento_ativo", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteEvento",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensEvento");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadEvento').hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridEvento').itensSelecionados, "cd_evento", itensSelecionados[r].cd_evento);
                    searchEvento(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("searchEvento").set('disabled', false);
                    dijit.byId("relatorioEvento").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                else
                    apresentaMensagem('apresentadorMensagemEvento', data);
            }, function (error) {
                if (!hasValue(dojo.byId("cadEvento").style.display))
                    apresentaMensagem('apresentadorMensagemEvento', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    })
}
//#endregion

//#region Persistência Duracao
function searchDuracao(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: !hasValue(document.getElementById("duracaoDesc").value) ? Endereco() + "/api/coordenacao/getDuracaoSearch?desc=&inicio=" + document.getElementById("inicioDuracao").checked + "&status=" + retornaStatus("statusDuracao") : Endereco() + "/api/coordenacao/getDuracaoSearch?desc=" + encodeURIComponent(document.getElementById("duracaoDesc").value) + "&inicio=" + document.getElementById("inicioDuracao").checked + "&status=" + retornaStatus("statusDuracao"),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_duracao"
		    }), Memory({ idProperty: "cd_duracao" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridDuracao = dijit.byId("gridDuracao");
            if (limparItens) {
                gridDuracao.itensSelecionados = [];
            }
            gridDuracao.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//** fim da função searchDuração

//** Método de Incluir Duração
function incluirDuracao() {
    apresentaMensagem('apresentadorMensagemDuracao', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formDuracao").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertDuracao", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                dc_duracao: dom.byId("dc_duracao").value,
                id_duracao_ativa: domAttr.get("id_duracao_ativa", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridDuracao';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadDuracao').hide();

                if (!hasValue(grid.itensSelecionados)) { grid.itensSelecionados = []; }

                insertObjSort(grid.itensSelecionados, "cd_duracao", itemAlterado);
                buscarItensSelecionados(gridName, 'selecionadoDuracao', 'cd_duracao', 'selecionaTodosDuracao', ['searchDuracao', 'relatorioDuracao'], 'todosItensDuracao');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_duracao");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemDuracao', error);
        });
    });
}

//** fim da inserção de duração

//** Método de Editar Duração
function alterarDuracao() {
    var gridName = 'gridDuracao';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formDuracao").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditDuracao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_duracao: dom.byId("cd_duracao").value,
                dc_duracao: dom.byId("dc_duracao").value,
                id_duracao_ativa: domAttr.get("id_duracao_ativa", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensDuracao");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadDuracao').hide();
                removeObjSort(grid.itensSelecionados, "cd_duracao", dom.byId("cd_duracao").value);
                insertObjSort(grid.itensSelecionados, "cd_duracao", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoMtvCancelBolsa', 'cd_duracao', 'selecionaTodosDuracao', ['searchDuracao', 'relatorioDuracao'], 'todosItensDuracao');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_duracao");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemDuracao', error);
        });
    });
}
// fim da edição de Duração

//** Método para Deletar o Duração
function deletarDuracao(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_duracao').value != 0)
                itensSelecionados = [{
                    cd_duracao: dom.byId("cd_duracao").value,
                    dc_duracao: dom.byId("dc_duracao").value,
                    id_duracao_ativa: domAttr.get("id_duracao_ativa", "checked")
                }];
        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteDuracao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            if (!hasValue(data.erro)) {
                var todos = dojo.byId("todosItensDuracao");
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadDuracao').hide();
                for (var r = itensSelecionados.length - 1; r >= 0; r--) {
                    removeObjSort(dijit.byId('gridDuracao').itensSelecionados, "cd_duracao", itensSelecionados[r].cd_duracao);
                }
                searchDuracao(true);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("searchDuracao").set('disabled', false);
                dijit.byId("relatorioDuracao").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            else
                apresentaMensagem('apresentadorMensagemDuracao', data);
        }, function (error) {
            if (!hasValue(dojo.byId("cadDuracao").style.display))
                apresentaMensagem('apresentadorMensagemDuracao', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    })
}
//#endregion

//#region Persistência AtividadeExtra
function searchAtividadeExtra(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: !hasValue(document.getElementById("atividadeExtraDesc").value) ? Endereco() + "/api/coordenacao/getTpAtividadeExtraSearch?desc=&inicio=" + document.getElementById("inicioAtividadeExtra").checked + "&status=" + retornaStatus("statusAtividadeExtra") : Endereco() + "/api/coordenacao/getTpAtividadeExtraSearch?desc=" + encodeURIComponent(document.getElementById("atividadeExtraDesc").value) + "&inicio=" + document.getElementById("inicioAtividadeExtra").checked + "&status=" + retornaStatus("statusAtividadeExtra"),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_tipo_atividade_extra"
		    }), Memory({ idProperty: "cd_tipo_atividade_extra" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridAtividadeExtra = dijit.byId("gridAtividadeExtra");
            if (limparItens) {
                gridAtividadeExtra.itensSelecionados = [];
            }
            gridAtividadeExtra.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};
//** fim da função searchAtividadeExtra

//** Método de Incluir Atividade Extra
function incluirAtividadeExtra() {
    apresentaMensagem('apresentadorMensagemAtividadeExtra', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formAtividadeExtra").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertAtividadeExtra", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                no_tipo_atividade_extra: dom.byId("no_tipo_atividade_extra").value,
                id_tipo_atividade_extra_ativa: domAttr.get("id_tipo_atividade_extra_ativa", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridAtividadeExtra';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadAtividadeExtra').hide();

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                insertObjSort(grid.itensSelecionados, "cd_tipo_atividade_extra", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadaAtividadeExtra', 'cd_tipo_atividade_extra', 'selecionaTodasAtividadesExtras', ['searchAtividadeExtra', 'relatorioAtividadeExtra'], 'todosItensAtividadeExtra');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_tipo_atividade_extra");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemAtividadeExtra', error);
        });
    });
};
//** fim da inserção de Atividade Extra

//** Método de Editar Atividade Extra
function alterarAtividadeExtra() {
    var gridName = 'gridAtividadeExtra';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formAtividadeExtra").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditAtividadeExtra",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_tipo_atividade_extra: dom.byId("cd_tipo_atividade_extra").value,
                no_tipo_atividade_extra: dom.byId("no_tipo_atividade_extra").value,
                id_tipo_atividade_extra_ativa: domAttr.get("id_tipo_atividade_extra_ativa", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensAtividadeExtra");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadAtividadeExtra').hide();

                removeObjSort(grid.itensSelecionados, "cd_tipo_atividade_extra", dom.byId("cd_tipo_atividade_extra").value);
                insertObjSort(grid.itensSelecionados, "cd_tipo_atividade_extra", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadaAtividadeExtra', 'cd_tipo_atividade_extra', 'selecionaTodasAtividadesExtras', ['searchAtividadeExtra', 'relatorioAtividadeExtra'], 'todosItensAtividadeExtra');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_tipo_atividade_extra");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemAtividadeExtra', error);
        });
    });
};

//** Método para Deletar o Atividade Extra
function deletarAtividadeExtra(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_tipo_atividade_extra').value != 0)
                itensSelecionados = [{
                    cd_tipo_atividade_extra: dom.byId("cd_tipo_atividade_extra").value,
                    no_tipo_atividade_extra: dom.byId("no_tipo_atividade_extra").value,
                    id_tipo_atividade_extra_ativa: domAttr.get("id_tipo_atividade_extra_ativa", "checked")
                }];
        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteAtividadeExtra",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItensAtividadeExtra");
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadAtividadeExtra').hide();
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridAtividadeExtra').itensSelecionados, "cd_tipo_atividade_extra", itensSelecionados[r].cd_tipo_atividade_extra);
                searchAtividadeExtra(true);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("searchAtividadeExtra").set('disabled', false);
                dijit.byId("relatorioAtividadeExtra").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            if (!hasValue(dojo.byId("cadAtividadeExtra").style.display))
                apresentaMensagem('apresentadorMensagemAtividadeExtra', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    })
};
//** fim do delete de Atividade Extra

//#endregion

//#region Persistência Motivo Desitencia
function searchMotivoDesistencia(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: !hasValue(document.getElementById("motivoDesistenciaDesc").value) ? Endereco() + "/api/coordenacao/getMotivoDesistenciaSearch?desc=&inicio=" + document.getElementById("inicioMotivoDesistencia").checked + "&status=" + retornaStatus("statusMotivoDesistencia") + "&isCancelamento=" + dijit.byId('isCancelamento').checked : Endereco() + "/api/coordenacao/getMotivoDesistenciaSearch?desc=" + encodeURIComponent(document.getElementById("motivoDesistenciaDesc").value) + "&inicio=" + document.getElementById("inicioMotivoDesistencia").checked + "&status=" + retornaStatus("statusMotivoDesistencia") + "&isCancelamento=" + dijit.byId('isCancelamento').checked,
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_motivo_desistencia"
		    }), Memory({ idProperty: "cd_motivo_desistencia" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridMotivoDesistencia = dijit.byId("gridMotivoDesistencia");

            if (limparItens)
                gridMotivoDesistencia.itensSelecionados = [];

            gridMotivoDesistencia.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};
//** fim da função search Motivo da Desistencia

//** Método de Incluir Motivo Desistência
function incluirMotivoDesistencia() {
    apresentaMensagem('apresentadorMensagemMotivoDesistencia', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formMotivoDesistencia").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertMotivoDesistencia", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                dc_motivo_desistencia: dom.byId("dc_motivo_desistencia").value,
                id_motivo_desistencia_ativo: domAttr.get("id_motivo_desistencia_ativo", "checked"),
                id_cancelamento: dijit.byId("id_cancelamento").checked
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridMotivoDesistencia';
                var grid = dijit.byId(gridName);
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadMotivoDesistencia').hide();
                if (!hasValue(grid.itensSelecionados)) {
                    grid.itensSelecionados = [];
                }
                insertObjSort(grid.itensSelecionados, "cd_motivo_desistencia", itemAlterado);
                buscarItensSelecionados(gridName, 'selecionadoMotivoDesistencia', 'cd_motivo_desistencia', 'selecionaTodosMotivosDesistencia', ['searchMotivoDesistencia', 'relatorioMotivoDesistencia'], 'todosItensMotivosDesistencia');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_motivo_desistencia");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemMotivoDesistencia', error);
        });
    });
};
//** fim da inserção de Motivo da Desistência

//** Método de Editar Motivo Desistência
function alterarMotivoDesistencia() {
    var gridName = 'gridMotivoDesistencia';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formMotivoDesistencia").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditMotivoDesistencia",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_motivo_desistencia: dom.byId("cd_motivo_desistencia").value,
                dc_motivo_desistencia: dom.byId("dc_motivo_desistencia").value,
                id_motivo_desistencia_ativo: domAttr.get("id_motivo_desistencia_ativo", "checked"),
                id_cancelamento: dijit.byId("id_cancelamento").checked
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensMotivosDesistencia");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadMotivoDesistencia').hide();

                removeObjSort(grid.itensSelecionados, "cd_motivo_desistencia", dom.byId("cd_motivo_desistencia").value);
                insertObjSort(grid.itensSelecionados, "cd_motivo_desistencia", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoMotivoDesistencia', 'cd_motivo_desistencia', 'selecionaTodosMotivosDesistencia', ['searchMotivoDesistencia', 'relatorioMotivoDesistencia'], 'todosItensMotivosDesistencia');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_motivo_desistencia");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemMotivoDesistencia', error);
        });
    });
};

// fim da edição de  Motivo Desistência
//** Método para Deletar o Motivo Desistência
function deletarMotivoDesistencia(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_motivo_desistencia').value != 0)
                itensSelecionados = [{
                    cd_motivo_desistencia: dom.byId("cd_motivo_desistencia").value,
                    dc_motivo_desistencia: dom.byId("dc_motivo_desistencia").value,
                    id_motivo_desistencia_ativo: domAttr.get("id_motivo_desistencia_ativo", "checked")
                }];

        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteMotivoDesistencia",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItensMotivosDesistencia");
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadMotivoDesistencia').hide();
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridMotivoDesistencia').itensSelecionados, "cd_motivo_desistencia", itensSelecionados[r].cd_motivo_desistencia);
                searchMotivoDesistencia(true);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("searchMotivoDesistencia").set('disabled', false);
                dijit.byId("relatorioMotivoDesistencia").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            if (!hasValue(dojo.byId("cadMotivoDesistencia").style.display))
                apresentaMensagem('apresentadorMensagemMotivoDesistencia', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    })
};
//** fim do delete de Motivo Desistência
//#endregion

//#region Persistência MotivoFalta
function searchMotivoFalta(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: !hasValue(document.getElementById("motivoFaltaDesc").value) ? Endereco() + "/api/coordenacao/getMotivoFaltaSearch?desc=&inicio=" + document.getElementById("inicioMotivoFalta").checked + "&status=" + retornaStatus("statusMotivoFalta") : Endereco() + "/api/coordenacao/getMotivoFaltaSearch?desc=" + encodeURIComponent(document.getElementById("motivoFaltaDesc").value) + "&inicio=" + document.getElementById("inicioMotivoFalta").checked + "&status=" + retornaStatus("statusMotivoFalta"),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_motivo_falta"
		    }), Memory({ idProperty: "cd_motivo_falta" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridMotivoFalta = dijit.byId("gridMotivoFalta");

            if (limparItens)
                gridMotivoFalta.itensSelecionados = [];

            gridMotivoFalta.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};
//** fim da função search Motivo da Desistencia

//** Método de Incluir Motivo da Falta
function incluirMotivoFalta() {
    apresentaMensagem('apresentadorMensagemMotivoFalta', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formMotivoFalta").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertMotivoFalta", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                dc_motivo_falta: dom.byId("dc_motivo_falta").value,
                id_motivo_falta_ativa: domAttr.get("id_motivo_falta_ativa", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridMotivoFalta';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadMotivoFalta').hide();

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                insertObjSort(grid.itensSelecionados, "cd_motivo_falta", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoMotivoFalta', 'cd_motivo_falta', 'selecionaTodosMotivosFalta', ['searchMotivoFalta', 'relatorioMotivoFalta'], 'todosItensMotivoFalta');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_motivo_falta");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemMotivoFalta', error);
        });
    });
};
//** fim da inserção de Motivo da Falta

//** Método de Editar Motivo Desistência
function alterarMotivoFalta() {
    var gridName = 'gridMotivoFalta';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formMotivoFalta").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditMotivoFalta",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_motivo_falta: dom.byId("cd_motivo_falta").value,
                dc_motivo_falta: dom.byId("dc_motivo_falta").value,
                id_motivo_falta_ativa: domAttr.get("id_motivo_falta_ativa", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensMotivoFalta");

                if (!hasValue(grid.itensSelecionados)) {
                    grid.itensSelecionados = [];
                }
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadMotivoFalta').hide();

                removeObjSort(grid.itensSelecionados, "cd_motivo_falta", dom.byId("cd_motivo_falta").value);
                insertObjSort(grid.itensSelecionados, "cd_motivo_falta", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoMtvCancelBolsa', 'cd_motivo_falta', 'selecionaTodosMtvCancelBolsa', ['searchMotivoFalta', 'relatorioMotivoFalta'], 'todosItensMotivoFalta');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_motivo_falta");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemMotivoFalta', error);
        });
    });
};
// fim da edição de  Motivo Desistência

//** Método para Deletar o Motivo da Falta
function deletarMotivoFalta(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_motivo_falta').value != 0)
                itensSelecionados = [{
                    cd_motivo_falta: dom.byId("cd_motivo_falta").value,
                    dc_motivo_falta: dom.byId("dc_motivo_falta").value,
                    id_motivo_falta_ativa: domAttr.get("id_motivo_falta_ativa", "checked")
                }];
        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteMotivoFalta",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItensMotivoFalta");
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("cadMotivoFalta").hide();
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridMotivoFalta').itensSelecionados, "cd_motivo_falta", itensSelecionados[r].cd_motivo_falta);
                searchMotivoFalta(true);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("searchMotivoFalta").set('disabled', false);
                dijit.byId("relatorioMotivoFalta").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            if (!hasValue(dojo.byId("cadMotivoFalta").style.display))
                apresentaMensagem('apresentadorMensagemMotivoFalta', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    })
};
//** fim do delete de Motivo da Falta
//#endregion

//#region Persistência Modalidade
function searchModalidade(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: !hasValue(document.getElementById("modalidadeDesc").value) ? Endereco() + "/api/coordenacao/getModalidadeSearch?desc=&inicio=" + document.getElementById("inicioModalidade").checked + "&status=" + retornaStatus("statusModalidade") : Endereco() + "/api/coordenacao/getModalidadeSearch?desc=" + encodeURIComponent(document.getElementById("modalidadeDesc").value) + "&inicio=" + document.getElementById("inicioModalidade").checked + "&status=" + retornaStatus("statusModalidade"),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_modalidade"
		    }), Memory({ idProperty: "cd_modalidade" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridModalidade = dijit.byId("gridModalidade");

            if (limparItens)
                gridModalidade.itensSelecionados = [];

            gridModalidade.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};
//** fim da função search Modalidade

//** Método de Incluir Modalidade
function incluirModalidade() {
    apresentaMensagem('apresentadorMensagemModalidade', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formModalidade").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertModalidade", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                no_modalidade: dom.byId("no_modalidade").value,
                id_modalidade_ativa: domAttr.get("id_modalidade_ativa", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridModalidade';
                var grid = dijit.byId(gridName);
                if (!hasValue(grid.itensSelecionados)) { grid.itensSelecionados = []; }

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadModalidade').hide();

                insertObjSort(grid.itensSelecionados, "cd_modalidade", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoModalidade', 'cd_modalidade', 'selecionaTodasModalidades', ['searchModalidade', 'relatorioModalidade'], 'todosItensModalidade');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_modalidade");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemModalidade', error);
        });
    });
};

//** fim da inserção de Modalidade

//** Método de Editar Modalidade
function alterarModalidade(msg, type, form) {
    var gridName = 'gridModalidade';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formModalidade").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditModalidade",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_modalidade: dom.byId("cd_modalidade").value,
                no_modalidade: dom.byId("no_modalidade").value,
                id_modalidade_ativa: domAttr.get("id_modalidade_ativa", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensModalidade");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadModalidade').hide();
                removeObjSort(grid.itensSelecionados, "cd_modalidade", dom.byId("cd_modalidade").value);
                insertObjSort(grid.itensSelecionados, "cd_modalidade ", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoModalidade', 'cd_modalidade', 'selecionaTodasModalidades', ['searchModalidade', 'relatorioModalidade'], 'todosItensModalidade');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_modalidade");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemModalidade', error);
        });
    });
};
// fim da edição de  Modalidade

//** Método para Deletar Modalidade
function deletarModalidade(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_modalidade').value != 0)
                itensSelecionados = [{
                    cd_modalidade: dom.byId("cd_modalidade").value,
                    no_modalidade: dom.byId("no_modalidade").value,
                    id_modalidade_ativa: domAttr.get("id_modalidade_ativa", "checked")
                }];
        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteModalidade",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItensModalidade");
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadModalidade').hide();
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridModalidade').itensSelecionados, "cd_modalidade", itensSelecionados[r].cd_modalidade);

                searchModalidade(true);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("searchModalidade").set('disabled', false);
                dijit.byId("relatorioModalidade").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            if (!hasValue(dojo.byId("cadModalidade").style.display))
                apresentaMensagem('apresentadorMensagemModalidade', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    });
};
//** fim do delete de Modalidade

//#endregion

//#region Persistência Regime

// Procura Regime pelo nome
function searchRegime(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: !hasValue(document.getElementById("regimeDesc").value) ? Endereco() + "/api/coordenacao/getRegimeSearch?desc=&abrev=" + encodeURIComponent(document.getElementById("regimeAbrev").value) + "&inicio=" + document.getElementById("inicioRegime").checked + "&status=" + retornaStatus("statusRegime") : Endereco() + "/api/coordenacao/getRegimeSearch?desc=" + encodeURIComponent(document.getElementById("regimeDesc").value) + "&abrev=" + encodeURIComponent(document.getElementById("regimeAbrev").value) + "&inicio=" + document.getElementById("inicioRegime").checked + "&status=" + retornaStatus("statusRegime"),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_regime"
		    }), Memory({ idProperty: "cd_regime" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridRegime = dijit.byId("gridRegime");

            if (limparItens)
                gridRegime.itensSelecionados = [];

            gridRegime.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};
//** fim da função search Regime

//** Método de Incluir Regime
function incluirRegime() {
    apresentaMensagem('apresentadorMensagemRegime', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formRegime").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertRegime", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                no_regime: dom.byId("no_regime").value,
                no_regime_abreviado: dom.byId("no_regime_abreviado").value,
                id_regime_ativo: domAttr.get("id_regime_ativo", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridRegime';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadRegime').hide();

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                insertObjSort(grid.itensSelecionados, "cd_regime", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoRegime', 'cd_regime', 'selecionaTodosRegime', ['searchRegime', 'relatorioRegime'], 'todosItensRegime');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_regime");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemRegime', error);
        });
    });
};
//** fim da inserção de Regime getValorDefault("regimeDesc", "no_regime");

//** Método de Editar Regime
function alterarRegime(msg, type, form) {
    var gridName = 'gridRegime';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formRegime").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditRegime",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_regime: dom.byId("cd_regime").value,
                no_regime: dom.byId("no_regime").value,
                no_regime_abreviado: dom.byId("no_regime_abreviado").value,
                id_regime_ativo: domAttr.get("id_regime_ativo", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensRegime");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadRegime').hide();

                removeObjSort(grid.itensSelecionados, "cd_regime", dom.byId("cd_regime").value);
                insertObjSort(grid.itensSelecionados, "cd_regime", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoRegime', 'cd_regime', 'selecionaTodosRegimes', ['searchRegime', 'relatorioRegime'], 'todosItensRegime');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_regime");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemRegime', error);
        });
    });
};
// fim da edição de  Regime

//** Método para Deletar Regime
function deletarRegime(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_regime').value != 0)
                itensSelecionados = [{
                    cd_regime: dom.byId("cd_regime").value,
                    no_regime: dom.byId("no_regime").value,
                    no_regime_abreviado: dom.byId("no_regime_abreviado").value,
                    id_regime_ativo: domAttr.get("id_regime_ativo", "checked")
                }];

        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteRegime",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItensRegime");

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadRegime').hide();
                dijit.byId("regimeDesc").set("value", '');

                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridRegime').itensSelecionados, "cd_regime", itensSelecionados[r].cd_regime);

                searchRegime(true);

                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("searchRegime").set('disabled', false);
                dijit.byId("relatorioRegime").set('disabled', false);

                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            if (!hasValue(dojo.byId("cadRegime").style.display))
                apresentaMensagem('apresentadorMensagemRegime', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    });
};
//** fim do delete de Regime
//#endregion

//#region Persistência Conceito
function searchConceito(limparItens) {
    require([
	"dojo/_base/xhr",
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (xhr, JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: !hasValue(document.getElementById("conceitoDesc").value) ? Endereco() + "/api/coordenacao/getConceitoSearch?desc=&inicio=" + document.getElementById("inicioConceito").checked + "&status=" + retornaStatus("statusConceito") + "&CodP=" + dijit.byId("codProduto").get("value") : Endereco() + "/api/coordenacao/getConceitoSearch?desc=" + encodeURIComponent(document.getElementById("conceitoDesc").value) + "&inicio=" + document.getElementById("inicioConceito").checked + "&status=" + retornaStatus("statusConceito") + "&CodP=" + dijit.byId("codProduto").get("value"),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_conceito"
		    }), Memory({ idProperty: "cd_conceito" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridConceito = dijit.byId("gridConceito");
            gridConceito.setStore(dataStore);
            //Recarregar o select do produto pois pode ter havido modificações
            xhr.get({
                url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_CONCEITO + "&cd_produto=null",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Authorization": Token() }
            }).then(function (dataProd) {
                loadProduto(jQuery.parseJSON(dataProd).retorno, 'codProduto');
            });
            if (limparItens) {
                gridConceito.itensSelecionados = [];
            }
            gridConceito.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};
//** fim da função search Conceito

//** Método de Incluir Conceito
function incluirConceito() {
    apresentaMensagem('apresentadorMensagemConceito', null);
    apresentaMensagem('apresentadorMensagem', null);

    if (!dijit.byId("formConceito").validate())
        return false;
    if (!validarPercentual())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertConceito", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                cd_conceito: dojo.byId("cd_conceito").value,
                no_conceito: dojo.byId("no_conceito").value,
                id_conceito_ativo: domAttr.get("id_conceito_ativo", "checked"),
                pc_inicial_conceito: dojo.number.parse(dom.byId("pc_inicial_conceito").value),
                pc_final_conceito: dojo.number.parse(dom.byId("pc_final_conceito").value),
                cd_produto: dijit.byId("cd_produtoC").value,
                no_produto: dojo.byId("cd_produtoC").value,
                vl_nota_participacao: dojo.number.parse(dojo.byId("vl_nota_participacao").value)
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridConceito';
                var grid = dijit.byId(gridName);
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadConceito').hide();
                //			dijit.byId("codProduto").set("value", dijit.byId("codProduto").set("value", dijit.byId("cd_produtoC").value).value == "" ? 0 : dijit.byId("cd_produtoC").value);
                if (!hasValue(grid.itensSelecionados)) {
                    grid.itensSelecionados = [];
                }
                insertObjSort(grid.itensSelecionados, "cd_conceito", itemAlterado);
                buscarItensSelecionados(gridName, 'selecionadoConceito', 'cd_conceito', 'selecionaTodosConceitos', ['searchConceito', 'relatorioConceito'], 'todosItensConceito');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_conceito");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemConceito', error);
        });
    });
};

function validarPercentual() {
    try {
        var percIni = unmaskFixed(dijit.byId('pc_inicial_conceito').get('value'), 2);
        var percFim = unmaskFixed(dijit.byId('pc_final_conceito').get('value'), 2);
        if (percIni > percFim) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPercentualConceito);
            apresentaMensagem("apresentadorMensagemConceito", mensagensWeb);
            return false;
        }
        return true;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//** fim da inserção de Conceito getValorDefault("conceitoDesc", "no_conceito");

//** Método de Editar Conceito
function alterarConceito(msg, type, form) {
    var gridName = 'gridConceito';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formConceito").validate())
        return false;

    if (!validarPercentual())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditConceito",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_conceito: dom.byId("cd_conceito").value,
                no_conceito: dom.byId("no_conceito").value,
                id_conceito_ativo: domAttr.get("id_conceito_ativo", "checked"),
                pc_inicial_conceito: dojo.number.parse(dom.byId("pc_inicial_conceito").value),
                pc_final_conceito: dojo.number.parse(dom.byId("pc_final_conceito").value),
                cd_produto: dijit.byId("cd_produtoC").value,
                no_produto: dojo.byId("cd_produtoC").value,
                vl_nota_participacao: dojo.number.parse(dojo.byId("vl_nota_participacao").value)
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensConceito");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadConceito').hide();
                dijit.byId("codProduto").set("value", dijit.byId("codProduto").set("value", dijit.byId("cd_produtoC").value).value == "" ? 0 : dijit.byId("cd_produtoC").value);

                removeObjSort(grid.itensSelecionados, "cd_conceito", dom.byId("cd_conceito").value);
                insertObjSort(grid.itensSelecionados, "cd_conceito", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoConceito', 'cd_conceito', 'selecionaTodosConceitos', ['searchConceito', 'relatorioConceito'], 'todosItensConceito');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_conceito");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemConceito', error);
        });
    });
};
// fim da edição de  Conceito

//** Método para Deletar Conceito
function deletarConceito(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_conceito').value != 0)
                itensSelecionados = [{
                    cd_conceito: dom.byId("cd_conceito").value,
                    no_conceito: dom.byId("no_conceito").value,
                    id_conceito_ativo: domAttr.get("id_conceito_ativo", "checked"),
                    pc_inicial_conceito: dom.byId("pc_inicial_conceito").value,
                    pc_final_conceito: dom.byId("pc_final_conceito").value,
                    cd_produto: dijit.byId("cd_produtoC").value,
                    vl_nota_participacao: dojo.number.parse(dojo.byId("vl_nota_participacao").value)
                }];
        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteConceito",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItensConceito");

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("cadConceito").hide();

                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridConceito').itensSelecionados, "cd_conceito", itensSelecionados[r].cd_conceito);

                searchConceito(false);

                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("searchConceito").set('disabled', false);
                dijit.byId("relatorioConceito").set('disabled', false);

                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            if (!hasValue(dojo.byId("cadConceito").style.display))
                apresentaMensagem('apresentadorMensagemConceito', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    });
};
//#endregion

//#region Persistência Feriado
function searchFeriado(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory",
    "dojo/dom-attr"
    ], function (JsonRest, ObjectStore, Cache, Memory, domAttr) {
        try {
            var Ano = dijit.byId("aaferiado").valueNode.value == '' ? 0 : dijit.byId("aaferiado").value;
            var Mes = dijit.byId("mmferiado").valueNode.value == '' ? 0 : dijit.byId("mmferiado").value;
            var Dia = dijit.byId("ddferiado").valueNode.value == '' ? 0 : dijit.byId("ddferiado").value;

            var AnoFim = dijit.byId("aaferiadoFim").valueNode.value == '' ? 0 : dijit.byId("aaferiadoFim").value;
            var MesFim = dijit.byId("mmferiadoFim").valueNode.value == '' ? 0 : dijit.byId("mmferiadoFim").value;
            var DiaFim = dijit.byId("ddferiadoFim").valueNode.value == '' ? 0 : dijit.byId("ddferiadoFim").value;
      
            myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getFeriadoSearch?desc=" + encodeURIComponent(document.getElementById("feriadoDesc").value) + "&inicio=" + document.getElementById("inicioFeriado").checked + "&status=" + retornaStatus("statusFeriado") +
                    "&Ano=" + Ano + "&Mes=" + Mes + "&Dia=" + Dia + "&AnoFim=" + AnoFim + "&MesFim=" + MesFim +
                    "&DiaFim=" + DiaFim + "&somenteAno=" + retornaStatus("somenteAnoPes") +
                    "&idFeriadoAtivo=" + domAttr.get("feriadoAtivoPesquisa", "checked"),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cod_feriado"
		    }), Memory({ idProperty: "cod_feriado" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridFeriado = dijit.byId("gridFeriado");

            if (limparItens)
                gridFeriado.itensSelecionados = [];

            gridFeriado.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};
//Fim da procura Feriado

//** Método de Incluir Feriado
function verificarCamposAnoFeriado() {
    try {
        var anoIni = hasValue(dijit.byId('aa_feriado') != '') ? dijit.byId('aa_feriado').value : 0;
        var anofim = hasValue(dijit.byId('anoFim') != '') ? dijit.byId('anoFim').value : 0;
        if ((anoIni == 0 && anofim != 0)
            || (anoIni != 0 && anofim == 0)) {
            caixaDialogo(DIALOGO_AVISO, 'Todos os campos de ano devem ser prenchidos.', 0, 0, 0);
            return false;
        } else return true;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirFeriado() {
    var consistirAno = verificarCamposAnoFeriado();
    if (consistirAno == false) {
        return false;
    }
    apresentaMensagem('apresentadorMensagemFeriado', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formFeriado").validate())
        return false;

    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        var ano = hasValue(dom.byId("aa_feriado").value) ? parseInt(dom.byId("aa_feriado").value) : 0;
        var ok = VerificaData(parseInt(dom.byId("dd_feriado").value), parseInt(dom.byId("mm_feriado").value), ano);
        var anoFinal = hasValue(dom.byId("anoFim").value) ? parseInt(dom.byId("anoFim").value) : 0;
        var okFinal = VerificaData(parseInt(dom.byId("diaFim").value), parseInt(dom.byId("mesFim").value), ano);
        if (!ok || !okFinal) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorDataInvalida);
            apresentaMensagem("apresentadorMensagemFeriado", mensagensWeb);
            return false;
        }
        showCarregando();
        xhr.post(Endereco() + "/api/coordenacao/postInsertFeriado", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: 'json',
            data: ref.toJson({
                dc_feriado: dom.byId("dc_feriado").value,
                id_feriado_financeiro: domAttr.get("id_feriado_financeiro", "checked"),
                id_feriado_ativo: domAttr.get("id_novo_feriado_ativo", "checked"),
                dd_feriado: dojo.number.parse(dom.byId("dd_feriado").value),
                mm_feriado: dojo.number.parse(dom.byId("mm_feriado").value),
                aa_feriado: dom.byId("aa_feriado").value,
                dd_feriado_fim: dojo.number.parse(dom.byId("diaFim").value),
                mm_feriado_fim: dojo.number.parse(dom.byId("mesFim").value),
                aa_feriado_fim: dojo.number.parse(dom.byId("anoFim").value)
            })
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (data != null && hasValue(data) && !hasValue(data.erro) && data.retorno != null) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridFeriado';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);

                    dijit.byId('cadFeriado').hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    if (itemAlterado != null)
                        insertObjSort(grid.itensSelecionados, "cod_feriado", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoFeriado', 'cod_feriado', 'selecionaTodosFeriados', ['searchFeriado', 'relatorioFeriado'], 'todosItensFeriado');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    if (itemAlterado != null)
                        setGridPagination(grid, itemAlterado, "cod_feriado");
                    dijit.byId('cadFeriado').hide();
                } else
                    apresentaMensagem('apresentadorMensagemFeriado', data);

                hideCarregando();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemFeriado', error);
            hideCarregando();
        });
    });
};
//** fim da inserção de Feriado

//** Método de Editar Feriado
function alterarFeriado() {
    var consistirAno = verificarCamposAnoFeriado();
    if (consistirAno == false) {
        return false;
    }
    var gridName = 'gridFeriado';
    var grid = dijit.byId(gridName);
    if (!dijit.byId("formFeriado").validate())
        return false;

    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        var ano = hasValue(dom.byId("aa_feriado").value) ? parseInt(dom.byId("aa_feriado").value) : 0;
        var ok = VerificaData(parseInt(dom.byId("dd_feriado").value), parseInt(dom.byId("mm_feriado").value), ano);
        var anoFinal = hasValue(dom.byId("anoFim").value) ? parseInt(dom.byId("anoFim").value) : 0;
        var okFinal = VerificaData(parseInt(dom.byId("diaFim").value), parseInt(dom.byId("mesFim").value), ano);
        if (!ok || !okFinal) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorDataInvalida);
            apresentaMensagem("apresentadorMensagemFeriado", mensagensWeb);
            return false;
        }
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditFeriado",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: 'json',
            postData: ref.toJson({
                cod_feriado: dom.byId("cod_feriado").value,
                dc_feriado: dom.byId("dc_feriado").value,
                id_feriado_financeiro: domAttr.get("id_feriado_financeiro", "checked"),
                id_feriado_ativo: domAttr.get("id_novo_feriado_ativo", "checked"),
                dd_feriado: parseInt(dom.byId("dd_feriado").value),
                mm_feriado: parseInt(dom.byId("mm_feriado").value),
                aa_feriado: dom.byId("aa_feriado").value,
                dd_feriado_fim: dojo.number.parse(dom.byId("diaFim").value),
                mm_feriado_fim: dojo.number.parse(dom.byId("mesFim").value),
                aa_feriado_fim: dojo.number.parse(dom.byId("anoFim").value)
            })
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (hasValue(data) && data != null && !hasValue(data.erro) && data.retorno != null) {
                    var itemAlterado = data.retorno;
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadFeriado').hide();
                    removeObjSort(grid.itensSelecionados, "cod_feriado", dom.byId("cod_feriado").value);
                    insertObjSort(grid.itensSelecionados, "cod_feriado", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoFeriado', 'cod_feriado', 'selecionaTodosFeriados', ['searchFeriado', 'relatorioFeriado'], 'todosItensFeriado');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cod_feriado");
                } else
                    apresentaMensagem('apresentadorMensagemFeriado', data);
                hideCarregando();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemFeriado', error);
            hideCarregando();
        });
    });
};
// fim da edição de  Feriado

//** Método para Deletar Feriado
function deletarFeriado(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cod_feriado').value != 0)
                itensSelecionados = [{
                    cod_feriado: dom.byId("cod_feriado").value,
                    dc_feriado: dom.byId("dc_feriado").value,
                    id_feriado_financeiro: domAttr.get("id_feriado_financeiro", "checked"),
                    dd_feriado: dojo.number.parse(dom.byId("dd_feriado").value),
                    mm_feriado: dojo.number.parse(dom.byId("mm_feriado").value),
                    aa_feriado: dojo.number.parse(dom.byId("aa_feriado").value),
                    dd_feriado_fim: dojo.number.parse(dom.byId("diaFim").value),
                    mm_feriado_fim: dojo.number.parse(dom.byId("mesFim").value),
                    aa_feriado_fim: dojo.number.parse(dom.byId("anoFim").value),
                    cd_pessoa_escola: Escola()
                }];
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteFeriado",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItensFeriado");
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadFeriado').hide();
                dijit.byId("feriadoDesc").set("value", '');
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridFeriado').itensSelecionados, "cod_feriado", itensSelecionados[r].cod_feriado);
                searchFeriado(true);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("searchFeriado").set('disabled', false);
                dijit.byId("relatorioFeriado").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                hideCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            if (!hasValue(dojo.byId("cadFeriado").style.display))
                apresentaMensagem('apresentadorMensagemFeriado', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
            hideCarregando();
        });
    });
};
//** fim do delete de Feriado

//#endregion

//#region Persistência de Participação
function searchParticipacao(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
		    JsonRest({
		        target: !hasValue(document.getElementById("no_participacao").value) ? Endereco() + "/api/coordenacao/getParticipacaoSearch?desc=&inicio=" + document.getElementById("cb_inicio").checked + "&status=" + retornaStatus("statusParticipacao") : Endereco() + "/api/coordenacao/getParticipacaoSearch?desc=" + encodeURIComponent(document.getElementById("no_participacao").value) + "&inicio=" + document.getElementById("cb_inicio").checked + "&status=" + retornaStatus("statusParticipacao"),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_participacao"
		    }), Memory({ idProperty: "cd_participacao" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridParticipacao = dijit.byId("gridParticipacao");

            if (limparItens)
                gridParticipacao.itensSelecionados = [];

            gridParticipacao.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//** fim da função searchParticipacao

//#region Persistência de Nivel
function searchNivel(limparItens) {
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Cache, Memory) {
        try {
            var nivel = {};
            
            if (!hasValue(document.getElementById("nivelDesc").value)) {
                nivel = {
                    desc: "",
                    inicio: document.getElementById("cb_inicio_nivel").checked,
                    status: retornaStatus("statusNivel")
                };
            } else {
                nivel = {
                    desc:encodeURIComponent(document.getElementById("nivelDesc").value),
                    inicio:document.getElementById("cb_inicio_nivel").checked,
                    status: retornaStatus("statusNivel"),
                }

            }

            xhr.get({
                url: Endereco() + "/api/coordenacao/getNivelSearch",
                preventCache: true,
                content: nivel,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataNivel) {

                var gridNivel = dijit.byId("gridNivel");

                if (limparItens)
                    gridNivel.itensSelecionados = [];

                gridNivel.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: dataNivel }) }));
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//** fim da função searchNível

//** Método de Incluir participação
function incluirParticipacao() {
    apresentaMensagem('apresentadorMensagemParticipacao', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formParticipacao").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertParticipacao", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                no_participacao: dom.byId("noParticipacao").value,
                id_participacao_ativa: domAttr.get("idParticipacaoAtiva", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridParticipacao';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadParticipacao').hide();

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                insertObjSort(grid.itensSelecionados, "cd_participacao", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoParticipacao', 'cd_participacao', 'selecionaTodosParticipacao', ['pesquisarParticipacao', 'relatorioParticipacao'], 'todosItensParticipacao');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_participacao");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemParticipacao', error);
        });
    });
}


//** Método de Incluir nivel
function incluirNivel() {
    apresentaMensagem('apresentadorMensagemNivel', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formNivel").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertNivel", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                dc_nivel: dom.byId("dc_nivel").value,
                nm_ordem: dom.byId("nm_ordem").value,
                id_ativo: domAttr.get("id_nivel_ativo", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridNivel';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadNivel').hide();

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                insertObjSort(grid.itensSelecionados, "cd_nivel", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoNivel', 'cd_nivel', 'selecionaTodosNiveis', ['pesquisarNivel', 'relatorioNivel'], 'todosItensNivel');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_nivel");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemNivel', error);
        });
    });
}


//** Método de Editar Participação
function alterarParticipacao() {

    var gridName = 'gridParticipacao';
    var grid = dijit.byId(gridName);

    if (!dijit.byId("formParticipacao").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditParticipacao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_participacao: dom.byId("cd_participacao").value,
                no_participacao: dom.byId("noParticipacao").value,
                id_participacao_ativa: domAttr.get("idParticipacaoAtiva", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensParticipacao");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadParticipacao').hide();

                removeObjSort(grid.itensSelecionados, "cd_participacao", dom.byId("cd_participacao").value);
                insertObjSort(grid.itensSelecionados, "cd_participacao", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoParticipacao', 'cd_participacao', 'selecionaTodosParticipacao', ['pesquisarParticipacao', 'relatorioParticipacao'], 'todosItensParticipacao');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_participacao");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemParticipacao', error);
        });
    });
}


//** Método de Editar Nivel
function alterarNivel() {

    var gridName = 'gridNivel';
    var grid = dijit.byId(gridName);

    if (!dijit.byId("formNivel").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditNivel",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_nivel: dom.byId("cd_nivel").value,
                dc_nivel: dom.byId("dc_nivel").value,
                nm_ordem: dom.byId("nm_ordem").value,
                id_ativo: domAttr.get("id_nivel_ativo", "checked")
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensNivel");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadNivel').hide();

                removeObjSort(grid.itensSelecionados, "cd_nivel", dom.byId("cd_nivel").value);
                insertObjSort(grid.itensSelecionados, "cd_nivel", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoNivel', 'cd_nivel', 'selecionaTodosNiveis', ['pesquisarNivel', 'relatorioNivel'], 'todosItensNivel');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_nivel");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemNivel', error);
        });
    });
}


//** Método para Deletar a Participação
function deletarParticipacao(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_participacao').value != 0)
                    itensSelecionados = [{
                        cd_participacao: dom.byId("cd_participacao").value,
                        no_participacao: dom.byId("noParticipacao").value,
                        id_participacao_ativa: domAttr.get("idParticipacaoAtiva", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteParticipacao",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensParticipacao");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadParticipacao').hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridParticipacao').itensSelecionados, "cd_participacao", itensSelecionados[r].cd_participacao);
                    searchParticipacao(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarParticipacao").set('disabled', false);
                    dijit.byId("relatorioParticipacao").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                else
                    apresentaMensagem('apresentadorMensagemParticipacao', data);
            }, function (error) {
                if (!hasValue(dojo.byId("cadParticipacao").style.display))
                    apresentaMensagem('apresentadorMensagemParticipacao', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    })
}
//#endregion


//** Método para Deletar o Nivel
function deletarNivel(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_nivel').value != 0)
                    itensSelecionados = [{
                        cd_nivel: dom.byId("cd_nivel").value,
                        dc_nivel: dom.byId("dc_nivel").value,
                        nm_ordem: dom.byId("nm_ordem").value,
                        id_nivel_ativo: domAttr.get("id_nivel_ativo", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteNivel",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensNivel");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadNivel').hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridNivel').itensSelecionados, "cd_nivel", itensSelecionados[r].cd_nivel);
                    searchNivel(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarNivel").set('disabled', false);
                    dijit.byId("relatorioNivel").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                else
                    apresentaMensagem('apresentadorMensagemNivel', data);
            }, function (error) {
                if (!hasValue(dojo.byId("cadNivel").style.display))
                    apresentaMensagem('apresentadorMensagemNivel', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion


//** Método para Deletar o Nivel
function deletarMensagemAvaliacao(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_mensagem_avalicao').value != 0)
                    itensSelecionados = [{
	                    cd_mensagem_avaliacao: dom.byId("cd_mensagem_avaliacao").value,
	                    tx_mensagem_avaliacao: dom.byId("txObs").value,
	                    id_mensagem_ativa: domAttr.get("id_mensagem_avaliacao_ativa_pers", "checked"),
	                    cd_produto: dijit.byId("pesCadProduto").value,
	                    cd_curso: dijit.byId("cbCursos").value
                    }];
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteMensagemAvaliacao",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensMensagemAvaliacao");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadMensagemAvaliacao').hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridMensagemAvaliacao').itensSelecionados, "cd_mensagem_avaliacao", itensSelecionados[r].cd_mensagem_avaliacao);
                    searchMensagemAvaliacao(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarMensagemAvaliacao").set('disabled', false);
                    dijit.byId("relatorioMensagemAvaliacao").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                else
                    apresentaMensagem('apresentadorMensagemMensagemAvaliacao', data);
            }, function (error) {
                if (!hasValue(dojo.byId("cadMensagemAvaliacao").style.display))
                    apresentaMensagem('apresentadorMensagemMensagemAvaliacao', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region Persistência de Carga Professor
function searchCargaProfessor(limparItens) {
    require([
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var qtdMinutosDuracao = hasValue(dijit.byId("nmCargaHorariaMin").value) ? dijit.byId("nmCargaHorariaMin").value : 0;
            var cdEscola = Escola();
            var myStore = Cache(
		    JsonRest({
		        target: Endereco() + "/api/coordenacao/getCargaProfessorSearch?qtd_minutos_duracao=" + parseInt(qtdMinutosDuracao),
		        handleAs: "json",
		        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		        idProperty: "cd_carga_professor"
		    }), Memory({ idProperty: "cd_carga_professor" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridCargaProfessor = dijit.byId("gridCargaProfessor");
            if (limparItens)
                gridCargaProfessor.itensSelecionados = [];
            gridCargaProfessor.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//** fim da função searchCargaProfessor

//** Método de Incluir Carga Professor
function incluirCargaProfessor() {
    apresentaMensagem('apresentadorMensagemCargaProfessor', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!dijit.byId("formCargaProfessor").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postInsertCargaProfessor", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson({
                cd_carga_professor: dojo.byId("cd_carga_professor").value,
                nm_carga_horaria: dom.byId("nmCargaHoraria").value,
                nm_carga_professor: dom.byId("nmCargaProfessor").value,
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var gridName = 'gridCargaProfessor';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadCargaProfessor').hide();

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                insertObjSort(grid.itensSelecionados, "cd_carga_professor", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoCargaProfessor', 'cd_carga_professor', 'selecionaTodosCargaProfessor', ['pesquisarCargaProfessor', 'relatorioCargaProfessor'], 'todosItensCargaProfessor');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_carga_professor");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemCargaProfessor', error);
        });
    });
}

//** Método de Editar Carga Professor
function alterarCargaProfessor() {
    var gridName = 'gridCargaProfessor';
    var grid = dijit.byId(gridName);

    if (!dijit.byId("formCargaProfessor").validate())
        return false;
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/coordenacao/postEditCargaProfessor",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_carga_professor: dojo.byId("cd_carga_professor").value,
                nm_carga_horaria: dom.byId("nmCargaHoraria").value,
                nm_carga_professor: dom.byId("nmCargaProfessor").value,
            })
        }).then(function (data) {
            try {
                var itemAlterado = jQuery.parseJSON(data).retorno;
                var todos = dojo.byId("todosItensCargaProfessor");

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId('cadCargaProfessor').hide();

                removeObjSort(grid.itensSelecionados, "cd_carga_professor", dom.byId("cd_carga_professor").value);
                insertObjSort(grid.itensSelecionados, "cd_carga_professor", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionadoCargaProfessor', 'cd_carga_professor', 'selecionaTodosCargaProfessor', ['pesquisarCargaProfessor', 'relatorioCargaProfessor'], 'todosItensCargaProfessor');
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_carga_professor");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemCargaProfessor', error);
        });
    });
}

//** Método para Deletar a Carga Professor
function deletarCargaProfessor(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_carga_professor').value != 0)
                    itensSelecionados = [{
                        cd_carga_professor: dojo.byId("cd_carga_professor").value
                    }];
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteCargaProfessor",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensCargaProfessor");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadCargaProfessor').hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridCargaProfessor').itensSelecionados, "cd_carga_professor", itensSelecionados[r].cd_carga_professor);
                    searchCargaProfessor(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarCargaProfessor").set('disabled', false);
                    dijit.byId("relatorioCargaProfessor").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                else
                    apresentaMensagem('apresentadorMensagemCargaProfessor', data);
            }, function (error) {
                if (!hasValue(dojo.byId("cadCargaProfessor").style.display))
                    apresentaMensagem('apresentadorMensagemCargaProfessor', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    })
}
//#endregion

//#region Persistência Estagio
function searchEstagio(limparItens) {
    require([
	"dojo/_base/xhr",
	"dojo/store/JsonRest",
	"dojo/data/ObjectStore",
	"dojo/store/Cache",
	"dojo/store/Memory"
    ], function (xhr, JsonRest, ObjectStore, Cache, Memory) {
        var myStore = Cache(
		JsonRest({
		    target: !hasValue(document.getElementById("estagioDesc").value) ? Endereco() + "/api/coordenacao/getEstagioSearch?desc=&abrev=" + encodeURIComponent(document.getElementById("estagioAbrev").value) + "&inicio=" + document.getElementById("inicioEstagio").checked + "&status=" + retornaStatus("statusEstagio") + "&CodP=" + dijit.byId("codProdutoE").get("value") : Endereco() + "/api/coordenacao/getEstagioSearch?desc=" + encodeURIComponent(document.getElementById("estagioDesc").value) + "&abrev=" + encodeURIComponent(document.getElementById("estagioAbrev").value) + "&inicio=" + document.getElementById("inicioEstagio").checked + "&status=" + retornaStatus("statusEstagio") + "&CodP=" + dijit.byId("codProdutoE").get("value"),
		    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
		}), Memory({}));
        dataStore = new ObjectStore({ objectStore: myStore });
        var gridEstagio = dijit.byId("gridEstagio");
        if (limparItens) {
            gridEstagio.itensSelecionados = [];
        }
        gridEstagio.setStore(dataStore);

        //Recarregar o select do produto pois pode ter havido modificações
        xhr.get({
            url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_ESTAGIO + "&cd_produto=null",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Authorization": Token() }
        }).then(function (dataProd) {
            loadProduto(jQuery.parseJSON(dataProd).retorno, 'codProdutoE');
        });
    });
};
//** fim da função search Estagio

//** Método de Incluir Estagio
function incluirEstagio() {
    try {
        apresentaMensagem('apresentadorMensagemEstagio', null);
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formEstagio").validate())
            return false;
        var cdEstagio = 0;
        if (document.getElementById('ordemOri').value >= document.getElementById('num_ordem_estagio').value) {
            require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                showCarregando();
                xhr.post(Endereco() + "/api/coordenacao/postInsertEstagio", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson({
                        no_estagio: dom.byId("no_estagio").value,
                        no_estagio_abreviado: dom.byId("no_estagio_abreviado").value,
                        id_estagio_ativo: domAttr.get("id_estagio_ativo", "checked"),
                        nm_ordem_estagio: dojo.number.parse(dom.byId("num_ordem_estagio").value),
                        cd_produto: dijit.byId("cd_produtoCE").value
                    })
                }).then(function (data) {  //LBM ATENÇÃO  aqui não vai fechar o form, vai dar refresh na grade Estágio para fechar somente com o botão fechar
                    try {
                        var itemAlterado = jQuery.parseJSON(data).retorno;
                        var gridName = 'gridEstagio';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId('cadEstagio').hide();
                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];
                        //dijit.byId("codProdutoE").set("value", dijit.byId("codProdutoE").set("value", dijit.byId("cd_produtoCE").value).value == "" ? 0 : dijit.byId("cd_produtoCE").value);
                        insertObjSort(grid.itensSelecionados, "cd_estagio", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionadoEstagio', 'cd_estagio', 'selecionaTodosEstagios', ['searchEstagio', 'relatorioEstagio'], 'todosItensEstagio');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_estagio");
                        loadGridEstagioOrdem(dijit.byId("cd_produtoCE").value);
                        cdEstagio = itemAlterado.nm_ordem_estagio;
                        //Recarregar o select do produto pois pode ter havido modificações
                        showCarregando();
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                }, function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemEstagio', error);
                });
            });
            var gridEstagioOrdem = dijit.byId('gridEstagioOrdem');
            for (var i = 0; i <= gridEstagioOrdem._by_idx.length; i++) {
                if (gridEstagioOrdem._by_idx[i].item.cd_estagio + "" == itemAlterado.cd_estagio) {
                    //Seleciona a linha com o item editado por default:
                    gridEstagioOrdem.selection.setSelected(i, true);
                    gridEstagioOrdem.render();
                    //Posiciona o foco do scroll no item editado:
                    gridEstagioOrdem.getRowNode(i).id = '';
                    gridEstagioOrdem.getRowNode(i).id = 'ordem_' + cdEstagio;
                    window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                    window.location.hash = '#' + 'ordem_' + cdEstagio;
                    break;
                }
            }
        }
        else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgOrdemInvalida);
            apresentaMensagem("apresentadorMensagemMat", mensagensWeb);
            document.getElementById('num_ordem_estagio').value = document.getElementById('ordemOri').value;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
function MarcaEstagioIncluido(cdEstagio) {
    try {
        var gridEstagioOrdem = dijit.byId('gridEstagioOrdem');

        for (var i = 0; i <= gridEstagioOrdem._by_idx.length; i++) {
            if (gridEstagioOrdem._by_idx[i].item.cd_estagio + "" == itemAlterado.cd_estagio) {
                //Seleciona a linha com o item editado por default:
                gridEstagioOrdem.selection.setSelected(i, true);
                gridEstagioOrdem.render();
                //Posiciona o foco do scroll no item editado:
                gridEstagioOrdem.getRowNode(i).id = '';
                gridEstagioOrdem.getRowNode(i).id = 'ordem_' + cdEstagio;
                window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                window.location.hash = '#' + 'ordem_' + cdEstagio;
                break;
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//** fim da inserção de Estagio getValorDefault("estagioDesc", "no_estagio");
//** Método para Deletar Estagio
function deletarEstagio(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_estagio').value != 0)
                itensSelecionados = [{
                    cd_estagio: dom.byId("cd_estagio").value,
                    no_estagio: dom.byId("no_estagio").value,
                    no_estagio_abreviado: dom.byId("no_estagio_abreviado").value,
                    id_conceito_ativo: domAttr.get("id_estagio_ativo", "checked"),
                    //nm_ordem_estagio: dojo.number.parse(dom.byId("nm_ordem_estagio").value),
                    cd_produto: dijit.byId("cd_produtoCE").value
                }];
        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteEstagio",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensEstagio");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadEstagio').hide();
                    dijit.byId("codProdutoE").set("value", dijit.byId("codProdutoE").set("value", dijit.byId("cd_produtoCE").value).value == "" ? 0 : dijit.byId("cd_produtoCE").value);
                    dijit.byId("estagioDesc").set("value", '');
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridEstagio').itensSelecionados, "cd_estagio", itensSelecionados[r].cd_estagio);
                    searchEstagio(false);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarEstagio").set('disabled', false);
                    dijit.byId("relatorioEstagio").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                else
                    apresentaMensagem('apresentadorMensagemEsc', data);
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            if (!hasValue(dojo.byId("cadEstagio").style.display))
                apresentaMensagem('apresentadorMensagemEsc', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    });
};
//** fim do delete de Estagio

function salvarEstagio() {
    if (!dijit.byId("formEstagio").validate())
        return false;
    var listaEstagioOrdem = [];
    var estagioOrdem = null;
    var grid = dijit.byId("gridEstagioOrdem");
    nrOrdem = 0;
    for (var i = 0; i < grid._by_idx.length; i++) {
        if (hasValue(grid) && hasValue(grid._by_idx))
            listaEstagioOrdem[i] = grid._by_idx[i].item;
        //if (grid._by_idx[i].item.no_estagio == "-") {
        //    listaEstagioOrdem[i].nm_ordem_estagio = 0;
        //    nrOrdem = 0;
        //}
        if (document.getElementById("no_estagio").value == grid._by_idx[i].item.no_estagio)
            nrOrdem = grid._by_idx[i].item.nm_ordem_estagio;
    }
    showCarregando();
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/coordenacao/postEditEstagio", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            data: ref.toJson({
                estagio: {
                    cd_estagio: dom.byId("cd_estagio").value,
                    no_estagio: dom.byId("no_estagio").value,
                    no_estagio_abreviado: dom.byId("no_estagio_abreviado").value,
                    id_estagio_ativo: domAttr.get("id_estagio_ativo", "checked"),
                    nm_ordem_estagio: nrOrdem,//dom.byId("num_ordem_estagio").value,  //LBM não é para alterar nada aqui
                    cd_produto: dijit.byId("cd_produtoCE").value
                },
                estagioOrdem: listaEstagioOrdem,
                noProduto: dojo.byId("cd_produtoCE").value
            })
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(eval(data));
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var item = [];
                    var gridName = 'gridEstagio';
                    var grid = dijit.byId(gridName);
                    if (hasValue(dijit.byId("gridEstagio").itensSelecionados)) {
                        for (var l = 0; l < dijit.byId("gridEstagio").itensSelecionados.length; l++)
                            if (dijit.byId("gridEstagio").itensSelecionados[l].cd_produto == dijit.byId("cd_produtoCE").value)
                                item[l] = grid.itensSelecionados[l].cd_estagio;
                    }
                    if (item.length > 0)
                        for (var i = 0; i < item.length; i++)
                            removeObjSort(grid.itensSelecionados, "cd_estagio", item[i]);
                    //grid.update();
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadEstagio').hide();
                    var ativo = dom.byId("id_estagio_ativo").checked ? 1 : 2;
                    dijit.byId("statusEstagio").set("value", ativo);

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    removeObjSort(grid.itensSelecionados, "cd_estagio", dom.byId("cd_estagio").value);
                    for (var i = 0; i < itemAlterado.length; i++)
                        insertObjSort(grid.itensSelecionados, "cd_estagio", itemAlterado[i]);

                    buscarItensSelecionados('gridEstagio', 'selecionadoEstagio', 'cd_estagio', 'selecionaTodosEstagio', ['searchEstagio', 'relatorioEstagio'], 'todosItensEstagio');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_estagio");
                    loadFiltroProduto(xhr);
                }
                else
                    apresentaMensagem('apresentadorMensagemEstagio', data);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemEstagio', error);
        });
    });

}
function Estagios(id, ordem, nome, produto, estagio_ativo) {
    try {
        this.ordem = ordem;
        this.id = id;
        this.nome = nome;
        this.produto = produto;
        this.estagio_ativo = estagio_ativo;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getEstagioFromGridRecursivo(array) {
    try {
        var arrayEstagios = new Array();

        for (var i = 0; i < array.length; i++)
            arrayEstagios[i] = new Estagios(array[i].item.cd_estagio, array[i].item.nm_ordem_estagio, array[i].item.no_estagio, array[i].item.no_estagio_abreviado, array[i].item.cd_produto, array[i].item.id_estagio_ativo);

        return arrayEstagios;
    }
    catch (e) {
        postGerarLog(e);
    }
}
function destroyGridEstagioOrdem() {
    try {
        var grid = dijit.byId("gridEstagioOrdem");
        if (hasValue(grid)) {
            grid.destroy();
            $('<div>').attr('id', 'gridEstagioOrdem').appendTo('#gridOrdem');
        }
        montarEstagioOrdem();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadFiltroProduto(xhr) {
    try {
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_ESTAGIO + "&cd_produto=null",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Authorization": Token() }
            }).then(function (dataProd) {
                loadProduto(jQuery.parseJSON(dataProd).retorno, 'codProdutoE');
                dijit.byId("codProdutoE").set("value", 0);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadGridEstagioOrdem(codProduto) {
    require([
		"dojo/_base/xhr",
		"dojo/data/ObjectStore",
		"dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getEstagioOrdem?CodP=" + codProduto + "&cd_estagio=null",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Authorization": Token() }
        }).then(function (dataProd) {
            try {
                //dataProd = jQuery.parseJSON(dataProd);
                var gridEstagioOrdem = dijit.byId("gridEstagioOrdem");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: dataProd.retorno }) });


                if (hasValue(dataStore.objectStore.data[0])) {
                    document.getElementById('ordemOri').value = dataStore.objectStore.data[0].nm_ordem_estagio + 1;
                    document.getElementById('num_ordem_estagio').value = dataStore.objectStore.data[0].nm_ordem_estagio + 1;
                }
                else
                    if (hasValue(document.getElementById('cd_produtoCE').value)) {
                        document.getElementById('ordemOri').value = 1;
                        document.getElementById('num_ordem_estagio').value = 1;
                    }
                for (var i = 0 ; i < dataStore.objectStore.data.length; i++)
                    if (dataStore.objectStore.data[i].cd_estagio == document.getElementById('cd_estagio').value) {
                        document.getElementById('num_ordem_estagio').value = dataStore.objectStore.data[i].nm_ordem_estagio;
                        break;
                    }
                gridEstagioOrdem.setStore(dataStore);
                if (gridEstagioOrdem._by_idx == 0) {
                    document.getElementById('divEstagioCima').style.display = "none";
                    document.getElementById('divEstagioBaixo').style.display = "none";
                    document.getElementById('divEstagioGravar').style.display = "none";
                    document.getElementById('divCancelarEstagio').style.display = "none";


                }
                else {
                    document.getElementById('divEstagioCima').style.display = "";
                    document.getElementById('divEstagioBaixo').style.display = "";
                    document.getElementById('divEstagioGravar').style.display = "";
                    document.getElementById('divCancelarEstagio').style.display = "";

                    for (var i = 0; i <= gridEstagioOrdem._by_idx.length; i++)
                        gridEstagioOrdem.selection.setSelected(i, false);

                    for (var i = 0; i < gridEstagioOrdem._by_idx.length; i++) {
                        if (gridEstagioOrdem._by_idx[i].item.cd_estagio + "" == dojo.byId('cd_estagio').value) {
                            //Seleciona a linha com o item editado por default:
                            gridEstagioOrdem.selection.setSelected(i, true);
                            gridEstagioOrdem.render();
                            //Posiciona o foco do scroll no item editado:
                            gridEstagioOrdem.getRowNode(i).id = '';
                            gridEstagioOrdem.getRowNode(i).id = 'ordem_' + gridEstagioOrdem.cd_estagio;
                            window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                            window.location.hash = '#' + 'ordem_' + gridEstagioOrdem.cd_estagio;
                        }
                        if ((!hasValue(dojo.byId('no_estagio').value))) {
                            //dijit.byId('gridEstagioOrdem').clearSelection() = null;
                            gridEstagioOrdem.getRowNode(i).id = '';
                            //gridEstagioOrdem.getRowNode(i).id = 'ordem_' + 0;
                            window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                            //window.location.hash = '#' + 'ordem_' + 0;
                        }
                    }
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
		function (error) {
		    apresentaMensagem('apresentadorMensagemEstagio', error);
		});
    });
}
function incluiEstagioGrid(myNewItem) {
    try {
        var gridEstagioOrdem = dijit.byId("gridEstagioOrdem");

        if (!dijit.byId("dialogEst").validate())
            return false;
        showCarregando();
        if (hasValue(document.getElementById('cd_produtoCE').value) && hasValue(document.getElementById('no_estagio').value)) {
            // dijit.byId("incluirEstagio").set("disabled", true);
            document.getElementById('divEstagioGravar').style.display = "";
            document.getElementById('divCancelarEstagio').style.display = "";
            //    gridEstagioOrdem.SetRow(document.getElementById('no_estagio').value, document.getElementById('num_ordem_estagio').value);
            //gridEstagioOrdem.addRow(["teste1", "10"]);
            if (!hasValue(myNewItem))
                myNewItem = {
                    Cursos: [],
                    Produto: null,
                    cd_estagio: 0,
                    cd_produto: dijit.byId('cd_produtoCE').value,
                    estagio_ativo: document.getElementById('id_estagio_ativo').checked == true ? "Sim" : "Não",
                    id_estagio_ativo: document.getElementById('id_estagio_ativo').checked,
                    nm_ordem_estagio: parseInt(document.getElementById('num_ordem_estagio').value) > 0 ? parseInt(document.getElementById('num_ordem_estagio').value) - 0.5 : parseInt(document.getElementById('num_ordem_estagio').value),
                    no_estagio: document.getElementById('no_estagio').value,
                    no_estagio_abreviado: document.getElementById('no_estagio_abreviado').value,
                    cor_legenda: dijit.byId("nm_paleta_cor").value
                };

            gridEstagioOrdem.store.newItem(myNewItem);
            gridEstagioOrdem.store.save();

            for (var l = 0; l < gridEstagioOrdem._by_idx.length ; l++)
                gridEstagioOrdem._by_idx[l].nm_ordem_estagio = gridEstagioOrdem._by_idx[l].item.nm_ordem_estagio;
            quickSortObj(gridEstagioOrdem._by_idx, 'nm_ordem_estagio');

            for (var l = gridEstagioOrdem._by_idx.length - 1, j = 1; l >= 0; l--, j++)
                gridEstagioOrdem._by_idx[l].item.nm_ordem_estagio = l + 1;
            //{
            //    if (gridEstagioOrdem._by_idx[l].item.nm_ordem_estagio > 0)
            //        gridEstagioOrdem._by_idx[l].item.nm_ordem_estagio = j;
            //    else
            //        j = j - 1;
            //}
            //gridEstagioOrdem.update();

            for (var i = 0; i <= gridEstagioOrdem._by_idx.length - 1; i++) {
                gridEstagioOrdem.selection.setSelected(i, false);
                if (gridEstagioOrdem._by_idx[i].item.nm_ordem_estagio == myNewItem.nm_ordem_estagio) {
                    //Seleciona a linha com o item editado por default:
                    gridEstagioOrdem.selection.setSelected(i, true);
                    setTimeout('atualiza(' + i + ')', 10);
                    //break;
                }
            }
            gridEstagioOrdem._by_idx.reverse();
            if (gridEstagioOrdem._by_idx == 0) {
                document.getElementById('divEstagioCima').style.display = "none";
                document.getElementById('divEstagioBaixo').style.display = "none";
                document.getElementById('divEstagioGravar').style.display = "none";
                document.getElementById('divCancelarEstagio').style.display = "none";
            }
            else {
                document.getElementById('divEstagioCima').style.display = "";
                document.getElementById('divEstagioBaixo').style.display = "";
                document.getElementById('divEstagioGravar').style.display = "";
                document.getElementById('divCancelarEstagio').style.display = "";
            }
            dijit.byId("dialogEst").hide();
            showCarregando();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
function atualiza(i) {
    try {
        var gridEstagioOrdem = dijit.byId('gridEstagioOrdem');
        gridEstagioOrdem.getRowNode(i).id = '';
        gridEstagioOrdem.getRowNode(i).id = 'ordem_' + gridEstagioOrdem._by_idx[i].item.cd_estagio;
        window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
        window.location.hash = '#ordem_' + gridEstagioOrdem._by_idx[i].item.cd_estagio;// + myNewItem.nm_ordem_estagio;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarEstagioGrid(grid) {
    try {
        if (!dijit.byId("dialogEst").validate())
            return false;
        var existEstagio = 0;
        showCarregando();
        for (var l = 0; l < grid._by_idx.length; l++) {
	        
            if ((grid._by_idx[l].item.nm_ordem_estagio == ordemOri && grid._by_idx[l].item.id_estagio_ativo == true) ||
                (l.toString()  == dojo.byId("index_estagio").value && grid._by_idx[l].item.id_estagio_ativo == false)) {
                // Muda os itens de lugares:
                grid._by_idx[l].item.no_estagio = document.getElementById('no_estagio').value;
                grid._by_idx[l].item.no_estagio_abreviado = document.getElementById('no_estagio_abreviado').value;
                grid._by_idx[l].item.id_estagio_ativo = document.getElementById('id_estagio_ativo').checked;
                grid._by_idx[l].item.estagio_ativo = document.getElementById('id_estagio_ativo').checked == true ? 'Sim' : 'Não';
                grid._by_idx[l].item.cor_legenda = dijit.byId("nm_paleta_cor").value;
                if (grid._by_idx[l].item.nm_ordem_estagio < parseInt(document.getElementById('num_ordem_estagio').value))
                    grid._by_idx[l].item.nm_ordem_estagio = parseInt(document.getElementById('num_ordem_estagio').value) + 0.5;
                else
                    grid._by_idx[l].item.nm_ordem_estagio = parseInt(document.getElementById('num_ordem_estagio').value) - 0.5;
                if (document.getElementById('id_estagio_ativo').checked == false)
                    grid._by_idx[l].item.nm_ordem_estagio = 0;
                else
                    grid._by_idx[l].item.no_estagio = document.getElementById('no_estagio').value;
                //break;
            }
            grid._by_idx[l].nm_ordem_estagio = grid._by_idx[l].item.nm_ordem_estagio;
        }
        quickSortObj(grid._by_idx, 'nm_ordem_estagio');
        grid._by_idx.reverse();
        for (var l = grid._by_idx.length - 1, j = 1; l >= 0; l--, j++) {
            if (grid._by_idx[l].item.nm_ordem_estagio > 0)
                grid._by_idx[l].item.nm_ordem_estagio = j;
            else
                j = j - 1;
        }

        grid.update();
        for (var i = 0; i <= grid._by_idx.length - 1; i++) {
            grid.selection.setSelected(i, false);
            if (grid._by_idx[i].item.cd_estagio == parseInt(document.getElementById('cd_estagio').value)) {
                //Seleciona a linha com o item editado por default:
                grid.selection.setSelected(i, true);
                //grid.render();
                ////Posiciona o foco do scroll no item editado:
                grid.getRowNode(i).id = '';
                grid.getRowNode(i).id = 'ordem_' + parseInt(document.getElementById('cd_estagio').value);
                window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                window.location.hash = '#ordem_' + parseInt(document.getElementById('cd_estagio').value);
                //break;
            }
        }
        dijit.byId("dialogEst").hide();
        showCarregando();
    }
    catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxOrdenacao(value, rowIndex, obj) {
    try {
        var gridName = 'gridEstagioOrdem';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nm_ordem_estagio", grid._by_idx[rowIndex].item.nm_ordem_estagio);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nm_ordem_estagio', 'selecionadoOrdenacao', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'nm_ordem_estagio', 'selecionadoOrdenacao', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarEstagioOrdem() {
    // ** Inicio da criação da grade de Estagio para Ordenar**\\
    require([
		"dojox/grid/EnhancedGrid",
		"dojo/data/ObjectStore",
		"dojo/store/Cache",
		"dojo/store/Memory",
		"dijit/form/Button",
		"dijit/Dialog"
    ], function (EnhancedGrid, ObjectStore, Cache, Memory, Button) {
        var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
        var gridEstagioOrdem = new EnhancedGrid({
            store: dataStore,
            structure: [
             { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoOrdenacao", width: "25px", styles: "text-align: center;", formatter: formatCheckBoxOrdenacao },
			{ name: "Estágio", field: "no_estagio", width: "60%" },
            { name: "Abreviado", field: "no_estagio_abreviado", width: "13%" },
			{ name: "Ordem", field: "nm_ordem_estagio", width: "15%" },
            { name: "Ativo", field: "estagio_ativo", width: "10%", styles: "text-align: center;" }
            ],
            selectionMode: "single",
            canSort: false,
            noDataMessage: msgNotRegEnc// msgNotRegEstag

        }, "gridEstagioOrdem");
        gridEstagioOrdem.canSort = function () { return false };
        gridEstagioOrdem.startup();
        gridEstagioOrdem.on("RowDblClick", function (evt) {
            dijit.byId("dialogEst").show();
            var idx = evt.rowIndex,
               item = this.getItem(idx),
               store = this.store;
            document.getElementById("index_estagio").value = idx,
            document.getElementById("cd_estagio").value = item.cd_estagio,
            document.getElementById("no_estagio").value = item.no_estagio;
            document.getElementById("no_estagio_abreviado").value = item.no_estagio_abreviado;
            dijit.byId("id_estagio_ativo").set("value", item.id_estagio_ativo);
            document.getElementById("num_ordem_estagio").value = item.nm_ordem_estagio;
            dojo.byId("widget_cor_legenda_estagio").style.backgroundColor = item.cor_legenda;
            dijit.byId("nm_paleta_cor").set("value", item.cor_legenda);
            alterarOrd = 1;
            ordemOri = item.nm_ordem_estagio;
            document.getElementById('divCancelarEst').style.display = "";
            document.getElementById('divLimparEst').style.display = "none";
            dojo.byId("incluirEst_label").innerHTML = "Alterar";
        }, true);
        gridEstagioOrdem.rowsPerPage = 5000;

        if (gridEstagioOrdem._by_idx == 0) {
            document.getElementById('divEstagioCima').style.display = "none";
            document.getElementById('divEstagioBaixo').style.display = "none";
            document.getElementById('divEstagioGravar').style.display = "none";
            document.getElementById('divCancelarEstagio').style.display = "none";
        }
        else {
            document.getElementById('divEstagioCima').style.display = "";
            document.getElementById('divEstagioBaixo').style.display = "";
            document.getElementById('divEstagioGravar').style.display = "";
            document.getElementById('divCancelarEstagio').style.display = "";
        }

        if (!hasValue(dijit.byId('subir'))) {
            new Button({
                label: "Subir", iconClass: 'dijitEditorIcon dijitEditorIconTabUp',
                onClick: function () {
                    //var subir = subirOrdemEstagio(gridEstagioOrdem);
                    subirDescerOrdemEst(gridEstagioOrdem, SOBE);
                }
            }, "subir");
            new Button({
                label: "Descer", iconClass: 'dijitEditorIcon dijitEditorIconTabDown',
                onClick: function () {
                    //var descer = descerOrdemEstagio(gridEstagioOrdem);
                    subirDescerOrdemEst(gridEstagioOrdem, DESCE);
                }
            }, "abaixo");
        }
    });
}

function descerOrdemEstagio(grid) {
    try {
        var itemSelecionado = grid.selection.getSelected();

        if (itemSelecionado.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum estágio para alterar a sua ordem.', null);
        else {

            for (var l = 0; l < grid._by_idx.length; l++)
                if (grid._by_idx[l].item.nm_ordem_estagio == itemSelecionado[0].nm_ordem_estagio) {
                    if (hasValue(grid._by_idx[l + 1])) {
                        var itemEncontrado = grid._by_idx[l].item;
                        var ordemEncontrada = grid._by_idx[l].item.nm_ordem_estagio;
                        var posicaoEncontrada = grid.selection.selectedIndex;

                        // Muda as ordens de lugares:
                        grid._by_idx[l].item.nm_ordem_estagio = grid._by_idx[l + 1].item.nm_ordem_estagio;
                        grid._by_idx[l + 1].item.nm_ordem_estagio = ordemEncontrada;

                        // Muda os itens de lugares:
                        grid._by_idx[l].item = grid._by_idx[l + 1].item;
                        grid._by_idx[l + 1].item = itemEncontrado;
                        grid.update();

                        grid.getRowNode(l).id = '';
                        grid.getRowNode(l + 1).id = 'ordem_' + grid._by_idx[l + 1].item.cd_estagio;
                        window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                        window.location.hash = '#' + 'ordem_' + grid._by_idx[l + 1].item.cd_estagio;

                        // Atualiza o item selecionado:
                        grid.selection.setSelected(posicaoEncontrada, false);
                        if (posicaoEncontrada < grid._by_idx.length)
                            grid.selection.setSelected(posicaoEncontrada + 1, true);
                        var codAlteradoDescer = grid._by_idx[l].item.cd_estagio + ";" + grid._by_idx[l + 1].item.cd_estagio + ";";
                    }
                    return codAlteradoDescer;
                    break;
                }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function subirOrdemEstagio(grid) {
    try {
        var itemSelecionado = grid.selection.getSelected();

        if (itemSelecionado.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum estágio para alterar a sua ordem.', null);
        else {

            for (var l = 0; l < grid._by_idx.length; l++)
                if (grid._by_idx[l].item.nm_ordem_estagio == itemSelecionado[0].nm_ordem_estagio) {
                    if (hasValue(grid._by_idx[l - 1])) {
                        var itemEncontrado = grid._by_idx[l].item;
                        var ordemEncontrada = grid._by_idx[l].item.nm_ordem_estagio;
                        var posicaoEncontrada = grid.selection.selectedIndex;

                        // Muda as ordens de lugares:
                        grid._by_idx[l].item.nm_ordem_estagio = grid._by_idx[l - 1].item.nm_ordem_estagio;
                        grid._by_idx[l - 1].item.nm_ordem_estagio = ordemEncontrada;

                        // Muda os itens de lugares:
                        grid._by_idx[l].item = grid._by_idx[l - 1].item;
                        grid._by_idx[l - 1].item = itemEncontrado;

                        grid.update();


                        grid.getRowNode(l).id = '';
                        grid.getRowNode(l - 1).id = 'ordem_' + grid._by_idx[l - 1].item.cd_estagio;
                        window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                        window.location.hash = '#' + 'ordem_' + grid._by_idx[l - 1].item.cd_estagio;

                        // Atualiza o item selecionado:
                        grid.selection.setSelected(posicaoEncontrada, false);
                        if (posicaoEncontrada < grid._by_idx.length)
                            grid.selection.setSelected(posicaoEncontrada - 1, true);
                        var codAlteradoSubir = grid._by_idx[l].item.cd_estagio + ";" + grid._by_idx[l - 1].item.cd_estagio + ";";
                    }
                    return codAlteradoSubir;
                    break;
                }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
function montarEstagiosSelecionados() {
    try {
        var dados = [];
        if (hasValue(dijit.byId("gridEstagioOrdem")) && hasValue(dijit.byId("gridEstagioOrdem").store.objectStore.data)) {
            //   var storeAval = dijit.byId("gridOrdenacao").store.objectStore.data;
            var storeAval = dijit.byId("gridEstagioOrdem").itensSelecionados;
            $.each(storeAval, function (index, val) {
                //if (val.selecionadoOrdenacao == true)
                dados.push(val);
            });
        }
        return dados;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarOrdenacao() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
    function (Memory, ObjectStore) {
        try {
            var aval = montarEstagiosSelecionados();
            var mensagensWeb = new Array();
            if (aval.length > 0) {
                var arrayAval = dijit.byId("gridEstagioOrdem")._by_idx;
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
                dijit.byId("gridEstagioOrdem").setStore(dataStore);
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Estágios excluídos com sucesso.");
                for (var l = dijit.byId("gridEstagioOrdem")._by_idx.length - 1, j = 1; l >= 0; l--, j++)
                    dijit.byId("gridEstagioOrdem")._by_idx[l].item.nm_ordem_estagio = j;
                dijit.byId("gridEstagioOrdem").itensSelecionados = [];
                dijit.byId("gridEstagioOrdem").update();
            } else {
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                //mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgNotSelectReg);
            }
            apresentaMensagem('apresentadorMensagemEstagio', mensagensWeb);
            //quickSortObj(dijit.byId("gridOrdenacao")._by_idx, 'nm_ordem_avaliacao');
            //dijit.byId("gridOrdenacao")._by_idx.reverse();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function subirDescerOrdemEst(grid, sobeDesce) {
    try {
        var operacao = sobeDesce == SOBE ? 1 : -1;

        //var itemSelecionado = grid.selection.getSelected();
        if (grid.itensSelecionados.length > 0 && grid.itensSelecionados[0].id_avaliacao_ativa == false) {
            caixaDialogo(DIALOGO_AVISO, msgSelectRegOrdem, null);
            grid.itensSelecionados = [];
            grid.update();
            return false;
        }

        var itemSelecionado = grid.itensSelecionados;

        if (itemSelecionado.length > 1)
            caixaDialogo(DIALOGO_AVISO, 'Selecione apenas um registro.', null);
        else
            if (hasValue(itemSelecionado) && hasValue(itemSelecionado[0])) {
                if (itemSelecionado[0].id_estagio_ativo == false) {
                    caixaDialogo(DIALOGO_AVISO, 'Não é possível movimentar um item inativo.', null);
                    grid.itensSelecionados = [];
                }
                else {
                    for (var l = 0; l < grid._by_idx.length; l++) {
                        if (hasValue(grid._by_idx[l - (operacao)]) && grid._by_idx[l - (operacao)].item.nm_ordem_estagio == 0)
                            caixaDialogo(DIALOGO_AVISO, 'O item já se encontra primeira possição.', null);
                        else {
                            if (grid._by_idx[l].item.nm_ordem_estagio == itemSelecionado[0].nm_ordem_estagio) {
                                if (hasValue(grid._by_idx[l - (operacao)])) {
                                    var itemEncontrado = grid._by_idx[l].item;
                                    var ordemEncontrada = grid._by_idx[l].item.nm_ordem_estagio;
                                    var posicaoEncontrada = grid.selection.selectedIndex;

                                    // Muda as ordens de lugares:
                                    grid._by_idx[l].item.nm_ordem_estagio = grid._by_idx[l - (operacao)].item.nm_ordem_estagio;
                                    grid._by_idx[l - (operacao)].item.nm_ordem_estagio = ordemEncontrada;

                                    // Muda os itens de lugares:
                                    grid._by_idx[l].item = grid._by_idx[l - (operacao)].item;
                                    grid._by_idx[l - (operacao)].item = itemEncontrado;

                                    grid.update();

                                    grid.getRowNode(l).id = '';
                                    grid.getRowNode(l - (operacao)).id = 'ordem_' + grid._by_idx[l - (operacao)].item.nm_ordem_estagio;
                                    window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                                    window.location.hash = '#' + 'ordem_' + grid._by_idx[l - (operacao)].item.nm_ordem_estagio;

                                    // Atualiza o item selecionado:
                                    grid.selection.setSelected(posicaoEncontrada, false);
                                    if (posicaoEncontrada < grid._by_idx.length)
                                        grid.selection.setSelected(posicaoEncontrada - 1, true);
                                    var codAlteradoSubir = grid._by_idx[l].item.nm_ordem_estagio + ";" + grid._by_idx[l - (operacao)].item.nm_ordem_estagio + ";";
                                }
                                return codAlteradoSubir;
                                break;
                            }
                        }
                    }
                }
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}
// fim dos procedimentos para estagio
//#endregion

//#region  Métodos para link

// Sala
function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formSala');
            apresentaMensagem('apresentadorMensagem', '');

            validaEdicaoNomeSalaOnline(itensSelecionados[0]);

            keepValues(FORM_SALA, null, dijit.byId('gridSala'), true);
            dijit.byId("cadSala").show();
            IncluirAlterar(0, 'divAlterarSala', 'divIncluirSala', 'divExcluirSala', 'apresentadorMensagemSala', 'divCancelarSala', 'divClearSala');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validaEdicaoNomeSalaOnline(sala) {
    if (!eval(MasterGeral()) && hasValue(sala.id_sala_online)) {
        dijit.byId("no_sala").set("disabled", true);
    } else {
        dijit.byId("no_sala").set("disabled", false);
    }
}

// Produto
function eventoEditarProduto(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formProduto');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_PRODUTO, null, dijit.byId('gridProduto'), true);
            dijit.byId("cadProduto").show();
            IncluirAlterar(0, 'divAlterarProduto', 'divIncluirProduto', 'divExcluirProduto', 'apresentadorMensagemProduto', 'divCancelarProduto', 'divClearProduto');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Estagio
function eventoEditarEstagio(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {

            apresentaMensagem('apresentadorMensagem', '');
            dojo.xhr.get({
                url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_CAD_ESTAGIO + "&cd_produto=null",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Authorization": Token() }
            }).then(function (dataProdAtivo) {
                loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, 'cd_produtoCE');
                dijit.byId("cd_produtoCE").set("value", itensSelecionados[0].cd_produto);
            });
            keepValues(FORM_ESTAGIO, null, dijit.byId('gridEstagio'), true);
            dijit.byId("cadEstagio").show();
            dijit.byId("gridEstagioOrdem").itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Duração
function eventoEditarDuracao(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formDuracao');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_DURACAO, null, dijit.byId('gridDuracao'), true);
            dijit.byId("cadDuracao").show();
            IncluirAlterar(0, 'divAlterarDuracao', 'divIncluirDuracao', 'divExcluirDuracao', 'apresentadorMensagemDuracao', 'divCancelarDuracao', 'divClearDuracao');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Modalidade
function eventoEditarModalidade(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formModalidade');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_MODALIDADE, null, dijit.byId('gridModalidade'), true);
            dijit.byId("cadModalidade").show();
            IncluirAlterar(0, 'divAlterarModalidade', 'divIncluirModalidade', 'divExcluirModalidade', 'apresentadorMensagemModalidade', 'divCancelarModalidade', 'divClearModalidade');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Regime
function eventoEditarRegime(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formRegime');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_REGIME, null, dijit.byId('gridRegime'), true);
            dijit.byId("cadRegime").show();
            IncluirAlterar(0, 'divAlterarRegime', 'divIncluirRegime', 'divExcluirRegime', 'apresentadorMensagemRegime', 'divCancelarRegime', 'divClearRegime');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Conceito
function eventoEditarConceito(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formConceito');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_CONCEITO, null, dijit.byId('gridConceito'), true);
            dijit.byId("cadConceito").show();
            IncluirAlterar(0, 'divAlterarConceito', 'divIncluirConceito', 'divExcluirConceito', 'apresentadorMensagemConceito', 'divCancelarConceito', 'divClearConceito');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Feriado
function eventoEditarFeriado(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formFeriado');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_FERIADO, null, dijit.byId('gridFeriado'), true);
            dijit.byId("cadFeriado").show();
            IncluirAlterar(0, 'divAlterarFeriado', 'divIncluirFeriado', 'divExcluirFeriado', 'apresentadorMensagemFeriado', 'divCancelarFeriado', 'divClearFeriado');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Evento
function eventoEditarEvento(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formEvento');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_EVENTO, null, dijit.byId('gridEvento'), true);
            dijit.byId("cadEvento").show();
            IncluirAlterar(0, 'divAlterarEvento', 'divIncluirEvento', 'divExcluirEvento', 'apresentadorMensagemEvento', 'divCancelarEvento', 'divClearEvento');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Motivo Falta
function eventoEditarMotivoFalta(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formMotivoFalta');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_MOTIVOFALTA, null, dijit.byId('gridMotivoFalta'), true);
            dijit.byId("cadMotivoFalta").show();
            IncluirAlterar(0, 'divAlterarMotivoFalta', 'divIncluirMotivoFalta', 'divExcluirMotivoFalta', 'apresentadorMensagemMotivoFalta', 'divCancelarMotivoFalta', 'divClearMotivoFalta');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Motivo desistência
function eventoEditarMotivoDesistencia(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formMotivoDesistencia');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_MOTIVODESISTENCIA, null, dijit.byId('gridMotivoDesistencia'), true);
            dijit.byId("cadMotivoDesistencia").show();
            IncluirAlterar(0, 'divAlterarMotivoDesistencia', 'divIncluirMotivoDesistencia', 'divExcluirMotivoDesistencia', 'apresentadorMensagemMotivoDesistencia', 'divCancelarMotivoDesistencia', 'divClearMotivoDesistencia');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Tipo de Atividade Extra
function eventoEditarAtividadeExtra(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formAtividadeExtra');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_ATIVIDADEEXTRA, null, dijit.byId('gridAtividadeExtra'), true);
            dijit.byId("cadAtividadeExtra").show();
            IncluirAlterar(0, 'divAlterarAtividadeExtra', 'divIncluirAtividadeExtra', 'divExcluirAtividadeExtra', 'apresentadorMensagemAtvidadeExtra', 'divCancelarAtividadeExtra', 'divClearAtividadeExtra');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Participação
function eventoEditarParticipacao(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formParticipacao');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_PARTICIPACAO, null, dijit.byId('gridParticipacao'), true);
            dijit.byId("cadParticipacao").show();
            IncluirAlterar(0, 'divAlterarParticipacao', 'divIncluirParticipacao', 'divExcluirParticipacao', 'apresentadorMensagemParticipacao', 'divCancelarParticipacao', 'divClearParticipacao');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Carga Professor
function eventoEditarCargaProfessor(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formCargaProfessor');
            clearForm("formCargaProfessor");
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(FORM_CARGAPROFESSOR, null, dijit.byId('gridCargaProfessor'), true);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


//Nível
function eventoEditarNivel(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar('#formNivel');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_NIVEL, null, dijit.byId('gridNivel'), true);
            dijit.byId("cadNivel").show();
            IncluirAlterar(0, 'divAlterarNivel', 'divIncluirNivel', 'divExcluirNivel', 'apresentadorMensagemNivel', 'divCancelarNivel', 'divClearNivel');
            
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarMensagemAvaliacao(itensSelecionados, xhr, ref) {
	try {
		if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
			caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
		else if (itensSelecionados.length > 1)
			caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
		else {
			getLimpar('#formMensagemAvaliacaoCad');
			apresentaMensagem('apresentadorMensagem', '');
            var mensagemAvaliacaoPesq = new MensagemAvaliacao(itensSelecionados[0]);

            xhr.post({
	            url: Endereco() + "/api/coordenacao/obterRecursosMensagemAvaliacao",
	            headers: {
		            "Accept": "application/json",
		            "Content-Type": "application/json",
		            "Authorization": Token()
	            },
	            handleAs: "json",
	            postData: ref.toJson(mensagemAvaliacaoPesq)
            }).then(function(dataMensagemAvaliacao) {
	            loadProduto(jQuery.parseJSON(dataMensagemAvaliacao).retorno.listaProdutos, 'pesCadProduto');
	            loadMultiCursoMensagemAvaliacao(jQuery.parseJSON(dataMensagemAvaliacao).retorno.listaCursos,
		            "cbCursos");

	            keepValues(FORM_MENSAGEM_AVALIACAO, null, dijit.byId('gridMensagemAvaliacao'), true);
	            dijit.byId("cadMensagemAvaliacao").show();
	            IncluirAlterar(0,
		            'divAlterarMensagemAvaliacao',
		            'divIncluirMensagemAvaliacao',
		            'divExcluirMensagemAvaliacao',
		            'apresentadorMensagemMensagemAvaliacao',
		            'divCancelarMensagemAvaliacao',
		            'divClearMensagemAvaliacao');
            });

		}
	}
	catch (e) {
		postGerarLog(e);
	}
}

//#endregion
