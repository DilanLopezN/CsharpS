var VISAO_ALUNO = 1;
var VISAO_AVALIACAO = 2;
var CONCEITO = 1;
var NOTA = 2;
var AVALIACAOTURMA = 3;
var dadosGridNota = new Object();
var dadosGridInicial = new Object();
var MOVIDO = 0;
var ATIVO = 1;
var DESISTENTE = 2;
var TRANSFERIDO = 3;
var ENCERRADO = 4;
var DEPENDENTE = 5;
var REPROVADO = 6;
var REMANEJADO = 7;
var REMATRICULADO = 8;
var AGUARDANDO = 9;
var MATRICULAsMATERIAL = 10
var CANCELADO = 11
var PONTOS = 'Pontos';
var MSGALUNO = new Object();
//#region componentes da grid

//#region monta os dropDonws, Métodos auxiliares, retornaIdentidadePai - consistirSituacaoAlunoTurma -  colorirLinha - verificarNotaAlunoMaiorNotaTurma - setIsConceitoNota

function consistirSituacaoAlunoTurma(field, idFilho, id_participacao, id_botao_participacao) {
    try{
        var dtaAvaliacao = new Date();
        var dtaHistorico = new Date();
        //var dtaCadastro = new Date();
        var dtaAuxiliar = null;
        var cdAluno = 0;
        var situacaoAlunoTurma = null
        var gridNota = dijit.byId('gridNota');
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        var historico = dijit.byId('gridNota').historico;

        if (hasValue(gridNota)) {
            if (visao == VISAO_AVALIACAO)
                for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
                    for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                        if(hasValue(gridNota.store._arrayOfTopLevelItems[i].children[j].dta_avaliacao_turma)){
                            dtaAvaliacao = gridNota.store._arrayOfTopLevelItems[i].children[j].dta_avaliacao_turma[0];
                        	dtaAvaliacao = dojo.date.locale.parse(dtaAvaliacao, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
						}
                        for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                            cd_aluno = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_aluno;
                            dtaAuxiliar = null;
                            situacaoAlunoTurma = null;
                            var id = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id;
                            if ((idFilho == id) || (hasValue(id) && id.length > 0 && (parseInt(idFilho) == id[0]))) {
                                if (hasValue(historico) && historico.length > 0) {
                                    for (var l = 0; l < historico.length; l++) {
                                        if (historico[l].cd_aluno == cd_aluno) {
                                                //dtaCadastro = dojo.date.locale.parse(historico[l].dtaCadastro, { formatLength: 'medium', locale: 'pt-br' });
                                                dtaHistorico = dojo.date.locale.parse(historico[l].dtaHistorico, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                                                //dtaCadastro = dtaHistorico; //Como está usando a sequencia eliminando a data de cadastro;
                                                if (dtaAuxiliar == null) {
                                                    //if (dojo.date.compare(dtaAvaliacao, dtaCadastro, "date") == 1)
                                                    //    dtaAuxiliar = dtaCadastro;
                                                    if (((dojo.date.compare(dtaAvaliacao, dtaHistorico, "date") == 1) || (dojo.date.compare(dtaAvaliacao, dtaHistorico, "date") == 0))){
                                                            //&& ((dojo.date.compare(dtaAuxiliar, dtaCadastro, "date") == -1) || (dojo.date.compare(dtaAuxiliar, dtaCadastro, "date") == 0))
                                                            //&& ((dojo.date.compare(dtaAvaliacao, dtaHistorico, "date") == 1) || (dojo.date.compare(dtaAvaliacao, dtaHistorico, "date") == 0))) {
                                                        dtaAuxiliar = dtaHistorico;
                                                        situacaoAlunoTurma = historico[l].id_situacao_historico;
                                                    }
                                                }
                                        }
                                    }
                                }
                                colorirLinha(situacaoAlunoTurma, field, id_participacao, id_botao_participacao);
                            }
                        }
                    }
                }
            if (visao == VISAO_ALUNO)
                for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
                    cd_aluno = gridNota.store._arrayOfTopLevelItems[i].cd_aluno;
                    dtaAuxiliar = null;
                    situacaoAlunoTurma = null;
                    for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                        for (var l = 0; l < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; l++) {
                            if (idFilho == gridNota.store._arrayOfTopLevelItems[i].children[j].children[l].id) {
                                dtaAvaliacao = gridNota.store._arrayOfTopLevelItems[i].children[j].children[l].dta_avaliacao_turma[0];
                                dtaAvaliacao = dojo.date.locale.parse(dtaAvaliacao, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                                if (historico != null && historico.length > 0) {
                                    for (var k = 0; k < historico.length; k++) {
                                        if (historico[k].cd_aluno == cd_aluno) {
                                                //dtaCadastro = dojo.date.locale.parse(historico[k].dtaCadastro, { formatLength: 'medium', locale: 'pt-br' });
                                                dtaHistorico = dojo.date.locale.parse(historico[k].dtaHistorico, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                                                //dtaCadastro = dtaHistorico;
                                                if (dtaAuxiliar == null) {
                                                    //if (dojo.date.compare(dtaAvaliacao, dtaCadastro, "date") == 1)
                                                    //    dtaAuxiliar = dtaCadastro;
                                                    // Se dtaAvaliacao >= datahistorico &&
                                                    if (((dojo.date.compare(dtaAvaliacao, dtaHistorico, "date") == 1) || (dojo.date.compare(dtaAvaliacao, dtaHistorico, "date") == 0))){
                                                            //&& ((dojo.date.compare(dtaAuxiliar, dtaCadastro, "date") == -1) || (dojo.date.compare(dtaAuxiliar, dtaCadastro, "date") == 0))
                                                            //&& ((dojo.date.compare(dtaAvaliacao, dtaHistorico, "date") == 1) || (dojo.date.compare(dtaAvaliacao, dtaHistorico, "date") == 0))) {
                                                        dtaAuxiliar = dtaHistorico;
                                                        situacaoAlunoTurma = historico[k].id_situacao_historico;
                                                    }
                                                }
                                        }
                                    }
                                }
                                colorirLinha(situacaoAlunoTurma, field, id_participacao, id_botao_participacao);
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

function colorirLinha(situacaoAluno, field, id_participacao, id_botao_participacao) {
    try{
        var field = field == "" ? null : field;
        if (hasValue(field) && hasValue(dijit.byId(field)) && hasValue(dijit.byId(field).domNode) && hasValue(dijit.byId(field).domNode.parentNode))
            switch (situacaoAluno) {
                //Matricula
                case (ATIVO):
                    {
                        dijit.byId(field).domNode.parentNode.parentNode.style.background = "";
                        //LBM dijit.byId(field).set('disabled', (dijit.byId('visao').value == VISAO_ALUNO));
                        dijit.byId(field).set('disabled', false);
                        if (hasValue(dijit.byId(id_botao_participacao)))
                            dijit.byId(id_botao_participacao).set('disabled', false);
                        break;
                    }
                    //Movimentação
                case (MOVIDO):
                    {
                        document.getElementById(field).disabled = true;
                        dijit.byId(field).set('disabled', true);
                        if (hasValue(dijit.byId(id_botao_participacao)))
                            dijit.byId(id_botao_participacao).set('disabled', true);
                        dijit.byId(field).domNode.parentNode.parentNode.style.background = '#5897ca';
                        break;
                    }
                    //Desistencia
                case (DESISTENTE):
                    {
                        document.getElementById(field).disabled = true;
                        dijit.byId(field).set('disabled', true);
                        if (hasValue(dijit.byId(id_botao_participacao)))
                            dijit.byId(id_botao_participacao).set('disabled', true);
                        dijit.byId(field).domNode.parentNode.parentNode.style.background = '#d30027';
                        break;
                    }
                    //Transferencia
                case (TRANSFERIDO):
                    {
                        document.getElementById(field).disabled = true;
                        dijit.byId(field).set('disabled', true);
                        if (hasValue(dijit.byId(id_botao_participacao)))
                            dijit.byId(id_botao_participacao).set('disabled', true);
                        dijit.byId(field).domNode.parentNode.parentNode.style.background = '#897756';
                        break;
                    }
                    //Encerramento
                case (ENCERRADO):
                    {
                        document.getElementById(field).disabled = false;
                        dijit.byId(field).set('disabled', false); //(dijit.byId('visao').value == VISAO_ALUNO));
                        if (hasValue(dijit.byId(id_botao_participacao)))
                            dijit.byId(id_botao_participacao).set('disabled', false);
                        dijit.byId(field).domNode.parentNode.parentNode.style.background = '#4dca48';
                        break;
                    }
                //Matriculas/material
                case (MATRICULAsMATERIAL):
                    {
                        document.getElementById(field).disabled = false;
                        dijit.byId(field).set('disabled', true); //(dijit.byId('visao').value == VISAO_ALUNO));
                        if (hasValue(dijit.byId(id_botao_participacao)))
                            dijit.byId(id_botao_participacao).set('disabled', true);
                        dijit.byId(field).domNode.parentNode.parentNode.style.background = '#DBDB70';
                        break;
                    }
                //Cancelado
                case (CANCELADO):
                    {
                        document.getElementById(field).disabled = false;
                        dijit.byId(field).set('disabled', true); //(dijit.byId('visao').value == VISAO_ALUNO));
                        if (hasValue(dijit.byId(id_botao_participacao)))
                            dijit.byId(id_botao_participacao).set('disabled', true);
                        dijit.byId(field).domNode.parentNode.parentNode.style.background = '#DBDB70';
                        break;
                    }

                case (null): {
                    document.getElementById(field).disabled = true;
                    dijit.byId(field).set('disabled', true);
                    if (hasValue(dijit.byId(id_botao_participacao)))
                        dijit.byId(id_botao_participacao).set('disabled', true);
                    dijit.byId(field).domNode.parentNode.parentNode.style.background = '#ffe772';
                    break;
                }
                default: {
                    dijit.byId(field).domNode.parentNode.parentNode.style.background = "";
                    document.getElementById(field).disabled = (dijit.byId('visao').value == VISAO_ALUNO);
                    dijit.byId(field).set('disabled', (dijit.byId('visao').value == VISAO_ALUNO));
                    if (hasValue(dijit.byId(id_botao_participacao)))
                        dijit.byId(id_botao_participacao).set('disabled', false);
                    break;
                }
            }
        if (id_participacao && hasValue(field))
            dijit.byId(field).set('disabled', true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificarNotaAlunoMaiorNotaTurma() {
    try{
        var gridNota = dijit.byId("gridNota");
        var notaMaximaTurma = 0.0;
        var nota = 0.0;
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        if (visao == VISAO_AVALIACAO)
            for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
                for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                    notaMaximaTurma = gridNota.store._arrayOfTopLevelItems[i].children[j].maximoNotaTurma[0];
                    for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                        var peso = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].peso[0];
                        nota = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0];
                        if (nota * peso > notaMaximaTurma) {
                            var mensagem_erro = msgNotaMaiorNotaTurma + ' ' + parseFloat(notaMaximaTurma).toFixed(1) + '.';

                            if (peso > 1)
                                mensagem_erro += msgPesoUtilizadoCalculo + peso;
                            caixaDialogo(DIALOGO_ERRO, mensagem_erro, null);
                            return false;
                        }
                    }//for j
                }//for r
            }//for i
        if (visao == VISAO_ALUNO)
            for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
                for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                    for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                        var peso = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].peso[0];
                        nota = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0];
                        notaMaximaTurma = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].maximoNotaTurma[0];
                        if (nota * peso > notaMaximaTurma) {
                            var mensagem_erro = msgNotaMaiorNotaTurma + ' ' + parseFloat(notaMaximaTurma).toFixed(1) + '.';

                            if (peso > 1)
                                mensagem_erro += msgPesoUtilizadoCalculo + peso;
                            caixaDialogo(DIALOGO_ERRO, mensagem_erro, null);
                            return false;
                        }
                    }
                }
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificaExistNotaSemAvaliador() {
    try{
        var gridNota = hasValue(dijit.byId('gridNota')) ? dijit.byId('gridNota').store : [];
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').get('value') : 0;
        var hasAvaliador = false;
        var nota = null;
        var conceito = 0;
        var retorno = true;
        if (parseInt(visao) == VISAO_AVALIACAO) {
            for (var i = 0; i < gridNota._arrayOfTopLevelItems.length; i++) {
                for (var j = 0; j < gridNota._arrayOfTopLevelItems[i].children.length; j++) {
                    hasAvaliador = gridNota._arrayOfTopLevelItems[i].children[j].cd_funcionario > 0;
                    for (var k = 0; k < gridNota._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                        nota = gridNota._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0];
                        conceito = gridNota._arrayOfTopLevelItems[i].children[j].children[k].cd_conceito;
                        if (!hasAvaliador && (nota > 0 || conceito > 0)) {
                            nom_aluno = gridNota._arrayOfTopLevelItems[i].children[j].children[k].dc_nome;
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgAvaliadorObrigatorio + " - " + nom_aluno);
                            apresentaMensagem("apresentadorMensagemAvaliacaoTurma", mensagensWeb);
                            retorno = false;
                            break
                        }
                    }
                }
            }
        } else {
            for (var i = 0; i < gridNota._arrayOfTopLevelItems.length; i++) {
                for (var j = 0; j < gridNota._arrayOfTopLevelItems[i].children.length; j++) {
                    for (var k = 0; k < gridNota._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                        hasAvaliador = gridNota._arrayOfTopLevelItems[i].children[j].children[k].cd_funcionario > 0;
                        nota = gridNota._arrayOfTopLevelItems[i].children[j].children[k].vl_nota;
                        conceito = gridNota._arrayOfTopLevelItems[i].children[j].children[k].cd_conceito;
                        if (!hasAvaliador && (nota > 0 || conceito > 0)) {
                            nom_aluno = gridNota._arrayOfTopLevelItems[i].dc_nome;
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgAvaliadorObrigatorio + " - " + nom_aluno);
                            apresentaMensagem("apresentadorMensagemAvaliacaoTurma", mensagensWeb);
                            retorno = false;
                            break
                        }
                    }
                }
            }
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornaIdentidadePai(dataGridStore, idFilho) {
    try{
        var identidadePai = 0;
        for (var i = 0; i < dataGridStore.store._arrayOfTopLevelItems.length; i++) {
            for (var j = 0; j < dataGridStore.store._arrayOfTopLevelItems[i].children.length; j++) {
                for (var k = 0; k < dataGridStore.store._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                    //necessário para pegar a indetindade do pai nessa visão
                    if (idFilho == dataGridStore.store._arrayOfTopLevelItems[i].children[j].children[k].id)
                        for (propriedade in dataGridStore.store._arrayOfTopLevelItems[i].children[j].children[k]._RRM) {
                            identidadePai += propriedade + ": " + dataGridStore.store._arrayOfTopLevelItems[i].children[j].children[k]._RRM + "\n";
                            break;
                        }
                }
            }
        }
        return parseInt(identidadePai);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornaIdentidadeAvo(dataGridStore, idFilho) {
    var identidadeAvo = "";
    for (var i = 0; i < dataGridStore.store._arrayOfTopLevelItems.length; i++) {
        for (var j = 0; j < dataGridStore.store._arrayOfTopLevelItems[i].children.length; j++) {
            for (var k = 0; k < dataGridStore.store._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                //necessário para pegar a indentidade do avô nessa visão
                if (idFilho == dataGridStore.store._arrayOfTopLevelItems[i].children[j].children[k].id)
                    return dataGridStore.store._arrayOfTopLevelItems[i].id[0];
            }
        }
    }
    return identidadeAvo;
}

function setIsConceitoNota(isConceitoNota) {
    try{
        if (isConceitoNota == true) {
            document.getElementById("tdTipo").style.visibility = "";
            document.getElementById("tdFilteringTipo").style.visibility = "";
        } else {
            document.getElementById("tdTipo").style.visibility = "hidden";
            document.getElementById("tdFilteringTipo").style.visibility = "hidden";
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region montarLegenda

function montarLegenda() {
    dojo.ready(function () {
        try{
            var chart = new dojox.charting.Chart("chart");
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, "legend");

            chart.addSeries("Matriculado", [1], { fill: "#FFF" }); //NormalRow
            chart.addSeries("Desistente", [1], { fill: "#d30027" }); //RedRow
            chart.addSeries("Transferido", [1], { fill: "#897756" }); //BigRedRow
            chart.addSeries("Movido", [1], { fill: "#5897ca" }); //BlueRow
            chart.addSeries("Encerrado", [1], { fill: "#4dca48" }); //GreenRow
            chart.addSeries("Matriculado s/Material", [1], { fill: "#DBDB70" }); //YellowRow
            chart.addSeries("Sem data/Matriculado após Avaliação", [1], { fill: "#ffe772" }); //YellowRow
            
            chart.render();
            dijit.byId("legend").refresh();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//#endregion

//#region criação dos metodos para montar o checkBox da grade avaliação turma e componentes da grade nota

function formatCheckBoxAvaliacaoTurma(value, rowIndex, obj) {
    try {
        var gridName = 'gridAvaliacaoTurma';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAvaliacaoTurma');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_turma", grid._by_idx[rowIndex].item.cd_turma);

            value = value || indice !== null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_turma', 'selecionadoAvaliacaoTurma', -1, 'selecionaTodosAvaliacaoTurma', 'selecionaTodosAvaliacaoTurma', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_turma', 'selecionadoAvaliacaoTurma', " + rowIndex + ", '" + id + "', 'selecionaTodosAvaliacaoTurma', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMensagemAluno(value, rowIndex, obj) {
    try {
        var gridName = 'gridMensagemAluno';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMensagemAluno');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_mensagem_avaliacao", grid._by_idx[rowIndex].item.cd_mensagem_avaliacao);

            value = value || indice !== null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_mensagem_avaliacao', 'selecionadoMensagemAluno', -1, 'selecionaTodosMensagemAluno', 'selecionaTodosMensagemAluno', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_mensagem_avaliacao', 'selecionadoMensagemAluno', " + rowIndex + ", '" + id + "', 'selecionaTodosMensagemAluno', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region Monta o input da grid de nota
function formatRadioButtonParticipacao(value, rowIndex, obj) {
    try {
        var gridName = 'gridAvaliacaoParticipacao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_i' + rowIndex + '_c' + obj.cd_conceito;
        var name = obj.field + '_i' + rowIndex;

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (grid._by_idx[rowIndex].item.cd_conceito_participacao != obj.cd_conceito)
            value = false;

        setTimeout("configuraRadioButtonParticipacao(" + value + ", " + rowIndex + ", '" + id + "', '" + name + "', '" + gridName + "',"+obj.cd_conceito+","+obj.vl_nota_participacao+")", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraRadioButtonParticipacao(value, rowIndex, id, name, gridName, cd_conceito, vl_nota_participacao) {
    require(["dojo/ready", "dijit/form/RadioButton"], function (ready, RadioButton) {
        ready(function () {
            var dojoId = dojo.byId(id);
            var grid = dijit.byId(gridName);

            if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
                dijit.byId(id).destroy();
            if (value == undefined)
                value = false;
            if (hasValue(dojoId) && dojoId.type == 'text')
                var checkBox = new RadioButton({
                    name: name,
                    checked: value,
                    onChange: function (marcou) { if(marcou) marcaParticipacaoAluno(value, rowIndex, id, name, gridName, cd_conceito, vl_nota_participacao); }
                }, id);
        })
    });
}

function marcaParticipacaoAluno(value, rowIndex, id, name, gridName, cd_conceito, vl_nota_participacao) {
    var grid = dijit.byId(gridName);
    var item = grid._by_idx[rowIndex].item;

    for (var i = 0; i < grid.store.objectStore.data.length; i++)
        if (grid.store.objectStore.data[i].cd_participacao == item.cd_participacao) {
            grid.store.objectStore.data[i].participacao_selecionada = true;
            grid.store.objectStore.data[i].cd_conceito_participacao = cd_conceito;
            grid.store.objectStore.data[i].vl_nota_participacao = vl_nota_participacao;
        }
}

function alteraNotasParticipacoes() {
    var gridNota = dijit.byId("gridNota");
    var gridAvaliacaoParticipacao = dijit.byId('gridAvaliacaoParticipacao');
    var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
    var idNota = dojo.byId('idNota').value;
    var participacoesAluno = new Array();
    var vl_nota_participacao = 0;

    //LBM if (visao == VISAO_AVALIACAO)
        for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) //1° nível da visão de avaliação
            for (var p = 0; p < gridNota.store._arrayOfTopLevelItems[i].children.length; p++) //2° nível da visão de avaliação
                for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children[p].children.length; j++) {
                    if (gridNota.store._arrayOfTopLevelItems[i].children[p].children[j].id[0] == parseInt(idNota)) {
                        //Atualiza a lista das participações dos alunos:
                        for (var k = 0; k < gridAvaliacaoParticipacao.store.objectStore.data.length; k++) {
                            if (gridAvaliacaoParticipacao.store.objectStore.data[k].participacao_selecionada) {
                                var novoItem = {
                                    cd_participacao_avaliacao: gridAvaliacaoParticipacao.store.objectStore.data[k].cd_participacao,
                                    cd_avaliacao_aluno: gridNota.store._arrayOfTopLevelItems[i].children[p].children[j].cd_avaliacao_aluno[0],
                                    cd_conceito_participacao: gridAvaliacaoParticipacao.store.objectStore.data[k].cd_conceito_participacao,
                                    vl_nota_participacao: gridAvaliacaoParticipacao.store.objectStore.data[k].vl_nota_participacao
                                };
                                vl_nota_participacao += gridAvaliacaoParticipacao.store.objectStore.data[k].vl_nota_participacao;
                            }
                            else {
                                var novoItem = {
                                    cd_participacao_avaliacao: gridAvaliacaoParticipacao.store.objectStore.data[k].cd_participacao,
                                    cd_avaliacao_aluno: gridNota.store._arrayOfTopLevelItems[i].children[p].children[j].cd_avaliacao_aluno[0],
                                    cd_conceito_participacao: null,
                                    vl_nota_participacao: gridAvaliacaoParticipacao.store.objectStore.data[k].vl_nota_participacao
                                };
                            }
                            participacoesAluno.push(novoItem);

                            //Atualiza as participações disponíveis:
                            var participacoesDisponiveis = jQuery.parseJSON(gridNota.store._arrayOfTopLevelItems[i].children[p].children[j].participacoesDisponiveis[0]);
                            for (var l = 0; l < participacoesDisponiveis.length; l++) {
                                if (participacoesDisponiveis[l].cd_participacao == gridAvaliacaoParticipacao.store.objectStore.data[k].cd_participacao)
                                    if (gridAvaliacaoParticipacao.store.objectStore.data[k].participacao_selecionada) {
                                        participacoesDisponiveis[l].participacao_selecionada = true;
                                        participacoesDisponiveis[l].cd_conceito_participacao = gridAvaliacaoParticipacao.store.objectStore.data[k].cd_conceito_participacao;
                                        participacoesDisponiveis[l].vl_nota_participacao = gridAvaliacaoParticipacao.store.objectStore.data[k].vl_nota_participacao;
                                    }
                                    else {
                                        participacoesDisponiveis[l].participacao_selecionada = false;
                                        participacoesDisponiveis[l].cd_conceito_participacao = null;
                                        participacoesDisponiveis[l].vl_nota_participacao = null;
                                    }
                            }

                            gridNota.store._arrayOfTopLevelItems[i].children[p].children[j].participacoesDisponiveis[0] = JSON.stringify(participacoesDisponiveis);
                            gridNota.store._arrayOfTopLevelItems[i].children[p].children[j].isModifiedA[0] = true;
                        }

                        gridNota.store._arrayOfTopLevelItems[i].children[p].children[j].participacoesAluno[0] = JSON.stringify(participacoesAluno);
                        gridNota.store.save();
                        
                        //Atualiza o icone do botão:
                        var botaoIMG = dojo.byId('botaoClick').value;
                        if (participacoesAluno.length > 0)
                            dijit.byId(botaoIMG).set('iconClass', 'dijitEditorIcon dijitEditorIconNewSGF');
                        else
                            dijit.byId(botaoIMG).set('iconClass', 'dijitEditorIcon dijitEditorIconNewPage');

                        //Atualiza a nota da avaliação:
                        dijit.byId(dojo.byId('notaField').value).set('disabled', false);
                        dojo.byId(dojo.byId('notaField').value).value = vl_nota_participacao == 0 ? null : maskFixed(vl_nota_participacao, 2);
                        //LBM if (visao == VISAO_AVALIACAO)
                            dijit.byId(dojo.byId('notaField').value)._onBlur();
                        dijit.byId(dojo.byId('notaField').value).set('disabled', true);
                        break;
                    }
                }

    dijit.byId('modalParticipacao').hide();
}

function incluirMensagemAluno()
{
    dojo.byId('cd_produto_FK').value = dojo.byId('cd_produto_CAD').value;
    dojo.byId('cd_curso_FK').value = dojo.byId('cd_curso_CAD').value;
    dojo.byId('produtoFK').value = dojo.byId('produtoCAD').value;
    dojo.byId('cursoFK').value = dojo.byId('cursoCAD').value;

    pesquisaMensagem.show();
}

function retornarMensagemAluno()
{
    var valido = true;
    var gridMensagemAluno = dijit.byId("gridMensagemAluno");
    if (!hasValue(gridMensagemAluno.itensSelecionados) || gridMensagemAluno.itensSelecionados.length <= 0 || gridMensagemAluno.itensSelecionados.length > 1) {
        if (gridMensagemAluno.itensSelecionados != null && gridMensagemAluno.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        if (gridMensagemAluno.itensSelecionados != null && gridMensagemAluno.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
        valido = false;
    }
    else {
        dijit.byId("textAreaObsAluno").set('value', gridMensagemAluno.itensSelecionados[0].tx_mensagem_avaliacao);
        dojo.byId('cd_mensagem_avaliacao').value = gridMensagemAluno.itensSelecionados[0].cd_mensagem_avaliacao;
        dijit.byId('btIncluirMsgAluno').set('disabled', true);
    }

    if (!valido) return false;
    gridMensagemAluno.itensSelecionados = [];
    pesquisaMensagem.hide();
}

function limparMensagemAluno()
{
    dijit.byId("textAreaObsAluno").set('value', "");
    dijit.byId('btIncluirMsgAluno').set('disabled', false);
    var itensSelecionados = [];
    if (MSGALUNO.length > 0) {
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            try {
                itensSelecionados = [{
                    cd_mensagem_avaliacao_aluno: MSGALUNO[0].cd_mensagem_avaliacao_aluno,
                    tx_mensagem_avaliacao_aluno: MSGALUNO[0].tx_mensagem_avaliacao_aluno,
                    id_mensagem_aluno_ativa: MSGALUNO[0].id_mensagem_ativa,
                    cd_aluno: MSGALUNO[0].cd_aluno,
                    cd_tipo_avaliacao: MSGALUNO[0].cd_tipo_avaliacao,
                    cd_mensagem_avaliacao: MSGALUNO[0].cd_mensagem_avaliacao
                        }];
                xhr.post({
                    url: Endereco() + "/api/coordenacao/postDeleteMensagemAvaliacaoAluno",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    postData: ref.toJson(itensSelecionados)
                }).then(function (data) {
                    dijit.byId('btLimparMsgAluno').set('label', 'Limpar');
                    apresentaMensagem('apresentadorMensagemAvaliacaoTurma', data);
                    dijit.byId('cadMensagemAluno').hide();
                }, function (error) {
                    if (!hasValue(dojo.byId("cadMensagemAvaliacao").style.display))
                        apresentaMensagem('apresentadorMensagemAluno', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    };
}

function cancelarMensagemAluno()
{
    if (MSGALUNO.length == 1) {
        dijit.byId('textAreaObsAluno').set('value', MSGALUNO[0].tx_mensagem_avaliacao_aluno);
        dijit.byId('ckMsgAlunoAtivo').set('checked', MSGALUNO[0].id_mensagem_ativa);
        dojo.byId('cd_mensagem_avaliacao').value = MSGALUNO[0].cd_mensagem_avaliacao;
        dijit.byId('btLimparMsgAluno').set('label', 'Excluir');
        dijit.byId('btAlterarMsgAluno').set('label', 'Salvar');
    }
    else {
        dijit.byId('btLimparMsgAluno').set('label', 'Limpar');
        dijit.byId('btAlterarMsgAluno').set('label', 'Salvar');
    }
    dijit.byId('btIncluirMsgAluno').set('disabled', MSGALUNO.length == 1);
}

function retornaMsgAluno(cdTipoAvaliacao, cdAluno, cdProduto, cdCurso, pFuncao)
{
    try
    {
        require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post(Endereco() + "/api/coordenacao/findMsgAlunobyTipo?cdTipoAvaliacao=" + cdTipoAvaliacao + "&cdAluno=" + cdAluno + "&cdProduto=" + cdProduto + "&cdCurso=" + cdCurso,
            {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson()
            }).then(function (returnDataMsgAluno) {
                try {
                    MSGALUNO = returnDataMsgAluno;
                    if (MSGALUNO.length == 0) {
                        MSGALUNO.cd_aluno = cdAluno;
                        MSGALUNO.cd_tipo_avaliacao = cdTipoAvaliacao;
                    }
                        if (hasValue(pFuncao))
                            pFuncao.call();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    apresentaMensagem('apresentadorMensagemAvaliacaoTurma', error.response.data);
                });
        });
    }
    catch (e) {
    postGerarLog(e);
    }
}

function alterarMensagemAluno()
{
    apresentaMensagem('apresentadorMensagemAluno', null);
    apresentaMensagem('apresentadorMensagem', null);
    if (!hasValue(dijit.byId("textAreaObsAluno").value)) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "É obrigatório preencher o campo mensagem para salvar o registro.");
        apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
        return false;
    }
    if (!hasValue(dojo.byId('cd_mensagem_avaliacao').value) || dojo.byId('cd_mensagem_avaliacao').value == 0) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "É obrigatório INCLUIR uma mensagem padrão para salvar o registro.");
        apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
        return false;
    }
    if (MSGALUNO.length == 0) {
        var cdAluno = MSGALUNO.cd_aluno;
        var cdTipoAvaliacao = MSGALUNO.cd_tipo_avaliacao;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/coordenacao/postIncluirMensagemAvaliacaoAluno", {
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": Token()
                },
                handleAs: "json",
                data: ref.toJson({
                    cd_tipo_avaliacao: cdTipoAvaliacao,
                    cd_aluno: cdAluno,
                    cd_mensagem_avaliacao_aluno: 0,
                    tx_mensagem_avaliacao_aluno: dijit.byId('textAreaObsAluno').value,
                    id_mensagem_ativa: dijit.byId('ckMsgAlunoAtivo').checked,
                    cd_mensagem_avaliacao: dojo.byId('cd_mensagem_avaliacao').value
                })
            }).then(function (data) {
                try {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemAvaliacaoTurma', data);
                    dojo.byId('cd_mensagem_avaliacao').value = 0;
                    dijit.byId('cadMensagemAluno').hide();
                }
                catch (er) {
                    showCarregando();
                    postGerarLog(er);
                }
            }, function (error) {
                showCarregando();
                    apresentaMensagem('apresentadorMensagemAluno', error);
            });
        });
    }
    else {
        showCarregando();
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            xhr.post({
                url: Endereco() + "/api/coordenacao/postEditMensagemAvaliacaoAluno",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_tipo_avaliacao: MSGALUNO[0].cd_tipo_avaliacao,
                    cd_aluno: MSGALUNO[0].cd_aluno,
                    cd_mensagem_avaliacao_aluno: MSGALUNO[0].cd_mensagem_avaliacao_aluno,
                    tx_mensagem_avaliacao_aluno: dijit.byId('textAreaObsAluno').value,
                    id_mensagem_aluno_ativa: dijit.byId('ckMsgAlunoAtivo').checked,
                    cd_mensagem_avaliacao: MSGALUNO[0].cd_mensagem_avaliacao
                })
            }).then(function (data) {
                try {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemAvaliacaoTurma', data);
                    dojo.byId('cd_mensagem_avaliacao').value = 0;
                    dijit.byId('cadMensagemAluno').hide();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                    apresentaMensagem('apresentadorMensagemAluno', error);
            });
        });
    }
}

function formatInputNotaAvaliacaoTurma(value, rowIndex, obj, k) {
    try {
        var gridNota = dijit.byId("gridNota");
        var icon;
        var id = k.field + '_input_' + gridNota._by_idx[rowIndex].item.id;
        var id_botao_participacao = k.field + '_BTN_PART_' + gridNota._by_idx[rowIndex].item._0;
        var isInFocus = dojo.byId('isInFocus').value == 'true';
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();
        if (rowIndex != -1) icon = "<input id='" + id + "' style='height:23px' />";
        var mediaNota = 0;
        var maiorNota = 0;
        var somaNotas = 0;
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
       
        if (gridNota._by_idx[rowIndex].item.cd_tipo_avaliacao[0] == 0)  {
            if ((hasValue(gridNota._by_idx[rowIndex].item.isChildren) && gridNota._by_idx[rowIndex].item.isChildren[0] >= 0) &&
                !(hasValue(gridNota._by_idx[rowIndex].item.cd_aluno) && gridNota._by_idx[rowIndex].item.cd_aluno != 0 && visao == VISAO_ALUNO)) {
                if (!isInFocus) {
                    maiorNota = gridNota._by_idx[rowIndex].item.notaMaxima[0] == null ? 0 : parseFloat(gridNota._by_idx[rowIndex].item.notaMaxima[0]).toFixed(1);
                    mediaNota = gridNota._by_idx[rowIndex].item.mediaNotas[0] == null ? 0 : parseFloat(gridNota._by_idx[rowIndex].item.mediaNotas[0]).toFixed(1);
                    somaNotas = gridNota._by_idx[rowIndex].item.somaNotas[0] == null ? 0 : parseFloat(gridNota._by_idx[rowIndex].item.somaNotas[0]).toFixed(1);
                }
                else {
                    maiorNota = gridNota._by_idx[rowIndex].item.notaMaxima[0] == null ? 0 : parseFloat(gridNota._by_idx[rowIndex].item.notaMaxima[0]).toFixed(0);
                    mediaNota = gridNota._by_idx[rowIndex].item.mediaNotas[0] == null ? 0 : parseFloat(gridNota._by_idx[rowIndex].item.mediaNotas[0]).toFixed(0);
                    somaNotas = gridNota._by_idx[rowIndex].item.somaNotas[0] == null ? 0 : parseFloat(gridNota._by_idx[rowIndex].item.somaNotas[0]).toFixed(0);
                }
                var idty = k.index;  //gridNota._by_idx[rowIndex].idty;
                var totalNotas = gridNota._by_idx[rowIndex].item.vl_nota[0];
                var maximoNotaTurma = gridNota._by_idx[rowIndex].item.maximoNotaTurma[0];
                var filho = gridNota._by_idx[rowIndex].item.isChildren;
                var id_participacao = (hasValue(gridNota._by_idx[rowIndex].item.id_participacao) && (!hasValue(gridNota._by_idx[rowIndex].item.children) || gridNota._by_idx[rowIndex].item.children.length == 0)) ? gridNota._by_idx[rowIndex].item.id_participacao[0] : false; //LBM
                if (id_participacao) {
                    if (hasValue(dijit.byId(id_botao_participacao)))
                        dijit.byId(id_botao_participacao).destroy();
                    icon += "<input id='" + id_botao_participacao + "' style='height:21px' />";
                    setTimeout(function (){
                        configuraButtonParticipacao(hasValue(value), id_botao_participacao, rowIndex, id);
                    }, 1);
                }

                if (visao == VISAO_AVALIACAO){
                    if (gridNota._by_idx[rowIndex].item.pai[0] == 1) 
                        setTimeout("configuraTextBoxNota(" + value + ", '" + id + "'," + true + "," + gridNota._by_idx[rowIndex].item.id + ",'" + gridNota._by_idx[rowIndex].item.idPai + "'," + totalNotas + "," + maximoNotaTurma + "," + maiorNota + "," + mediaNota + "," + filho + "," + idty + "," + somaNotas + "," + id_participacao + ",'" + id_botao_participacao + "')", 1);
                    else
                        setTimeout("configuraTextBoxNota(" + value + ", '" + id + "'," + false + "," + gridNota._by_idx[rowIndex].item.id + ",'" + gridNota._by_idx[rowIndex].item.idPai + "'," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + filho + "," + idty + "," + somaNotas + "," + id_participacao + ",'" + id_botao_participacao + "')", 1);
				}
                if (visao == VISAO_ALUNO){
                    if (gridNota._by_idx[rowIndex].item.pai[0] == 1 && gridNota._by_idx[rowIndex].item.isChildren[0] == 0) 
                        setTimeout("configuraTextBoxNota(" + value + ", '" + id + "'," + true + "," + gridNota._by_idx[rowIndex].item.id + ",'" + gridNota._by_idx[rowIndex].item.idPai + "'," + totalNotas + "," + maximoNotaTurma + "," + maiorNota + "," + somaNotas + "," + filho + "," + idty + "," + somaNotas + "," + id_participacao + ",'" + id_botao_participacao + "')", 1);
                    else 
                        setTimeout("configuraTextBoxNota(" + value + ", '" + id + "'," + false + "," + gridNota._by_idx[rowIndex].item.id + ",'" + gridNota._by_idx[rowIndex].item.idPai + "'," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + filho + "," + idty + "," + somaNotas + "," + id_participacao + ", '" + id_botao_participacao + "')", 1);
                 }
                return icon;
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatInputNotaAvaliacaoTurma2(value, rowIndex, obj, k) {
    try {
        var gridNota = dijit.byId("gridNota");
        var icon;
        var id = k.field + '_input_' + gridNota._by_idx[rowIndex].item.id;
        var id_botao_participacao = k.field + '_BTN_PART_' + gridNota._by_idx[rowIndex].item._0;

        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();
        if (rowIndex != -1) icon = "<input id='" + id + "' style='height:23px' />";
        var mediaNota = 0;
        var maiorNota = 0;
        var somaNotas = 0;
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;

        if (gridNota._by_idx[rowIndex].item.cd_tipo_avaliacao[0] == 0) {
            if ((hasValue(gridNota._by_idx[rowIndex].item.isChildren) && gridNota._by_idx[rowIndex].item.isChildren[0] >= 0) &&
                !(hasValue(gridNota._by_idx[rowIndex].item.cd_aluno) && gridNota._by_idx[rowIndex].item.cd_aluno != 0 && visao == VISAO_ALUNO) &&
                !(hasValue(gridNota._by_idx[rowIndex].item.cd_aluno) && hasValue(gridNota._by_idx[rowIndex].item.cd_aluno) &&
                    gridNota._by_idx[rowIndex].item.cd_aluno == 0 && gridNota._by_idx[rowIndex].item.cd_avaliacao != 0 && visao == VISAO_AVALIACAO)) {
                maiorNota = gridNota._by_idx[rowIndex].item.notaMaxima[0] == null ? 0 : parseFloat(gridNota._by_idx[rowIndex].item.notaMaxima[0]).toFixed(1);
                mediaNota = gridNota._by_idx[rowIndex].item.mediaNotas[0] == null ? 0 : parseFloat(gridNota._by_idx[rowIndex].item.mediaNotas[0]).toFixed(1);
                somaNotas = gridNota._by_idx[rowIndex].item.somaNotas[0] == null ? 0 : parseFloat(gridNota._by_idx[rowIndex].item.somaNotas[0]).toFixed(1);
                var idty = k.index; // gridNota._by_idx[rowIndex].idty;
                var totalNotas = gridNota._by_idx[rowIndex].item.vl_nota[0];
                var maximoNotaTurma = gridNota._by_idx[rowIndex].item.maximoNotaTurma[0];
                var filho = gridNota._by_idx[rowIndex].item.isChildren;
                var id_participacao = (hasValue(gridNota._by_idx[rowIndex].item.id_participacao) && (!hasValue(gridNota._by_idx[rowIndex].item.children) || gridNota._by_idx[rowIndex].item.children.length == 0)) ? gridNota._by_idx[rowIndex].item.id_participacao[0] : false; //LBM
                if (id_participacao) {
                    if (hasValue(dijit.byId(id_botao_participacao)))
                        dijit.byId(id_botao_participacao).destroy();
                    icon += "<input id='" + id_botao_participacao + "' class='nowrap' style='height:21px' />";
                    setTimeout(function () {
                        configuraButtonParticipacao(hasValue(value), id_botao_participacao, rowIndex, id);
                    }, 1);
                }

                if (visao == VISAO_AVALIACAO) {
                    if (gridNota._by_idx[rowIndex].item.pai[0] == 1)
                        setTimeout("configuraTextBoxNota(" + value + ", '" + id + "'," + true + "," + gridNota._by_idx[rowIndex].item.id + ",'" + gridNota._by_idx[rowIndex].item.idPai + "'," + totalNotas + "," + maximoNotaTurma + "," + maiorNota + "," + mediaNota + "," + filho + "," + idty + "," + somaNotas + "," + id_participacao + ",'" + id_botao_participacao + "')", 1);
                    else
                        setTimeout("configuraTextBoxNota(" + value + ", '" + id + "'," + false + "," + gridNota._by_idx[rowIndex].item.id + ",'" + gridNota._by_idx[rowIndex].item.idPai + "'," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + filho + "," + idty + "," + somaNotas + "," + id_participacao + ",'" + id_botao_participacao + "')", 1);
                }
                if (visao == VISAO_ALUNO) {
                    if (gridNota._by_idx[rowIndex].item.pai[0] == 1 && gridNota._by_idx[rowIndex].item.isChildren[0] == 0)
                        setTimeout("configuraTextBoxNota(" + value + ", '" + id + "'," + true + "," + gridNota._by_idx[rowIndex].item.id + ",'" + gridNota._by_idx[rowIndex].item.idPai + "'," + totalNotas + "," + maximoNotaTurma + "," + maiorNota + "," + somaNotas + "," + filho + "," + idty + "," + somaNotas + "," + id_participacao + ",'" + id_botao_participacao + "')", 1);
                    else
                        setTimeout("configuraTextBoxNota(" + value + ", '" + id + "'," + false + "," + gridNota._by_idx[rowIndex].item.id + ",'" + gridNota._by_idx[rowIndex].item.idPai + "'," + 0 + "," + 0 + "," + 0 + "," + 0 + "," + filho + "," + idty + "," + somaNotas + "," + id_participacao + ", '" + id_botao_participacao + "')", 1);
                }
                return icon;
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraButtonParticipacao(hasParticipacao, idfield, rowIndex, notaId) {
    try {
        var dialogGrid = '';
        var gridNota = dijit.byId("gridNota");
        var idNota = gridNota._by_idx[rowIndex].item.id[0];
        
        if (hasValue(dijit.byId('dialogGrid'))) dialogGrid = dijit.byId('dialogGrid');
        if (!hasValue(dijit.byId(idfield))) {
            require(["dijit/form/Button"], function (Button) {
                try {
                    var myButton = new Button({
                        title: "Clique aqui para lançar a nota de participação.",
                        iconClass: hasParticipacao == true ? 'dijitEditorIcon dijitEditorIconNewSGF' : 'dijitEditorIcon dijitEditorIconNewPage',
                        name: "button_" + idfield,
                        onClick: function () {
                            montarDialogParticipacao(gridNota._by_idx[rowIndex].item.participacoesDisponiveis[0], idfield, idNota, rowIndex, notaId);
                        }
                    }, idfield);
                }
                catch (er) {
                    postGerarLog(er);
                }
            });
            var buttonFkArray = [idfield];
            diminuirBotaoGrid(buttonFkArray);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function cancelarParticipacao() {
    var rowIndexParticipacao = parseInt(dojo.byId('rowIndexParticipacao').value);
    var gridNota = dijit.byId("gridNota");
    var idNota = gridNota._by_idx[rowIndexParticipacao].item.id[0];

    montarDialogParticipacao(gridNota._by_idx[rowIndexParticipacao].item.participacoesDisponiveis[0], dojo.byId('botaoClick').value, idNota, rowIndexParticipacao, dojo.byId('notaField').value);
}

function limparParticipacao() {
    var rowIndexParticipacao = parseInt(dojo.byId('rowIndexParticipacao').value);
    var gridNota = dijit.byId("gridNota");
    var idNota = gridNota._by_idx[rowIndexParticipacao].item.id[0];

    var participacoesDisponiveis = jQuery.parseJSON(gridNota._by_idx[rowIndexParticipacao].item.participacoesDisponiveis[0]);

    for (var i = 0; i < participacoesDisponiveis.length; i++) {
        participacoesDisponiveis[i].participacao_selecionada = false;
        participacoesDisponiveis[i].cd_conceito_participacao = 0;
        participacoesDisponiveis[i].vl_nota_participacao = 0;
    }

    gridNota._by_idx[rowIndexParticipacao].item.participacoesAluno[0] = '[]';
    
    montarDialogParticipacao(JSON.stringify(participacoesDisponiveis), dojo.byId('botaoClick').value, idNota, rowIndexParticipacao, dojo.byId('notaField').value);
}

function montarDialogParticipacao(participacoesDisponiveis, buttonField, idNota, rowIndex, notaField) {
    try {
        dojo.byId('botaoClick').value = buttonField;
        dojo.byId('notaField').value = notaField;
        dojo.byId('idNota').value = idNota;
        dojo.byId('rowIndexParticipacao').value = rowIndex;
        
        require(["dojo/ready", "dijit/DropDownMenu", "dijit/form/Button", "dijit/DropDownMenu", "dijit/MenuItem", "dijit/form/DropDownButton", "dojox/grid/EnhancedGrid", "dojo/data/ObjectStore",
                 "dojo/store/Memory", "dojo/_base/xhr", "dojox/json/ref", "dijit/MenuSeparator"],
            function (ready, DropDownMenu, Button, DropDownMenu, MenuItem, DropDownButton, EnhancedGrid, ObjectStore, Memory, xhr, ref, MenuSeparator) {
                ready(function () {
                    try {
                        destroyCreateGridParticipacao();
                        var conceitosDisponiveis = !hasValue(dijit.byId("gridNota").conceitosDisponiveis) ? 1 : dijit.byId("gridNota").conceitosDisponiveis;
                        var structure = new Array();
                        var width = (100 - 35) / conceitosDisponiveis.length;

                        structure.push({ name: " ", field: "no_participacao", width: "35%" });
                        for (var i = 0; i < conceitosDisponiveis.length; i++)
                            structure.push({ name: conceitosDisponiveis[i].no_conceito + "<br>(" + conceitosDisponiveis[i].vl_nota_participacao + ")", vl_nota_participacao: conceitosDisponiveis[i].vl_nota_participacao, cd_conceito: conceitosDisponiveis[i].cd_conceito, field: "participacao_selecionada", width: width + "%", styles: "text-align:center;", formatter: formatRadioButtonParticipacao });
                        structure.push({ name: "", field: "cd_participacao", styles: "display:none;" });

                        var dataParticipacoes = jQuery.parseJSON(participacoesDisponiveis);
                        var dataStore = new ObjectStore({ objectStore: new Memory({ data: dataParticipacoes }) });
                        var gridAvaliacaoParticipacao = new EnhancedGrid({
                            store: dataStore,
                            structure: structure,
                            plugins: {
                                pagination: {
                                    pageSizes: ["7", "14", "30", "100", "All"],
                                    description: true,
                                    sizeSwitch: true,
                                    pageStepper: true,
                                    defaultPageSize: "7",
                                    gotoButton: true,
                                    /*page step to be displayed*/
                                    maxPageStep: 4,
                                    /*position of the pagination bar*/
                                    position: "button"
                                }
                            },
                            noDataMessage: msgNotRegEnc
                        }, "gridAvaliacaoParticipacao");
                        
                        gridAvaliacaoParticipacao.canSort = function (col) { return false; };
                        gridAvaliacaoParticipacao.startup();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
            });
        
        dijit.byId('modalParticipacao').show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridParticipacao() {
    try {
        if (hasValue(dijit.byId("gridAvaliacaoParticipacao"))) {
            dijit.byId("gridAvaliacaoParticipacao").destroy();  
            $('<div>').attr('id', 'gridAvaliacaoParticipacao').attr('style', 'height: 260px;').appendTo('#paiGridAvaliacaoParticipacao');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraTextBoxNota(valorNota, idfield, isPai, id, idPai, totalNotas, maximoNotaTurma, maiorNota, mediaNotas, isChildren, idty, somaNotas, id_participacao, id_botao_participacao) {
    try {
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        var isInFocus = dojo.byId('isInFocus').value == 'true';
        if (isNaN(valorNota) || valorNota == null) {
            valorNota = "";
        }
        else if (valorNota >= 0 && valorNota != null) {
            if(!isInFocus)
                valorNota = parseFloat(valorNota).toFixed(1);
            else
                valorNota = parseFloat(valorNota).toFixed(0);
            valorNota = valorNota.toString().replace('.', ',');
        }
        if (isNaN(mediaNotas) || mediaNotas == null)
            mediaNotas = 0;
        else
            mediaNotas = maskFixed(mediaNotas, 1);
        if (isInFocus) mediaNotas = "";

        if (visao == VISAO_AVALIACAO)
            if (isPai && isChildren == 1) {
                if (!isInFocus)
                    somaNotas = maskFixed(somaNotas, 1);
                else
                    somaNotas = maskFixed(somaNotas, 0);
                valorNota = isInFocus ? "" : somaNotas;
            }

        if (!hasValue(dijit.byId(idfield))) {
            var kk = idty;
            var widthNota = id_participacao ? '60' : '100';
            var newTextBox = new dijit.form.TextBox({
                name: "textBox" + idfield,
                value: valorNota,
                style: "width: " + widthNota + "%;",
                patter: idPai,
                //placeHolder: isPai ? mediaNotas.toString().replace(".", ",") : '',
                onBlur: function (b) {
                    atualizarNotaAvalicao(id, idPai, this, visao, kk);
                }
            }, idfield);

            if (visao == VISAO_AVALIACAO) {
                if (isPai && hasValue(dijit.byId(idfield))) {
                    totalNotas = parseFloat(totalNotas).toFixed(1).toString().replace(".", ",");
                    maximoNotaTurma = parseFloat(maximoNotaTurma).toFixed(1).toString().replace(".", ",");
                    maiorNota = parseFloat(maiorNota).toFixed(1).toString().replace(".", ",");
                    dijit.byId(idfield).set('disabled', true);
                    maximoNotaTurma = maximoNotaTurma.toString(',', '.');
                    var media = (unmaskFixed(mediaNotas, 2) * 100) / unmaskFixed(maximoNotaTurma, 2);
                    dojo.attr(idfield, "title", "Média=" + maskFixed(unmaskFixed(mediaNotas, 1), 1) + " Máximo=" + maximoNotaTurma + " Média(%)=" + maskFixed(media, 2));
                    dijit.byId(idfield).set('value', mediaNotas);
                }
            } else {//Visão do aluno
                if (isPai && hasValue(dijit.byId(idfield) && (isChildren == 0))) {
                    totalNotas = parseFloat(totalNotas).toFixed(1).toString().replace(".", ",");
                    dojo.attr(idfield, "title", "Total=" + totalNotas);
                }
                //LBM habilitado para digitar notas na segunda visão.
                //dijit.byId(idfield).set('disabled', true);
            }
            if (hasValue(dijit.byId(idfield))) {
                dijit.byId(idfield).on("keypress", function (e) {
                    if(!isInFocus)
                        mascaraFloat(document.getElementById(idfield), 1)
                    else
                        mascaraFloat(document.getElementById(idfield), 0)
                });
            }
        }

        consistirSituacaoAlunoTurma(idfield, id, id_participacao, id_botao_participacao);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarNotaAvalicao(id, idPai, obj, visao, kk) {
    try{
        $('#' + obj.id).focus();
        var gridNota = dijit.byId("gridNota");
        var isInFocus = dojo.byId("isInFocus").value == 'true';
        var mediaNotas = 0;
        var totalNotas = 0;
        var maiorNotaTurma = 0;
        var notaMaximaTurma = 0.0;
        var somaNotas = 0;
        var nota = 0;
        var nota2 = 0;
        var nomeCampoPai = '';
        var dijitPai = '';
        var identidadePai = "";
        var cdAvaliacao = 0;
        var novaSomaNotas = 0;
        var numeroNotas = 0; //calcula o número de notas que são maior ou igual a zero
        if (visao == VISAO_AVALIACAO)
            for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
                for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children.length; k++) { //percorre os filhos do primerio nível
                    idPai = retornaIdentidadePai(gridNota, id);
                    if (gridNota.store._arrayOfTopLevelItems[i].children[k].pai[0] == 1 && gridNota.store._arrayOfTopLevelItems[i].children[k].id[0] == idPai) {
                        notaMaximaTurma = gridNota.store._arrayOfTopLevelItems[i].children[k].maximoNotaTurma[0];
                        for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children[k].children.length; j++) {
                            nota = gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota[0];
                            nota2 = gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota_2[0];
                            if (isNaN(nota))
                                nota = '';
                            if (isNaN(nota2))
                                nota2 = '';
                            if (nota != '' && nota != null && !isInFocus)
                                if (nota > 0) nota = parseFloat(gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota[0]).toFixed(1);
                            if (nota != '' && nota != null && isInFocus)
                                if (nota > 0) nota = parseFloat(gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota[0]).toFixed(0);
                            if (nota2 != '' && nota2 != null)
                                if (nota2 > 0) nota2 = parseFloat(gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota_2[0]).toFixed(1);
                            // Atualiza a nota que está sendo editada:
                            if (gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].id[0] == id) {
                                var peso = gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].peso[0];
                                var notaNova = obj.value;
                                if (notaNova != "" && notaNova != null) {
                                    notaNova = notaNova.toString().replace(',', '.');
                                    notaNova = parseFloat(notaNova).toFixed(1);
                                    gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].isModifiedA[0] = true;
                                    if (kk == 2)
                                        gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota[0] = notaNova;
                                    else
                                        gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota_2[0] = notaNova;
                                }
                                else {
                                    gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].isModifiedA[0] = true;
                                    if (kk == 2)
                                        gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota[0] = null;
                                    else
                                        gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota_2[0] = null;
                                }
                                if (kk == 2) {
                                    nota = notaNova;
                                    if (!NotaUtrapassaNotaMaxima(nota, peso, notaMaximaTurma)) {
                                        gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota[0] = "";
                                        nota = "";
                                        dijit.byId(obj.id).set('value', "");
                                        obj.value = "";
                                    }
                                }
                                else {
                                    nota2 = notaNova;
                                    if (!NotaUtrapassaNotaMaxima(nota2, peso, notaMaximaTurma)) {
                                        gridNota.store._arrayOfTopLevelItems[i].children[k].children[j].vl_nota_2[0] = "";
                                        nota2 = "";
                                        dijit.byId(obj.id).set('value', "");
                                        obj.value = "";
                                    }
                                }

                            }
                            //Calcula totais e médias considerando nota 2 se for o caso:
                            if (kk == 3 && nota2 >= 0 && nota2 != '' && nota2 != null)
                                nota = nota2

                            //Calcula o total das notas:
                            if (nota >= 0 && nota != '' && nota != null) {
                                numeroNotas++;
                                nota = parseFloat(nota * gridNota.store._arrayOfTopLevelItems[i].children[k].peso[0]);
                                totalNotas += nota;
                            }

                            //Calcula a nota máxima:
                            if (nota > maiorNotaTurma)
                                maiorNotaTurma = nota;

                            gridNota.store._arrayOfTopLevelItems[i].children[k].somaNotas[0] = totalNotas;
                            gridNota.store._arrayOfTopLevelItems[i].children[k].notaMaxima[0] = maiorNotaTurma;
                            gridNota.store._arrayOfTopLevelItems[i].children[k].isModified[0] = true;


                            if (totalNotas >= 0 && numeroNotas > 0) {
                                mediaNotas = parseFloat(totalNotas / numeroNotas).toFixed(1);
                                gridNota.store._arrayOfTopLevelItems[i].children[k].mediaNotas[0] = parseFloat(mediaNotas);
                            }
                            else {
                                mediaNotas = parseFloat(0).toFixed(1);
                                gridNota.store._arrayOfTopLevelItems[i].children[k].mediaNotas[0] = parseFloat(mediaNotas);
                            }
                            //Altera as informações do pai:
                            nomeCampoPai = 'vl_nota_input_' + gridNota.store._arrayOfTopLevelItems[i].children[k].id[0];
                            dijitPai = dijit.byId(nomeCampoPai);
                            gridNota.store._arrayOfTopLevelItems[i].children[k].vl_nota[0] = totalNotas;

                            var maximoNotaTurma = parseFloat(notaMaximaTurma).toFixed(1).toString().replace(".", ",");
                            maximoNotaTurma = maximoNotaTurma.toString(',', '.');
                            var media = (parseFloat(mediaNotas) * 100) / parseFloat(maximoNotaTurma);
                            media = parseFloat(media).toFixed(1);
                            media = media.toString().replace('.', ',');
                            if(hasValue(dojo.byId(nomeCampoPai), true))
                                dojo.attr(nomeCampoPai, "title", "Média=" + maskFixed(unmaskFixed(mediaNotas, 1),1) + " Máximo=" + notaMaximaTurma.toString().replace(".", ",") + " Média(%)=" + media);

                            if (hasValue(dijitPai))
                                dijitPai.set('value', mediaNotas.toString().replace(".", ","));
                        }
                    }
                }
                //  break;
            }
        else {
            identidadePai = retornaIdentidadeAvo(gridNota, id);
            for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
                if (gridNota.store._arrayOfTopLevelItems[i].pai[0] == 1 && gridNota.store._arrayOfTopLevelItems[i].id[0] == identidadePai) {
                    for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                        for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                            if (gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_avaliacao > 0) {
                                notaMaximaTurma = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].maximoNotaTurma[0];
                                nota = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0];
                                nota2 = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota_2[0];
                                if (isNaN(nota))
                                    nota = '';
                                if (nota != '' && nota != null && nota >= 0) {
                                    nota = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0];
                                    nota = parseFloat(nota).toFixed(1);
                                }
                                else nota = null;
                                if (isNaN(nota2))
                                    nota2 = '';
                                if (nota2 != '' && nota2 != null && nota2 >= 0) {
                                    nota2 = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota_2[0];
                                    nota2 = parseFloat(nota).toFixed(1);
                                }
                                else nota2 = null;
                                // Atualiza a nota que está sendo editada:
                                if (gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id[0] == id) {
                                    var peso = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].peso[0];
                                    var novaNota = obj.value;
                                    if (novaNota != "" && novaNota != null) {
                                        notaNova = novaNota.toString().replace(',', '.');
                                        notaNova = parseFloat(notaNova).toFixed(1);
                                        gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].isModifiedA[0] = true;
                                        if (kk == 2)
                                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0] = notaNova;
                                        else
                                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota_2[0] = notaNova;
                                    }
                                    else {
                                        gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].isModifiedA[0] = true;
                                        if (kk == 2)
                                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0] = null;
                                        else
                                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota_2[0] = null;
                                    }
                                    if (kk == 2) {
                                        nota = novaNota;
                                        if (!NotaUtrapassaNotaMaxima(nota, peso, notaMaximaTurma)) {
                                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0] = "";
                                            nota = "";
                                            dijit.byId(obj.id).set('value', "");
                                            obj.value = "";
                                        }
                                    }
                                    else {
                                        nota2 = novaNota;
                                        if (!NotaUtrapassaNotaMaxima(nota2, peso, notaMaximaTurma)) {
                                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota_2[0] = "";
                                            nota2 = "";
                                            dijit.byId(obj.id).set('value', "");
                                            obj.value = "";
                                        }
                                    }

                                }
                                nota = unmaskFixed(nota, 1);
                                //Calcula totais e médias considerando nota 2 se for o caso:
                                if (kk == 3 && nota2 >= 0 && nota2 != '' && nota2 != null)
                                    nota = nota2

                                if (nota >= 0 && nota != '' && nota != null) {
                                    totalNotas += parseFloat(nota * gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].peso[0]);
                                    novaSomaNotas = totalNotas;
                                }
                                //Altera as informações do pai: Não tem mais pai LBM
                                //nomeCampoPai = 'vl_nota_input_' + gridNota.store._arrayOfTopLevelItems[i].id[0];
                                //dijitPai = dijit.byId(nomeCampoPai);
                                //dojo.attr(nomeCampoPai, "title", "Total=" + totalNotas.toString().replace(".", ","));
                                //dijitPai.set('value', parseFloat(totalNotas).toFixed(1).toString().replace(".", ","));
                                if(!isInFocus)
                                    gridNota.store._arrayOfTopLevelItems[i].vl_nota[0] = parseFloat(novaSomaNotas).toFixed(1);
                                else
                                    gridNota.store._arrayOfTopLevelItems[i].vl_nota[0] = parseFloat(novaSomaNotas).toFixed(0);
                                gridNota.store._arrayOfTopLevelItems[i].isModified[0] = true;;
                            }
                        }// for k
                    }// for j
                }//if pai
            }//end for first

            //---Recalcular as médias para a visão do aluno--
            //Pega o código da avaliação para fazer a consistência

            for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) 
                for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) 
                    for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++) 
                        if (id == gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id)
                            cdAvaliacao = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_avaliacao;

            // recalculando a média para a  visão do aluno, não será mostrado na tela só server para a consistência na base --
            if (cdAvaliacao != 0) {
                nota = 0;
                somaNotas = 0;
                mediaNotas = 0;
                var interar = 0;
                for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++)
                    for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) 
                        for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++) 
                            if (cdAvaliacao[0] == gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_avaliacao[0]) {
                                nota = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0];
                                //condiona a nota para que possa fazer a média para os alunos que fizeram a prova, pois se o aluno for matriculado após a prova a nota deve ser vazia.
                                if (nota != null && nota != "" && !isNaN(nota)) {
                                    nota = unmaskFixed(nota, 1);
                                    interar++;
                                    somaNotas += parseFloat(nota);
                                    mediaNotas = parseFloat(somaNotas / interar).toFixed(1);
                                }
                            }

                //atualiza os valores e medias para cada filho na visão do aluno
                for (var l = 0; l < gridNota.store._arrayOfTopLevelItems.length; l++) {//alunos
                    for (var m = 0; m < gridNota.store._arrayOfTopLevelItems[l].children.length; m++) {//tipos
                        for (var n = 0; n < gridNota.store._arrayOfTopLevelItems[l].children[m].children.length; n++) {//nomes
                            if (cdAvaliacao[0] == gridNota.store._arrayOfTopLevelItems[l].children[m].children[n].cd_avaliacao[0]) {
                                if (!isInFocus) {
                                    gridNota.store._arrayOfTopLevelItems[l].children[m].children[n].somaNotas[0] = parseFloat(somaNotas).toFixed(1);
                                    gridNota.store._arrayOfTopLevelItems[l].children[m].children[n].mediaNotas[0] = parseFloat(mediaNotas).toFixed(1);
                                }
                                else {
                                    gridNota.store._arrayOfTopLevelItems[l].children[m].children[n].somaNotas[0] = parseFloat(somaNotas).toFixed(0);
                                    gridNota.store._arrayOfTopLevelItems[l].children[m].children[n].mediaNotas[0] = parseFloat(mediaNotas).toFixed(0);
                                }
                            }
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

function NotaUtrapassaNotaMaxima(nota, peso, notaMaximaTurma) {
    try{
        if (nota * peso > notaMaximaTurma) {
            var mensagem_erro = msgNotaMaiorNotaTurma + ' ' + parseFloat(notaMaximaTurma).toFixed(1) + '.';
            if (peso > 1)
                mensagem_erro += msgPesoUtilizadoCalculo + peso;
            caixaDialogo(DIALOGO_ERRO, mensagem_erro, null);
            return false;
        }
        return true;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region dropdown para coneceito da grid nota

function formatDropDownConceitoAvaliacaoTurma(value, rowIndex, obj, k) {
    try {
        var gridNota = dijit.byId("gridNota");
        var icon;
        var idConceito = 0;
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        if ((visao == VISAO_ALUNO)) {
            if (hasValue(gridNota._by_idx[rowIndex].item.isChildren) && gridNota._by_idx[rowIndex].item.isChildren[0] == 1) {
                idConceito = gridNota._by_idx[rowIndex].item.cd_conceito[0];

                var idField = k.field + '_drpw_' + gridNota._by_idx[rowIndex].item._0;
                if (hasValue(dijit.byId(idField)))
                    dijit.byId(idField).destroy();

                if (rowIndex != -1) icon = "<input id='" + idField + "' /> ";
                setTimeout("configuraDropDownNota(" + value + ", '" + idField + "'," + gridNota._by_idx[rowIndex].item.id + "," + gridNota._by_idx[rowIndex].item.pai + "," + idConceito + "," + gridNota._by_idx[rowIndex].item.cd_produto + ")", 1);
                return icon;
            }
        }
        if ((visao == VISAO_AVALIACAO) && (!gridNota._by_idx[rowIndex].item.pai[0] == 1)) {
            if (hasValue(gridNota._by_idx[rowIndex].item.isChildren) && gridNota._by_idx[rowIndex].item.isChildren[0] == 1) {
                idConceito = gridNota._by_idx[rowIndex].item.cd_conceito[0];

                var idField = k.field + '_drpw_' + gridNota._by_idx[rowIndex].item._0;
                if (hasValue(dijit.byId(idField)))
                    dijit.byId(idField).destroy();

                if (rowIndex != -1) icon = "<input id='" + idField + "' /> ";
                setTimeout("configuraDropDownNota(" + value + ", '" + idField + "'," + gridNota._by_idx[rowIndex].item.id + "," + gridNota._by_idx[rowIndex].item.pai + "," + idConceito + "," + gridNota._by_idx[rowIndex].item.cd_produto + ")", 1)
                return icon;
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraDropDownNota(value, idfield, id, idPai, idConceito, cdProduto) {
    var conceito = [];
    if (idConceito == null) idConceito = 0;
    if (!hasValue(dijit.byId(idfield))) {
        require(["dojo/_base/xhr", "dojo/store/Memory", "dijit/form/FilteringSelect"],
                function (xhr, Memory, FilteringSelect) {
            xhr.get({
                url: Endereco() + "/api/coordenacao/returnConceitosAtivos?idProduto=" + (cdProduto || 0) + "&idConceito=" + idConceito,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataConceito) {
                try{
                    var data = $.parseJSON(dataConceito).retorno;

                    conceito.push({
                        id: 0,
                        name: "Escolha Conceito",
                        idName: "Escolha Conceito"
                    });

                    $.each(data, function (i, value) {
                        conceito.push({
                            id: value.cd_conceito,
                            name: value.no_conceito,
                            idName: value.no_conceito
                        });
                    })

                    var stateStore = new Memory({
                        data: conceito
                    });
                    var cbxConceito = dijit.byId(idfield);
                    //if (hasValue(cbxConceito)) cbxConceito.destroy();
                    if (idConceito == null || idConceito == 0) idConceito = '0';
                    else idConceito;

                    if (!hasValue(cbxConceito)) 
                        var cbxConceito = new FilteringSelect({
                            id: idfield,
                            name: idfield,
                            store: stateStore,
                            value: idConceito,
                            searchAttr: "idName",
                            style: "width:100%",
                            onBlur: function (b) { atualizarConceito(id, idPai, this); }
                        }, idfield);
                    
                    consistirSituacaoAlunoTurma(idfield, id, false);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },

            function (error) {
                apresentaMensagem('apresentadorMensagemAvaliacaoTurma', error);
            });
        });
    }
}

function atualizarConceito(id, idPai, obj) {
    try{
        var quebra = 0;
        var gridNota = dijit.byId("gridNota");
        var idConceito = obj.value > 0 ? obj.value : null;
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
            if (visao == VISAO_ALUNO)
                gridNota.store._arrayOfTopLevelItems[i].isModified[0] = true;
            for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                if (visao == VISAO_AVALIACAO)
                    gridNota.store._arrayOfTopLevelItems[i].children[j].isModified[0] = true;
                for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                    if (visao == VISAO_AVALIACAO)
                        if (gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id[0] == id) {
                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_conceito[0] = obj.value;
                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].isModifiedA[0] = true;
                            quebra = 1;
                            break;
                        }
                    if (visao == VISAO_ALUNO)
                        if (gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id[0] == id) {
                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_conceito[0] = obj.value;
                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].isModifiedA[0] = true;
                            quebra = 1;
                            break;
                        }
                }
                if (quebra == 1) break;
            }
        if (quebra == 1) break;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region monta o idSegundaProva
function formatCheckIdSegunda(value, rowIndex, obj, k) {
    try{
        var gridNota = dijit.byId("gridNota");
        var icon;
        var id = k.field + '_Selected_' + gridNota._by_idx[rowIndex].item._0;
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        if (gridNota._by_idx[rowIndex].item.pai[0] == 0 && hasValue(gridNota._by_idx[rowIndex].item.isChildren) && gridNota._by_idx[rowIndex].item.isChildren[0] == 1) {
            var idFilho = gridNota._by_idx[rowIndex].item.id[0];
            if (hasValue(dijit.byId(id)))
                dijit.byId(id).destroy();
            if (rowIndex != -1) icon = "<input id='" + id + "' /> ";
            setTimeout("configuraCheckBoxIdSegunda(" + value + ", " + rowIndex + ", '" + id + "'," + idFilho + ")", 1);
            return icon;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxIdSegunda(value, rowIndex, id, idFilho) {
    if (!hasValue(dijit.byId(id))) {
        require(["dijit/form/CheckBox"], function (CheckBox) {
            var checkBox = new CheckBox({
                name: "checkBox" + id,
                value: value,
                checked: value == null ? false : value,
                onChange: function (b) { atualizarCheckBox(idFilho, this) }
            }, id);
        });
        // consistirSituacaoAlunoTurma(id, idFilho);
    }
}

function atualizarCheckBox(idFilho, obj) {
    try{
        var gridNota = dijit.byId("gridNota");
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        var identidadePai = 0;
        for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++)
            for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++)
                for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++)
                    if (idFilho == gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id) {
                        gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id_segunda_prova[0] = obj.checked;
                        break;
                    }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region  monta o formatDataAvaliacao
function formatDataAvaliacao(value, rowIndex, obj, k) {
    try {
        var gridNota = dijit.byId("gridNota");
        var icon;
        var id = k.field + '_Selected_' + gridNota._by_idx[rowIndex].item._0;
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        var lenthFilhas = 0;
        if ((dijit.byId(id) != null))
            dijit.byId(id).destroy();
        if (rowIndex != -1) icon = "<input id='" + id + "' /> ";
        if (visao == VISAO_AVALIACAO && gridNota._by_idx[rowIndex].item.pai[0] == 1)
            if (gridNota._by_idx[rowIndex].item.pai[0] == 1 && hasValue(gridNota._by_idx[rowIndex].item.isChildren) && gridNota._by_idx[rowIndex].item.isChildren[0] == 1) {
                if (hasValue(gridNota._by_idx[rowIndex].item.children))
                    lenthFilhas = gridNota._by_idx[rowIndex].item.children.length;
                var idFilho = gridNota._by_idx[rowIndex].item.id[0];
                value = gridNota._by_idx[rowIndex].item.dta_avaliacao_turma[0];
                setTimeout("configuraDateAvaliacao('" + value + "', '" + rowIndex + "', '" + id + "','" + idFilho + "','" + lenthFilhas + "')", 1);
                return icon;
            }
        if (visao == VISAO_ALUNO)
            if (hasValue(gridNota._by_idx[rowIndex].item.isChildren) && gridNota._by_idx[rowIndex].item.isChildren[0] == 1) {
                var idFilho = gridNota._by_idx[rowIndex].item.id[0];
                value = gridNota._by_idx[rowIndex].item.dta_avaliacao_turma[0];
                setTimeout("configuraDateAvaliacao('" + value + "', '" + rowIndex + "', '" + id + "','" + idFilho + "')", 1);
                return icon;
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraDateAvaliacao(valor, rowIndex, id, idFilho, lenth) {
    try{
        require(["dijit/form/DateTextBox"],
             function (DateTextBox) {
                valor = valor == "" ? null : valor;
                if ((dijit.byId(id) == null)) 
                    var date = new DateTextBox({
                        value: valor,
                        name: "data" + id,
                        style: "width: 100%;",
                        required: true,
                        disabled: dijit.byId('visao') == VISAO_ALUNO,
                        onChange: function (b) {
                            atualizarDataAvaliacao(idFilho, this, id, rowIndex, lenth);
                        },
                        onClick: function (b) { setarFocus(this); }
                    }, id);
                if (dijit.byId(id) != null) {
                    dijit.byId(id).set('value', valor);
                    dojo.byId(id).value = valor;
                    maskDate("#" + id);
                }
        });
        apresentaMensagem('apresentadorMensagemAvaliacaoTurma', null);
        consistirSituacaoAlunoTurma(null, idFilho, false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarFocus(obj) {
    $('#' + obj.id).focus()
}

function atualizarDataAvaliacao(idFilho, obj, id, rowIndex, qtdFilhos) {
    try{
        qtdFilhos = parseInt(qtdFilhos);
        rowIndex = parseInt(rowIndex);
        var idObjNeto = "";
        var gridNota = dijit.byId("gridNota");
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        var isConceito = hasValue(dijit.byId('tipoAvaliacao')) ? dijit.byId('tipoAvaliacao').value : 0;

        if (visao == VISAO_AVALIACAO) {
                for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
                    for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                        if (idFilho == gridNota.store._arrayOfTopLevelItems[i].children[j].id[0] && hasValue(dojo.byId(id))) {
                            gridNota.store._arrayOfTopLevelItems[i].children[j].dt_avaliacao_turma[0] = dojo.date.locale.parse(dojo.byId(id).value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                            gridNota.store._arrayOfTopLevelItems[i].children[j].dta_avaliacao_turma[0] = dojo.byId(id).value;
                            gridNota.store._arrayOfTopLevelItems[i].children[j].isModified[0] = true;
                            dijit.byId(id).set('value', obj.value);
                            for (var l = 0, qtd = 1; l < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; l++, qtd++) {
                                var idNeto = gridNota.store._arrayOfTopLevelItems[i].children[j].children[l].id[0];
                                var idParticipacao = gridNota.store._arrayOfTopLevelItems[i].children[j].children[l].id_participacao[0];
                                if (hasValue(gridNota.getRowNode(rowIndex + qtd))) {
                                    if (isConceito == NOTA && gridNota._treeCache.items[rowIndex].opened && hasValue(gridNota.getRowNode(rowIndex + qtd).childNodes[0].childNodes[0].childNodes[0].childNodes[2].firstChild.firstChild)) {
                                        idObjNeto = gridNota.getRowNode(rowIndex + qtd).childNodes[0].childNodes[0].childNodes[0].childNodes[2].firstChild.firstChild.firstChild.id;
                                        consistirSituacaoAlunoTurma(idObjNeto, idNeto, idParticipacao);
                                        idObjNeto = gridNota.getRowNode(rowIndex + qtd).childNodes[0].childNodes[0].childNodes[0].childNodes[3].firstChild.firstChild.firstChild.id;
                                        consistirSituacaoAlunoTurma(idObjNeto, idNeto, idParticipacao);
                                    }
                                    else
                                        if (isConceito == CONCEITO && gridNota.getRowNode(rowIndex + qtd).childNodes[0].childNodes[0].childNodes[0].childNodes[2].firstChild.childNodes[2] != null) {
                                            idObjNeto =               gridNota.getRowNode(rowIndex + qtd).childNodes[0].childNodes[0].childNodes[0].childNodes[2].firstChild.childNodes[2].firstChild.id;
                                            consistirSituacaoAlunoTurma(idObjNeto, idNeto, idParticipacao);
                                        }
                                }
                            }
                            break;
                        }
                    }
                }
            } else {
                for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
                    gridNota.store._arrayOfTopLevelItems[i].isModified[0] = true;
                    for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                        for (var l = 0; l < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; l++) {
                            if (idFilho == gridNota.store._arrayOfTopLevelItems[i].children[j].children[l].id[0]) {
                                gridNota.store._arrayOfTopLevelItems[i].children[j].children[l].dt_avaliacao_turma[0] = dojo.date.locale.parse(dojo.byId(id).value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                                gridNota.store._arrayOfTopLevelItems[i].children[j].children[l].dta_avaliacao_turma[0] = dojo.byId(id).value;
                                dijit.byId(id).set('value', obj.value);
                                if (hasValue(gridNota.getRowNode(rowIndex))) {
                                    if (isConceito == NOTA)
                                        idObjNeto = gridNota.getRowNode(rowIndex).childNodes[0].childNodes[0].childNodes[0].childNodes[2].childNodes[0].childNodes[0].children[0].id;
                                    else
                                        idObjNeto = gridNota.getRowNode(rowIndex).childNodes[0].childNodes[0].childNodes[0].childNodes[2].childNodes[0].childNodes[2].childNodes[0].id;
                                    consistirSituacaoAlunoTurma(idObjNeto, idFilho);
                                }
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
//#endregion

//#region motar dropdown avaliador
function formatAvaliador(value, rowIndex, obj, k) {
    try {
        
        var gridNota = dijit.byId("gridNota");
        var icon;
        var id = k.field + '_drpw_' + gridNota._by_idx[rowIndex].item._0;
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;

        if ((dijit.byId(id) != null))
            dijit.byId(id).destroy();
        if (rowIndex != -1) icon = "<input id='" + id + "' /> ";
        if (visao == VISAO_AVALIACAO && gridNota._by_idx[rowIndex].item.pai[0] == 1)
            if (gridNota._by_idx[rowIndex].item.pai[0] == 1 && hasValue(gridNota._by_idx[rowIndex].item.isChildren) && gridNota._by_idx[rowIndex].item.isChildren[0] == 1) {

                var idFilho = gridNota._by_idx[rowIndex].item.id[0];
                value = gridNota._by_idx[rowIndex].item.cd_funcionario[0];

                setTimeout(function () { configuraDropDownAvaliador(value, rowIndex, id, idFilho) }, 1);
                return icon;
            }
        if (visao == VISAO_ALUNO)
            if (hasValue(gridNota._by_idx[rowIndex].item.isChildren) && gridNota._by_idx[rowIndex].item.isChildren[0] == 1) {

                var idFilho = gridNota._by_idx[rowIndex].item.id[0];
                value = gridNota._by_idx[rowIndex].item.cd_funcionario[0];

                setTimeout(function () { configuraDropDownAvaliador(value, rowIndex, id, idFilho) }, 1);
                return icon;
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraDropDownAvaliador(valor, rowIndex, id, idFilho) {
    var avaliadores = [];
    if (!hasValue(dijit.byId(id), true)) {
        try{
            var data = jQuery.parseJSON(dojo.byId('cbAvaliadorAvaliacao').value);
            avaliadores.push({
                id: 0,
                name: "Escolha Avaliador",
                idName: "Escolha Avaliador"
            });

            $.each(data, function (i, value) {
                avaliadores.push({
                    id: value.cd_funcionario,
                    name: value.no_pessoa,
                    idName: value.no_pessoa
                });
            })

            var stateStore = new dojo.store.Memory({
                data: avaliadores
            });
            var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
            var avaliador_desabilitado = (visao == VISAO_ALUNO);
                    
            if (!hasValue(dijit.byId(id), true)) {
                if (!hasValue(valor) || valor < 0)
                    valor = 0;

                var cbxAvaliador = new dijit.form.FilteringSelect({
                    id: id,
                    name: id,
                    store: stateStore,
                    value: valor,
                    searchAttr: "idName",
                    style: "width:100%",
				    disabled: avaliador_desabilitado,
                    onBlur: function (b) { atualizarAvaliador(id, idFilho, this); }
                }, id);
            }
        }
        catch (e) {
            postGerarLog(e);
        }
        // consistirSituacaoAlunoTurma(idfield, id);
    }
}

function atualizarAvaliador(id, idFilho, obj) {
    try{
        var gridNota = dijit.byId("gridNota");
        var cod_avaliador = obj.value > 0 ? obj.value : 0;

        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
            if (visao == VISAO_ALUNO) 
                gridNota.store._arrayOfTopLevelItems[i].isModified[0] = true;
            for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                if (visao == VISAO_AVALIACAO) {
                    if (gridNota.store._arrayOfTopLevelItems[i].children[j].id[0] == idFilho) {
                        gridNota.store._arrayOfTopLevelItems[i].children[j].cd_funcionario[0] = cod_avaliador;
                        gridNota.store._arrayOfTopLevelItems[i].children[j].isModified[0] = true;
                        break;
                    }
                } else {
                    for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                        if (gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id[0] == idFilho) {
                            gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_funcionario[0] = cod_avaliador;
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

//#endregion

//#endregion

//#region formatando imagem e botão para a observação da grade

function formatImage(value, rowIndex, obj, k) {
    try {
        var gridNota = dijit.byId("gridNota");
        var icon;
        var id = k.field + '_IMG_' + gridNota._by_idx[rowIndex].item._0;
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        var hasTexto = hasValue(value) ? true : false;

        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();
        if (rowIndex != -1) icon = "<input id='" + id + "' style='height:21px' /> ";

        if (visao == VISAO_AVALIACAO)
            if (gridNota._by_idx[rowIndex].item.cd_tipo_avaliacao[0] == 0) {
                if (!(gridNota._by_idx[rowIndex].item.pai[0] == 1))
                    setTimeout("configuraButtonImg(" + hasTexto + ", '" + id + "'," + gridNota._by_idx[rowIndex].item.id + "," + gridNota._by_idx[rowIndex].item.idPai + "," + false + ",'" + +gridNota._by_idx[rowIndex].item.isChildren + "')", 1);
                else
                    setTimeout("configuraButtonImg(" + hasTexto + ", '" + id + "'," + gridNota._by_idx[rowIndex].item.id + "," + gridNota._by_idx[rowIndex].item.idPai + "," + true + ",'" + +gridNota._by_idx[rowIndex].item.isChildren + "')", 1);
                return icon;
            }
        if (visao == VISAO_ALUNO)
            if (gridNota._by_idx[rowIndex].item.cd_tipo_avaliacao == 0) {
                if ((gridNota._by_idx[rowIndex].item.pai[0] != 1) && hasValue(gridNota._by_idx[rowIndex].item.isChildren[0]) && (gridNota._by_idx[rowIndex].item.isChildren[0] == 1)) {
                    setTimeout("configuraButtonImg(" + hasTexto + ", '" + id + "'," + gridNota._by_idx[rowIndex].item.id + "," + gridNota._by_idx[rowIndex].item.idPai + "," + false + ")", 1);
                    return icon;
                }
            } 
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatMsg(value, rowIndex, obj, k) {
    try {
        var gridNota = dijit.byId("gridNota");
        var icon;
        var id = k.field + '_IMG_' + gridNota._by_idx[rowIndex].item._0;
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        var hasTexto = hasValue(value) ? true : false;

        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();
        if (rowIndex != -1) icon = "<input id='" + id + "' style='height:21px' /> ";

        if (visao == VISAO_ALUNO)
            if (gridNota._by_idx[rowIndex].item.cd_tipo_avaliacao != 0) {
                if ((gridNota._by_idx[rowIndex].item.pai[0] != 1)) {
                    setTimeout("configuraButtonMsg(" + hasTexto + ", '" + id + "'," + gridNota._by_idx[rowIndex].item.id + "," + gridNota._by_idx[rowIndex].item.idPai[0] + "," + false + ")", 1);
                    return icon;
                }
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraButtonImg(value, idfield, id, idPai, isPai, isFilho) {
    try{
        var dialogGrid = '';
        if (hasValue(dijit.byId('dialogGrid'))) dialogGrid = dijit.byId('dialogGrid');
        if (!hasValue(dijit.byId(idfield))) {
            require(["dijit/form/Button"], function (Button) {
                try{
                    var myButton = new Button({
                        title: "Clique aqui para digitar uma observação",
                        iconClass: value == true ? 'dijitEditorIcon dijitEditorIconNewSGF' : 'dijitEditorIcon dijitEditorIconNewPage',
                        name: "button_" + idfield,
                        onClick: function () {
                            montarDialogObservacao(id, idPai, this, dialogGrid, isPai, idfield, isFilho);
                        }
                    }, idfield);
                }
                catch (er) {
                    postGerarLog(er);
                }
            });
            var buttonFkArray = [idfield];
            diminuirBotaoGrid(buttonFkArray);
            //consistirSituacaoAlunoTurma(idfield, isFilho);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraButtonMsg(value, idfield, id, idPai, isPai, isFilho) {
    try {
        var dialogGrid = '';
        //if (hasValue(dijit.byId('dialogGrid'))) dialogGrid = dijit.byId('dialogGrid');
        if (hasValue(dijit.byId('cadMensagemAluno'))) dialogGrid = dijit.byId('cadMensagemAluno');
        if (!hasValue(dijit.byId(idfield))) {
            require(["dijit/form/Button"], function (Button) {
                try {
                    var myButton = new Button({
                        title: "Clique aqui para pesquisar/editar uma mensagem",
                        iconClass: value == true ? 'dijitEditorIcon dijitEditorIconNewSGF' : 'dijitEditorIcon dijitEditorIconNewPage',
                        name: "button_" + idfield,
                        onClick: function () {
                            montarDialogMsg(id, dialogGrid, idfield, isFilho);
                        }
                    }, idfield);
                }
                catch (er) {
                    postGerarLog(er);
                }
            });
            var buttonFkArray = [idfield];
            diminuirBotaoGrid(buttonFkArray);
            //consistirSituacaoAlunoTurma(idfield, isFilho);
        //    dijit.byId('btCancelarMsgAluno').onClick = function () {
        //        dijit.byId('cadMensagemAluno').hide();
        //        montarDialogMsg(id, dialogGrid, idfield, isFilho);
        //    }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarDialogObservacao(id, idPai, obj, dialog, isPai, field, isFilho) {
    try{
        var gridNota = dijit.byId("gridNota");
        var descricao = '';
        var descricaoOld = '';
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        if (hasValue(dijit.byId('textAreaObs'))) dijit.byId('textAreaObs').set('value', '');
        dojo.byId('botaoClick').value = "";
        dojo.byId('botaoClick').value = field;
        dijit.byId('btOKNota').set('iconClass', 'dijitEditorIcon dijitEditorIconRedo');
        dijit.byId('btCancelarNota').set('iconClass', 'dijitEditorIcon dijitEditorIconCancel');
        isFilho = parseInt(isFilho);
        // pega o valor do pai e set no textAreas
        if (visao == VISAO_AVALIACAO) {
            for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
                for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                    if (isPai && isFilho == 1) {// entra aqui so se for pai e filho ao mesmo tempo ou seja se for os nomes das avaliações.
                        if (gridNota.store._arrayOfTopLevelItems[i].children[j].pai[0] == 1 && gridNota.store._arrayOfTopLevelItems[i].children[j].id[0] == id) {
                            if (gridNota.store._arrayOfTopLevelItems[i].children[j].dc_observacao[0] !== "") {
                                descricaoOld = gridNota.store._arrayOfTopLevelItems[i].children[j].dc_observacao + '';
                                dijit.byId('textAreaObs').set('value', descricaoOld);
                            }
                            break;
                        }
                    }
                    else {
                        idPai = retornaIdentidadePai(gridNota, id);
                        if (gridNota.store._arrayOfTopLevelItems[i].children[j].pai[0] == 1 & gridNota.store._arrayOfTopLevelItems[i].children[j].id[0] == idPai) {
                            for (var m = 0; m < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; m++) {
                                if (gridNota.store._arrayOfTopLevelItems[i].children[j].children[m].dc_observacao != "" && gridNota.store._arrayOfTopLevelItems[i].children[j].children[m].id == id) {
                                    descricaoOld = gridNota.store._arrayOfTopLevelItems[i].children[j].children[m].dc_observacao + '';
                                    dijit.byId('textAreaObs').set('value', descricaoOld);
                                }
                            }
                            break;
                        }
                    }//else
                }//for j
            }//for i
        }//end if VISAO_AVALIACAO
        else {
            for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) { //TODO: tentar trocar por uma função mais performatica (busca binária)
                for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++)
                    for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++)
                        if (gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id == id) {
                            descricaoOld = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].dc_observacao + '';
                            dijit.byId('textAreaObs').set('value', descricaoOld);
                        }
            }
        }//end else

        //inputs auxiliares para gardar as informações do registro que esta sendo clicacado
        if (hasValue(dojo.byId('idNota'))) {
            dojo.byId('idNota').value = "";
            dojo.byId('idNota').value = id;
        };
        if (hasValue(dojo.byId('isPai'))) {
            dojo.byId('isPai').value = "";
            dojo.byId('isPai').value = isPai;
        };
        if (hasValue(dojo.byId('idPaiNota'))) {
            dojo.byId('idPaiNota').value = "";
            dojo.byId('idPaiNota').value = retornaIdentidadePai(gridNota, id);
        }

        dialog.show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarDialogMsg(id, dialog, field) {
    try {
        apresentaMensagem('apresentadorMensagemAluno', null);
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemAvaliacaoTurma', null);
        var gridNota = dijit.byId("gridNota");
        var cd_aluno = 0;
        var cd_tipo_avaliacao = 0;
        var aux = 0; //Variável para quebrar loop
        if (hasValue(dijit.byId('textAreaObsAluno'))) dijit.byId('textAreaObsAluno').set('value', "");
        dojo.byId('botaoClick').value = "";
        dojo.byId('botaoClick').value = field;
        // pega o valor do cd_aluno
        for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
            for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++)
                //for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++)
                if (gridNota.store._arrayOfTopLevelItems[i].children[j].id == id) {
                    cd_aluno = gridNota.store._arrayOfTopLevelItems[i].cd_aluno[0];
                    cd_tipo_avaliacao = gridNota.store._arrayOfTopLevelItems[i].children[j].cd_tipo_avaliacao[0];
                    var aux = 1;
                    break;
                }
            if (aux == 1) break
        }
        var cdProduto = dojo.byId('cd_produto').value;
        var cdCurso = dojo.byId('cd_curso').value;
        MSGALUNO = [];
        retornaMsgAluno(cd_tipo_avaliacao, cd_aluno, cdProduto, cdCurso, function () {
            dojo.byId('produtoCAD').value = dojo.byId('produto').value;
            dojo.byId('cursoCAD').value = dojo.byId('curso').value;
            dojo.byId('cd_produto_CAD').value = cdProduto;
            dojo.byId('cd_curso_CAD').value = cdCurso;
            if (MSGALUNO.length == 1) {
                dijit.byId('textAreaObsAluno').set('value', MSGALUNO[0].tx_mensagem_avaliacao_aluno);
                dijit.byId('ckMsgAlunoAtivo').set('checked', MSGALUNO[0].id_mensagem_ativa);
                dojo.byId('cd_mensagem_avaliacao').value = MSGALUNO[0].cd_mensagem_avaliacao;
                dijit.byId('btLimparMsgAluno').set('label', 'Excluir');
                dijit.byId('btAlterarMsgAluno').set('label', 'Salvar');
                document.getElementById("divGravarMensagemAluno").style.display = "none";
                document.getElementById("divGravarMensagemPadraoAluno").style.display = "";
            }
            else {
                dijit.byId('btLimparMsgAluno').set('label', 'Limpar');
                dijit.byId('btAlterarMsgAluno').set('label', 'Salvar');
                document.getElementById("divGravarMensagemAluno").style.display = "";
                document.getElementById("divGravarMensagemPadraoAluno").style.display = "none";
            }
            dijit.byId('btIncluirMsgAluno').set('disabled', MSGALUNO.length == 1);
            dialog.show();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function diminuirBotaoGrid(buttonFkArray) {
    try{
        for (var p = 0; p < buttonFkArray.length; p++) {
            var buttonFk = document.getElementById(buttonFkArray[p]);

            if (hasValue(buttonFk)) {
                buttonFk.parentNode.style.minWidth = '13px';
                buttonFk.parentNode.style.width = '15px';
                buttonFk.parentNode.style.height = '15px';
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#endregion

//#region métodos auxiliares - destroyCreateGridNotas - keepValues

var NotaConceito = new Array(
    { name: "Conceito", id: "1" },
    { name: "Notas", id: "2" }
);

function returnDataTurma() {
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post(Endereco() + "/api/turma/returnDataAvaliacaoTurma", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson()
        }).then(function (returnDataAvaliacaoTurma) {
            try{
                if (!hasValue(returnDataAvaliacaoTurma.erro)) {
                    var result = $.parseJSON(returnDataAvaliacaoTurma).retorno;
                    loadSelect(result.tiposAvaliacoes, 'cbAvaliacaoCurso', 'cd_tipo_avaliacao', 'dc_tipo_avaliacao');
                    loadSelect(result.cursos, 'cbCurso', 'cd_curso', 'no_curso');
                    loadSelect(result.turmas, 'cbTurma', 'cd_turma', 'no_turma');
                    loadSelect(result.turmas, 'turma', 'cd_turma', 'no_turma');
                    loadSelect(result.funcionarios, 'cbAvaliador', 'cd_pessoa', 'no_pessoa');
                } else
                    apresentaMensagem('apresentadorMensagemAvaliacaoTurma', data);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemAvaliacaoTurma', error.response.data);
        });
    });
}

function loadTipoCoceitoNota(Memory, id) {
    require(["dojo/store/Memory"],
     function (Memory) {
         try{
             var stateStore = new Memory({
                 data: [
                        // { name: "Todos", id: 0 },
                         { name: "Conceito", id: 1 },
                         { name: "Notas", id: 2 }
                 ]
             });
             id.store = stateStore;
             id._onChangeActive = false;
             id.set("value", 0);
             id._onChangeActive = true;
         }
         catch (e) {
             postGerarLog(e);
         }
     });
}

function loadVisao(Memory, id) {
    require(["dojo/store/Memory"],
     function (Memory) {
         try{
             var stateStore = new Memory({
                 data: [
                         { name: "Avaliações por Alunos", id: 1 },
                         { name: "Alunos por Avaliações", id: 2 }
                 ]
             });
             id.store = stateStore;
             id._onChangeActive = false;
             id.set("value", 2);
             id._onChangeActive = true;
         }
         catch (e) {
             postGerarLog(e);
         }
     });
}

function destroyCreateGridNotas() {
    try {
        if (hasValue(dijit.byId("gridNota"))) {
            dijit.byId("gridNota").destroy();
            $('<div>').attr('id', 'gridNota').appendTo('#paiGridNota');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

// restaura dados
function keepValues(grid, xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr) {
    try {
        var valorCacelamento = grid.selection.getSelected();
        
        clearForm('#formAvaliacaoTurma');
        getLimpar('#formAvaliacaoTurma');

        var visao = dijit.byId('visao');
        visao._onChangeActive = false;
        visao.reset();
        visao.set('value', 2);
        visao.old_value = 2;
        visao._onChangeActive = true;

        var value = grid.itemSelecionado;

        dojo.byId('cd_turma').value = value.cd_turma;
        dojo.byId('turma').value = value.no_turma;
        dojo.byId("produto").value = value.no_produto;
        dojo.byId("curso").value = value.no_curso;
        dojo.byId("cd_produto").value = value.cd_produto;
        dojo.byId("cd_curso").value = value.cd_curso;

        var tipoAvaliacao = value.isConceito;
        var dijitTipoAvaliacao = dijit.byId("tipoAvaliacao");
        dijitTipoAvaliacao._onChangeActive = false;
        if (hasValue(dojo.byId('tipoAvaliacao').value, true)) {
            if (!tipoAvaliacao) 
                dijitTipoAvaliacao.set("value", NOTA)
            else
                dijitTipoAvaliacao.set("value", CONCEITO);
        }
        dijitTipoAvaliacao._onChangeActive = true;

        loagGridNota(value.cd_turma, xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, dijitTipoAvaliacao.get('value'), true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function returnCriteriosByTipoAvaliacao(cd_tipo_avaliacao) {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getNomesCriterio?cd_tipo_avaliacao=" + cd_tipo_avaliacao,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataNomes) {
            try{
                var result = $.parseJSON(dataNomes).retorno;
                loadSelect(result, 'cbNomes', 'cd_criterio_avaliacao', 'dc_criterio_avaliacao');
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
//#endregion

//#region criação da grade
function montarCadastroAvalicaoTurma() {
    require([
          "dojo/_base/xhr",
          "dojo/dom",
          "dijit/registry",
          "dojox/grid/EnhancedGrid",
          "dojox/grid/enhanced/plugins/Pagination",
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory",
          "dojo/query",
          "dojo/dom-attr",
          "dijit/form/Button",
          "dojo/ready",
          "dijit/form/DropDownButton",
          "dijit/DropDownMenu",
          "dijit/MenuItem",
          "dojo/domReady!",
          "dojo/parser",
          "dijit/form/FilteringSelect",
          "dijit/Dialog"
    ], function (xhr, dom, _registry, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, domAttr, Button,
                 ready, DropDownButton, DropDownMenu, MenuItem, FilteringSelect) {
        ready(function () {
            try {
                //findAllEmpresasUsuarioComboFiltroEscolaTelaAvaliacao();
                dojo.byId('trEscolaFiltroTelaAvaliacao').style.display = "none";
                //apresentador de messagem especifico da fk turma, necessario pois as mensagens de erro devem ser apresentadas na procura.
                dojo.byId("apresentadorMensagemFks").value = "apresentadorMensagemTurmaFK";
                returnDataTurma();
                myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/turma/getSearchAvaliacaoTurma?idTurma=0&idTipoAvaliacao=0&cd_tipo_avaliacao=0&cd_criterio_avaliacao=0&cd_curso=0&cd_funcionario=0&dta_inicial&dta_final" + "&cd_escola_combo=0",
                    //+ (dijit.byId("escolaFiltroTelaAvaliacao").value || dojo.byId("_ES0").value),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_avaliacao_turma"
                }), Memory({ idProperty: "cd_avaliacao_turma" }));
                var gridAvaliacaoTurma = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosAvaliacaoTurma' style='display:none'/>", field: "selecionadoAvaliacaoTurma", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAvaliacaoTurma },
                       // { name: "Código", field: "cd_avaliacao_turma", width: "5%", styles: "text-align: right; min-width:60px; max-width:75px;" },
                        { name: "Turma", field: "no_turma", width: "65%", minwidth: "10%" },
                        { name: "Tipo", field: "no_tipo_criterio", width: "30%", minwidth: "10%" }
                      ],
                    noDataMessage: msgNotRegEncFiltro,
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
                }, "gridAvaliacaoTurma");
                gridAvaliacaoTurma.pagination.plugin._paginator.plugin.connect(gridAvaliacaoTurma.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridAvaliacaoTurma, 'cd_avaliacao', 'selecionaTodosAvaliacao');
                });
                gridAvaliacaoTurma.canSort = function (col) { return Math.abs(col) != 1; };
                gridAvaliacaoTurma.startup();
                gridAvaliacaoTurma.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                        item = this.getItem(idx);
                        dojo.byId('isConceitoNota').value = item.is_conceito_nota;
                        dojo.byId('isConceito').value = item.isConceito;
                        dojo.byId('isInFocus').value = item.isInFocus;
                        //dijit.byId('visao').set("disabled", item.isConceito);

                        dijit.byId('salvarAvaliacaoTurma').set("disabled", false);
                        dijit.byId('gridAvaliacaoTurma').itemSelecionado = item;
                        montarCadastroEdicaoAvaliacaoTurma(dijit.byId('gridAvaliacaoTurma'), xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                var gridMensagemAluno = new EnhancedGrid({
                    //store: dataStore = dojo.data.ObjectStore({ objectStore: null }),
                    structure:
                        [
                            { name: "<input id='selecionaTodosMensagemAluno' style='display:none'/>", field: "selecionadoMensagemAluno", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMensagemAluno },
                            { name: "Mensagem", field: "tx_mensagem_avaliacao", width: "85%", minwidth: "10%" },
                            { name: "Ativo", field: "mensagemAtiva", width: "10%", styles: "min-width:80px;text-align:center;" }
                        ],
                    noDataMessage: msgNotRegEnc
                }, "gridMensagemAluno");
                gridMensagemAluno.canSort = function (col) { return Math.abs(col) != 1; };
                gridMensagemAluno.startup();

                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        myStore = Cache(
                            JsonRest({
                                target: Endereco() + "/api/coordenacao/getMensagemAvaliacaoSearch?desc=&inicio=false&status=1&produto=" + dojo.byId('cd_produto_CAD').value + "&curso=" + dojo.byId('cd_curso_CAD').value,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                idProperty: "cd_mensagem_avaliacao"
                            }), Memory({ idProperty: "cd_mensagem_avaliacao" }));
                        dataStore = new ObjectStore({ objectStore: myStore });
                        dijit.byId("gridMensagemAluno").setStore(dataStore);
                        dijit.byId("gridMensagemAluno").update();
                        incluirMensagemAluno();
                    }
                }, "btIncluirMsgAluno");

                new Button({
                    label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconWikiword',
	                onClick: function () {
		                myStore = Cache(
			                JsonRest({
				                target: Endereco() + "/api/coordenacao/getMensagemAvaliacaoSearch?desc=&inicio=false&status=1&produto=" + dojo.byId('cd_produto_CAD').value + "&curso=" + dojo.byId('cd_curso_CAD').value,
				                handleAs: "json",
				                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
				                idProperty: "cd_mensagem_avaliacao"
			                }), Memory({ idProperty: "cd_mensagem_avaliacao" }));
		                dataStore = new ObjectStore({ objectStore: myStore });
		                dijit.byId("gridMensagemAluno").setStore(dataStore);
		                dijit.byId("gridMensagemAluno").update();
		                incluirMensagemAluno();
	                }
                }, "btAlterarMsgPadraoAluno");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        alterarMensagemAluno();
                    }
                }, "btAlterarMsgAluno");

                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    onClick: function () {
                        limparMensagemAluno();
                    }
                }, "btLimparMsgAluno");

                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconUndo',
                    onClick: function () {
                        cancelarMensagemAluno();
                    }
                }, "btCancelarMsgAluno");

                //Metodos para criação do link
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        eventoEditarAvaliacaoTurma(dijit.byId('gridAvaliacaoTurma').itensSelecionados, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr);
                    }
                });

                menu.addChild(acaoEditar);

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
                        buscarTodosItens(gridAvaliacaoTurma, 'todosItens', ['pesquisarAvaliacaoTurma', 'relatorioAvaliacaoTurma']);
                        searchAvaliacaoTurma(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () {
                        buscarItensSelecionados('gridAvaliacaoTurma', 'selecionadoAvaliacaoTurma', 'cd_avaliacao_turma', 'selecionaTodosAvaliacaoTurma', ['pesquisarAvaliacaoTurma', 'relatorioAvaliacaoTurma'], 'todosItens');
                    }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dojo.byId("linkSelecionados").appendChild(button.domNode);

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        editarAvaliacaoTurma();
                    }
                }, "salvarAvaliacaoTurma");

                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        try {
                            dojo.byId("cd_turma").value = '';
                            clearForm('#formAvaliacaoTurma');
                            getLimpar('#formAvaliacaoTurma');
                            destroyCreateGridNotas();
                            setIsConceitoNota(false);
                            dijit.byId('cadProTurmaFK').set('disabled', false);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparAvaliacaoTurma");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cadAvaliacaoTurma").hide();
                    }
                }, "fecharAvaliacaoTurma");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        alterarAvaliacaoGrid(dijit.byId("gridOrdenacao"));
                    }
                }, "alterarAvaliacaoTurma");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        // keepValues(null, gridAvaliacao, false);
                    }
                }, "cancelarAvaliacaoTurma");

                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            DeletarAvaliacaoTurma();
                        });
                    }
                }, "deleteAvaliacaoTurma");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        searchAvaliacaoTurma(true);
                    }
                }, "pesquisarAvaliacaoTurma");
                decreaseBtn(document.getElementById("pesquisarAvaliacaoTurma"), '32px');

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        searchMensagemAluno();
                    }
                }, "pesquisarMensagem");
                decreaseBtn(document.getElementById("pesquisarMensagem"), '32px');

                new Button({
                    label: "Limpar", iconClass: '', disabled: true, type: "reset", onClick: function () {
                        dojo.byId("cd_turma_pesquisa").value = 0;
                        dojo.byId("cbTurma").value = "";
                        dijit.byId('limparProTurmaFK').set("disabled", true);
                        apresentaMensagem('apresentadorMensagem', null);
                    }
                }, "limparProTurmaFK");

                if (hasValue(document.getElementById("limparProTurmaFK"))) {
                    document.getElementById("limparProTurmaFK").parentNode.style.minWidth = '40px';
                    document.getElementById("limparProTurmaFK").parentNode.style.width = '40px';
                }

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try {
                            showCarregando();
							dojo.byId("cd_turma").value = '';
                            dojo.byId('isUpdate').value = false;
                            dijit.byId('turma').set('disabled', true);
                            dijit.byId('turma').reset();

                            var dijitTipoAvaliacao = dijit.byId('tipoAvaliacao');
                            dijitTipoAvaliacao._onChangeActive = false;
                            dijitTipoAvaliacao.set('disabled', false);
                            dijitTipoAvaliacao.reset();
                            dijitTipoAvaliacao.set('values', 0);
                            dijitTipoAvaliacao._onChangeActive = true;

                            apresentaMensagem('apresentadorMensagemAvaliacaoTurma', null);
                            dijit.byId("salvarAvaliacaoTurma").set('disabled', true);
                            destroyCreateGridNotas();
                            returnDataTurma();
                            setIsConceitoNota(false);
                            dijit.byId('cadProTurmaFK').set('disabled', false);
                            dijit.byId("cadAvaliacaoTurma").show();
                            showCarregando();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novaAvaliacaoTurma");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        require(["dojo/_base/xhr"], function (xhr) {
                            try{
                                var requisicao = "/api/turma/getUrlRelatorioAvaliacaoTurma?" + getStrGridParameters('gridAvaliacaoTurma') + "idTurma=";
                                var idTurma = hasValue(dijit.byId("cbTurma").value) ? dijit.byId("cbTurma").value : 0;
                                var idTipoAvaliacao = hasValue(dijit.byId("cbTipoAvaliacao").value) ? dijit.byId("cbTipoAvaliacao").value : 0;// nota ou conceito
                                var cbTipoAvaliacaoCurso = hasValue(dijit.byId("cbAvaliacaoCurso").value) ? dijit.byId("cbAvaliacaoCurso").value : 0;
                                var idCriterioAvaliacao = hasValue(dijit.byId("cbNomes").value) ? dijit.byId("cbNomes").value : 0;
                                var idCurso = hasValue(dijit.byId("cbCurso").value) ? dijit.byId("cbCurso").value : 0;
                                var idFuncionario = hasValue(dijit.byId("cbAvaliador").value) ? dijit.byId("cbAvaliador").value : 0;
                                var dtaInicial = hasValue(dojo.byId("dtaIni").value) ? dojo.date.locale.parse(dojo.byId("dtaIni").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                                var dtaFinal = hasValue(dojo.byId("dtaFim").value) ? dojo.date.locale.parse(dojo.byId("dtaFim").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                                xhr.get({
                                    url: Endereco() + requisicao + idTurma + "&idTipoAvaliacao=" + idTipoAvaliacao + "&cd_tipo_avaliacao=" + cbTipoAvaliacaoCurso + "&cd_criterio_avaliacao=" + idCriterioAvaliacao + "&cd_curso=" + idCurso + "&cd_funcionario=" + idFuncionario + "&dta_inicial=" + dtaInicial + "&dta_final=" + dtaFinal + "&cd_escola_combo=0",
                                    //+ (dijit.byId("escolaFiltroTelaAvaliacao").value || dojo.byId("_ES0").value),
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (data) {
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '815px', '750px', 'popRelatorio');
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
                }, "relatorioAvaliacaoTurma");

            new Button({
                label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                    if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                        montarGridPesquisaTurmaFK(function () {
                            abrirTurmaFK();
                            dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                    montarGridPesquisaAluno(false, function () {
                                        abrirAlunoFKTurmaFK(true);
                                    });
                                }
                                else
                                    abrirAlunoFKTurmaFK(true);
                            });
                        });
                    else
                        abrirTurmaFK();
                }
            }, "pesProTurmaFK");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                            montarGridPesquisaTurmaFK(function () {
                                abrirTurmaFKCadastro();
                                dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                    if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                        montarGridPesquisaAluno(false, function () {
                                            abrirAlunoFKTurmaFK(true);
                                        });
                                    }
                                    else
                                        abrirAlunoFKTurmaFK(true);
                                });
                            });
                        else
                            abrirTurmaFKCadastro();
                    }
                }, "cadProTurmaFK");

                btnPesquisar(dojo.byId("pesProTurmaFK"));
                btnPesquisar(dojo.byId("cadProTurmaFK"));
                //******************************inicio***************************\\

                //Atachando eventos
                dijit.byId('cadAvaliacaoTurma').on('show', function () {
                    if (hasValue(dijit.byId('gridNota'))) dijit.byId('gridNota').update();
                });

                dijit.byId("tipoAvaliacao").on("change", function (e) {
                    try {
                        apresentaMensagem('apresentadorMensagemAvaliacaoTurma', null);
                        var turma = parseInt(dojo.byId("cd_turma").value);
                        var dijitVisao = dijit.byId('visao');

                        dijitVisao._onChangeActive = false;
                        dijitVisao.set('value', 2);
                        dijitVisao.old_value = 2;
                        dijitVisao._onChangeActive = true;

                        if (hasValue(e > 0) && turma > 0) {
                            showCarregando();
                            loagGridNota(turma, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, e, true);
                            dijit.byId("salvarAvaliacaoTurma").set('disabled', false);
                        }
                        if (hasValue(e <= 0)) {
                            destroyCreateGridNotas();
                            dijit.byId("salvarAvaliacaoTurma").set('disabled', true);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("btOKNota").on("click", function (e) {
                    try{
                        var idNota = dojo.byId('idNota').value;
                        var isPai = dojo.byId('isPai').value;
                        var idPai = dojo.byId('idPaiNota').value;
                        var descricao = dijit.byId('textAreaObs').value;
                        var botaoIMG = dojo.byId('botaoClick').value;
                        var gridNota = hasValue(dijit.byId("gridNota")) ? dijit.byId("gridNota") : 0;
                        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
                        if (visao == VISAO_AVALIACAO) {
                            for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {//1° primeiro nível da visão de avaliação
                                for (var p = 0; p < gridNota.store._arrayOfTopLevelItems[i].children.length; p++) {//2° primeiro nível da visão de avaliação
                                    if (isPai == "false") {
                                        if (gridNota.store._arrayOfTopLevelItems[i].children[p].id[0] == parseInt(idPai)) {
                                            for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children[p].children.length; j++) {
                                                if (gridNota.store._arrayOfTopLevelItems[i].children[p].children[j].id[0] == parseInt(idNota)) {
                                                    gridNota.store._arrayOfTopLevelItems[i].children[p].children[j].dc_observacao[0] = descricao;
                                                    if (gridNota.store._arrayOfTopLevelItems[i].children[p].children[j].dc_observacao[0] != "")
                                                        dijit.byId(botaoIMG).set('iconClass', 'dijitEditorIcon dijitEditorIconNewSGF');
                                                    else dijit.byId(botaoIMG).set('iconClass', 'dijitEditorIcon dijitEditorIconNewPage');
                                                    break;
                                                }
                                            }//end for
                                            break;
                                        }
                                    }
                                    else {
                                        if (gridNota.store._arrayOfTopLevelItems[i].children[p].pai[0] == 1 && gridNota.store._arrayOfTopLevelItems[i].children[p].id[0] == parseInt(idNota)) {
                                            gridNota.store._arrayOfTopLevelItems[i].children[p].dc_observacao[0] = descricao;
                                            if (gridNota.store._arrayOfTopLevelItems[i].children[p].dc_observacao[0] != "")
                                                dijit.byId(botaoIMG).set('iconClass', 'dijitEditorIcon dijitEditorIconNewSGF');
                                            else dijit.byId(botaoIMG).set('iconClass', 'dijitEditorIcon dijitEditorIconNewPage');
                                            break;
                                        }
                                    }
                                }
                            }
                        }// end if VISAO_AVALIACAO
                        else {
                            if (isPai == "false")
                                for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++)
                                    for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++)
                                        for (var l = 0; l < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; l++)
                                            if (gridNota.store._arrayOfTopLevelItems[i].children[j].id[0] == idPai)
                                                if (gridNota.store._arrayOfTopLevelItems[i].children[j].children[l].id[0] == parseInt(idNota)) {
                                                    gridNota.store._arrayOfTopLevelItems[i].children[j].children[l].dc_observacao[0] = descricao;
                                                    if (gridNota.store._arrayOfTopLevelItems[i].children[j].children[l].dc_observacao[0] != "")
                                                        dijit.byId(botaoIMG).set('iconClass', 'dijitEditorIcon dijitEditorIconNewSGF');
                                                    else dijit.byId(botaoIMG).set('iconClass', 'dijitEditorIcon dijitEditorIconNewPage');
                                                    break;
                                                }//end if
                        }//end else
                        dijit.byId('dialogGrid').hide();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("visao").on("change", function (e) {
                    try {
                        if (e != dijit.byId("visao").old_value) {
                            var turma = parseInt(dojo.byId("cd_turma").value);
                            var isConceitoNota = dojo.byId('isConceitoNota').value == 'true';
                            var isConceito = dojo.byId('isConceito').value;
                            var tipo = hasValue(dijit.byId("tipoAvaliacao")) ? dijit.byId("tipoAvaliacao").value : 0;
                            if (hasValue(dijit.byId('gridNota'))) armazenaDadosGridNota(false);
                            //if (e == VISAO_AVALIACAO && isConceito == 'true') salvarAvaliacao();//Como vai ser carregado do banco novamente devido ao erro salvar para pegar as notas porventura digitadas
                            if (!isConceitoNota) {
                                if (hasValue(turma) > 0) {
                                    showCarregando();
                                    loagGridNota(turma, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, tipo, false) //e == VISAO_AVALIACAO);
                                }
                            }
                            else
                                if (hasValue(turma) > 0 && hasValue(tipo) > 0) {
                                    showCarregando();
                                    loagGridNota(turma, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, tipo, false);
                                }
                            dijit.byId("visao").old_value = e;
                            //dijit.byId('gridNota').startup();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                //fim  eventos
                //Carregando dropDowns;
                loadTipoCoceitoNota(Memory, dijit.byId("tipoAvaliacao"));
                loadVisao(Memory, dijit.byId("visao"));
                montarLegenda();
                
                criarOuCarregarCompFiltering('cbTipoAvaliacao', NotaConceito, "width: 100%;", null, ready, Memory, FilteringSelect, 'id', 'name', 2);

                dijit.byId("cbAvaliacaoCurso").on("change", function (e) {
                    dijit.byId("cbNomes").reset();
                    dijit.byId("cbNomes").store.data = null;
                    if (e > 0) dijit.byId("cbNomes").set('disabled', false);
                    else dijit.byId("cbNomes").set('disabled', true);
                    returnCriteriosByTipoAvaliacao(e);
                });

                //dijit.byId("pesquisarTurmaFK").on("click", function (e) {
                //    var tipoOrigem = dojo.byId("idOrigemCadastro").value;
                //    if (hasValue(tipoOrigem) && tipoOrigem == AVALIACAOTURMA) {
                //        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                //        pesquisarTurmaFK();
                //    }
                //});

                dijit.byId("tipoTurmaFK").on("change", function (e) {
                    if (this.displayedValue == "Personalizada") {
                        dijit.byId("pesTurmasFilhasFK").set("checked", true);
                        dijit.byId('pesTurmasFilhasFK').set('disabled', true);
                    } else {
                        dijit.byId("pesTurmasFilhasFK").set("checked", false);
                        dijit.byId('pesTurmasFilhasFK').set('disabled', false);
                    }
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323055', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['cbAvaliacaoCurso', 'cbNomes', 'cbCurso', 'cbTurma', 'cbTipoAvaliacao', 'cbAvaliador', 'dtaIni', 'dtaFim'], 'pesquisarAvaliacaoTurma', ready);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}
//#endregion

//#region métodos para crud

//#region armazenaDadosGridNota
function searchMensagemAluno() {
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Cache, Memory) {
        try {
            var mensagemAvaliacao = {};

            if (!hasValue(document.getElementById("descFK").value)) {
                mensagemAvaliacao = {
                    desc: "",
                    inicio: document.getElementById("ckInicioMsg").checked,
                    status: 1,
                    produto: dojo.byId("cd_produto_FK").value,
                    curso: dojo.byId("cd_curso_FK").value
                };
            } else {
                mensagemAvaliacao = {
                    desc: encodeURIComponent(document.getElementById("descFK").value),
                    inicio: document.getElementById("ckInicioMsg").checked,
                    status: 1,
                    produto: dojo.byId("cd_produto_FK").value,
                    curso: dojo.byId("cd_curso_FK").value
                }

            }

            xhr.get({
                url: Endereco() + "/api/coordenacao/getMensagemAvaliacaoSearch",
                preventCache: true,
                content: mensagemAvaliacao,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataMensagemAvaliacao) {

                var gridMensagemAluno = dijit.byId("gridMensagemAluno");

                gridMensagemAluno.itensSelecionados = [];

                gridMensagemAluno.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: dataMensagemAvaliacao }) }));
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function armazenaDadosGridNota(arvore) {
    try{
        var retorno = new Array();
        var dadosGridNotaDijit = dijit.byId('gridNota').store._arrayOfTopLevelItems;

        // guarda o histórico para ser usado no retorno
        var historico = dijit.byId('gridNota').historico;
        for (var i = 0; i < dadosGridNotaDijit.length; i++) {
            if (dadosGridNotaDijit[i].cd_tipo_avaliacao[0] > 0) {
                retorno[i] = {
                    cd_tipo_avaliacao: dadosGridNotaDijit[i].cd_tipo_avaliacao,
                    dc_nome: dadosGridNotaDijit[i].dc_nome,
                    dc_tipo_avaliacao: dadosGridNotaDijit[i].dc_tipo_avaliacao,
                    id: dadosGridNotaDijit[i].id,
                    idPai: dadosGridNotaDijit[i].idPai,
                    pai: dadosGridNotaDijit[i].pai
                }

                var novoChildrenPrimeiroNivel = new Array();
                for (var j = 0; j < dadosGridNotaDijit[i].children.length; j++) {
                    novoChildrenPrimeiroNivel[j] = {
                        participacoesAluno: dadosGridNotaDijit[i].children[j].participacoesAluno[0],
                        participacoesDisponiveis: dadosGridNotaDijit[i].children[j].participacoesDisponiveis[0],
                        id_participacao: dadosGridNotaDijit[i].children[j].id_participacao[0],
                        ativo: dadosGridNotaDijit[i].children[j].ativo[0],
                        cd_aluno: dadosGridNotaDijit[i].children[j].cd_aluno[0],
                        cd_avaliacao: dadosGridNotaDijit[i].children[j].cd_avaliacao[0],
                        cd_avaliacao_aluno: dadosGridNotaDijit[i].children[j].cd_avaliacao_aluno[0],
                        cd_avaliacao_turma: dadosGridNotaDijit[i].children[j].cd_avaliacao_turma[0],
                        cd_conceito: dadosGridNotaDijit[i].children[j].cd_conceito[0],
                        cd_criterio_avaliacao: dadosGridNotaDijit[i].children[j].cd_criterio_avaliacao[0],
                        cd_funcionario: dadosGridNotaDijit[i].children[j].cd_funcionario[0],
                        cd_produto: dadosGridNotaDijit[i].children[j].cd_produto[0],
                        cd_tipo_avaliacao: dadosGridNotaDijit[i].children[j].cd_tipo_avaliacao[0],
                        dc_avaliacao_turma: dadosGridNotaDijit[i].children[j].dc_avaliacao_turma[0],
                        dc_conceito: dadosGridNotaDijit[i].children[j].dc_conceito[0],
                        dc_nome: dadosGridNotaDijit[i].children[j].dc_nome[0],
                        dc_nome_avaliador: dadosGridNotaDijit[i].children[j].dc_nome_avaliador[0],
                        dc_observacao: dadosGridNotaDijit[i].children[j].dc_observacao[0],
                        dc_observacao_aux: dadosGridNotaDijit[i].children[j].dc_observacao_aux[0],
                        dt_cadastro: dadosGridNotaDijit[i].children[j].dt_cadastro[0],
                        dt_avaliacao_turma: dadosGridNotaDijit[i].children[j].dt_avaliacao_turma[0],
                        dta_avaliacao_turma: dadosGridNotaDijit[i].children[j].dta_avaliacao_turma[0],
                        dt_desistencia: dadosGridNotaDijit[i].children[j].dt_desistencia[0],
                        dta_desistencia: dadosGridNotaDijit[i].children[j].dta_desistencia[0],
                        dt_matricula: dadosGridNotaDijit[i].children[j].dt_matricula[0],
                        dta_matricula: dadosGridNotaDijit[i].children[j].dt_matricula[0],
                        dt_movimento: dadosGridNotaDijit[i].children[j].dt_movimento[0],
                        dta_movimento: dadosGridNotaDijit[i].children[j].dta_movimento[0],
                        dt_transferencia: dadosGridNotaDijit[i].children[j].dt_transferencia[0],
                        dta_transferencia: dadosGridNotaDijit[i].children[j].dta_transferencia[0],
                        dta_termino_turma: dadosGridNotaDijit[i].children[j].dta_termino_turma[0],
                        id: dadosGridNotaDijit[i].children[j].id[0],
                        idPai: dadosGridNotaDijit[i].children[j].idPai[0],
                        id_segunda_prova: dadosGridNotaDijit[i].children[j].id_segunda_prova[0],
                        isConceito: dadosGridNotaDijit[i].children[j].isConceito[0],
                        isConceitoNota: dadosGridNotaDijit[i].children[j].isConceitoNota[0],
                        isModified: dadosGridNotaDijit[i].children[j].isModified[0],
                        isModifiedA: dadosGridNotaDijit[i].children[j].isModifiedA[0],
                        maximoNotaTurma: dadosGridNotaDijit[i].children[j].maximoNotaTurma[0],
                        mediaNotas: dadosGridNotaDijit[i].children[j].mediaNotas[0],
                        notaMaxima: dadosGridNotaDijit[i].children[j].notaMaxima[0],
                        pai: dadosGridNotaDijit[i].children[j].pai[0],
                        somaNotas: dadosGridNotaDijit[i].children[j].somaNotas[0],
                        vl_nota: dadosGridNotaDijit[i].children[j].vl_nota[0],
                        isChildren: dadosGridNotaDijit[i].children[j].isChildren,
                        peso: dadosGridNotaDijit[i].children[j].peso,
                        vl_nota_2: dadosGridNotaDijit[i].children[j].vl_nota_2[0]
                    }
                    retorno[i].children = novoChildrenPrimeiroNivel;

                    var novoChildrenSegundoNivel = new Array();
                    for (var k = 0; k < dadosGridNotaDijit[i].children[j].children.length; k++) {
                        novoChildrenSegundoNivel[k] = {
                            participacoesAluno: dadosGridNotaDijit[i].children[j].children[k].participacoesAluno[0],
                            participacoesDisponiveis: dadosGridNotaDijit[i].children[j].children[k].participacoesDisponiveis[0],
                            id_participacao: dadosGridNotaDijit[i].children[j].children[k].id_participacao[0],
                            ativo: dadosGridNotaDijit[i].children[j].children[k].ativo[0],
                            cd_aluno: dadosGridNotaDijit[i].children[j].children[k].cd_aluno[0],
                            cd_avaliacao: dadosGridNotaDijit[i].children[j].children[k].cd_avaliacao[0],
                            cd_avaliacao_aluno: dadosGridNotaDijit[i].children[j].children[k].cd_avaliacao_aluno[0],
                            cd_avaliacao_turma: dadosGridNotaDijit[i].children[j].children[k].cd_avaliacao_turma[0],
                            cd_conceito: dadosGridNotaDijit[i].children[j].children[k].cd_conceito[0],
                            cd_criterio_avaliacao: dadosGridNotaDijit[i].children[j].children[k].cd_criterio_avaliacao[0],
                            cd_funcionario: dadosGridNotaDijit[i].children[j].children[k].cd_funcionario[0],
                            cd_produto: dadosGridNotaDijit[i].children[j].children[k].cd_produto[0],
                            cd_tipo_avaliacao: dadosGridNotaDijit[i].children[j].children[k].cd_tipo_avaliacao[0],
                            dc_avaliacao_turma: dadosGridNotaDijit[i].children[j].children[k].dc_avaliacao_turma[0],
                            dc_conceito: dadosGridNotaDijit[i].children[j].children[k].dc_conceito[0],
                            dc_nome: dadosGridNotaDijit[i].children[j].children[k].dc_nome[0],
                            dc_nome_avaliador: dadosGridNotaDijit[i].children[j].children[k].dc_nome_avaliador[0],
                            dc_observacao: dadosGridNotaDijit[i].children[j].children[k].dc_observacao[0],
                            dc_observacao_aux: dadosGridNotaDijit[i].children[j].children[k].dc_observacao_aux[0],
                            dt_avaliacao_turma: dadosGridNotaDijit[i].children[j].children[k].dt_avaliacao_turma[0],
                            dt_cadastro: dadosGridNotaDijit[i].children[j].children[k].dt_cadastro[0],
                            dt_desistencia: dadosGridNotaDijit[i].children[j].children[k].dt_desistencia[0],
                            dta_desistencia: dadosGridNotaDijit[i].children[j].children[k].dta_desistencia[0],
                            dt_matricula: dadosGridNotaDijit[i].children[j].children[k].dt_matricula[0],
                            dta_matricula: dadosGridNotaDijit[i].children[j].children[k].dta_matricula[0],
                            dt_movimento: dadosGridNotaDijit[i].children[j].children[k].dt_movimento[0],
                            dta_movimento: dadosGridNotaDijit[i].children[j].children[k].dta_movimento[0],
                            dt_transferencia: dadosGridNotaDijit[i].children[j].children[k].dt_transferencia[0],
                            dta_transferencia: dadosGridNotaDijit[i].children[j].children[k].dta_transferencia[0],
                            dta_termino_turma: dadosGridNotaDijit[i].children[j].children[k].dta_termino_turma[0],
                            id: dadosGridNotaDijit[i].children[j].children[k].id[0],
                            idPai: dadosGridNotaDijit[i].children[j].children[k].idPai[0],
                            id_segunda_prova: dadosGridNotaDijit[i].children[j].children[k].id_segunda_prova[0],
                            isConceito: dadosGridNotaDijit[i].children[j].children[k].isConceito[0],
                            isConceitoNota: dadosGridNotaDijit[i].children[j].children[k].isConceitoNota[0],
                            isModified: dadosGridNotaDijit[i].children[j].children[k].isModified[0],
                            isModifiedA: dadosGridNotaDijit[i].children[j].children[k].isModifiedA[0],
                            maximoNotaTurma: dadosGridNotaDijit[i].children[j].children[k].maximoNotaTurma[0],
                            mediaNotas: dadosGridNotaDijit[i].children[j].children[k].mediaNotas[0],
                            notaMaxima: dadosGridNotaDijit[i].children[j].children[k].notaMaxima[0],
                            pai: dadosGridNotaDijit[i].children[j].children[k].pai[0],
                            somaNotas: dadosGridNotaDijit[i].children[j].children[k].somaNotas[0],
                            vl_nota: dadosGridNotaDijit[i].children[j].children[k].vl_nota[0],
                            vl_nota_2: dadosGridNotaDijit[i].children[j].children[k].vl_nota_2[0],
                            peso: dadosGridNotaDijit[i].children[j].children[k].peso[0],
                            isChildren: dadosGridNotaDijit[i].children[j].children[k].isChildren
                        }
                        retorno[i].children[j].children = novoChildrenSegundoNivel;
                    }
                }
            } else {
                retorno[i] = {
                    participacoesAluno: dadosGridNotaDijit[i].participacoesAluno[0],
                    participacoesDisponiveis: dadosGridNotaDijit[i].participacoesDisponiveis[0],
                    id_participacao: dadosGridNotaDijit[i].id_participacao[0],
                    ativo: dadosGridNotaDijit[i].ativo[0],
                    cd_aluno: dadosGridNotaDijit[i].cd_aluno[0],
                    cd_avaliacao: dadosGridNotaDijit[i].cd_avaliacao[0],
                    cd_avaliacao_aluno: dadosGridNotaDijit[i].cd_avaliacao_aluno[0],
                    cd_avaliacao_turma: dadosGridNotaDijit[i].cd_avaliacao_turma[0],
                    cd_conceito: dadosGridNotaDijit[i].cd_conceito[0],
                    cd_criterio_avaliacao: dadosGridNotaDijit[i].cd_criterio_avaliacao[0],
                    cd_funcionario: dadosGridNotaDijit[i].cd_funcionario[0],
                    cd_produto: dadosGridNotaDijit[i].cd_produto[0],
                    cd_tipo_avaliacao: dadosGridNotaDijit[i].cd_tipo_avaliacao[0],
                    dc_avaliacao_turma: dadosGridNotaDijit[i].dc_avaliacao_turma[0],
                    dc_conceito: dadosGridNotaDijit[i].dc_conceito[0],
                    dc_nome: dadosGridNotaDijit[i].dc_nome[0],
                    dc_nome_avaliador: dadosGridNotaDijit[i].dc_nome_avaliador[0],
                    dc_observacao: dadosGridNotaDijit[i].dc_observacao[0],
                    dc_observacao_aux: dadosGridNotaDijit[i].dc_observacao_aux[0],
                    dt_avaliacao_turma: dadosGridNotaDijit[i].dt_avaliacao_turma[0],
                    dt_cadastro: dadosGridNotaDijit[i].dt_cadastro[0],
                    dt_desistencia: dadosGridNotaDijit[i].dt_desistencia[0],
                    dta_desistencia: dadosGridNotaDijit[i].dt_desistencia[0],
                    dt_matricula: dadosGridNotaDijit[i].dt_matricula[0],
                    dta_matricula: dadosGridNotaDijit[i].dta_matricula[0],
                    dt_movimento: dadosGridNotaDijit[i].dt_movimento[0],
                    dta_movimento: dadosGridNotaDijit[i].dta_movimento[0],
                    dt_transferencia: dadosGridNotaDijit[i].dt_transferencia[0],
                    dta_transferencia: dadosGridNotaDijit[i].dt_transferencia[0],
                    dta_termino_turma: dadosGridNotaDijit[i].dta_termino_turma[0],
                    id: dadosGridNotaDijit[i].id[0],
                    idPai: dadosGridNotaDijit[i].idPai[0],
                    id_segunda_prova: dadosGridNotaDijit[i].id_segunda_prova[0],
                    isConceito: dadosGridNotaDijit[i].isConceito[0],
                    isConceitoNota: dadosGridNotaDijit[i].isConceitoNota[0],
                    isModified: dadosGridNotaDijit[i].isModified[0],
                    isModifiedA: dadosGridNotaDijit[i].isModifiedA[0],
                    maximoNotaTurma: dadosGridNotaDijit[i].maximoNotaTurma[0],
                    mediaNotas: dadosGridNotaDijit[i].mediaNotas[0],
                    notaMaxima: dadosGridNotaDijit[i].notaMaxima[0],
                    pai: dadosGridNotaDijit[i].pai[0],
                    somaNotas: dadosGridNotaDijit[i].somaNotas[0],
                    vl_nota: dadosGridNotaDijit[i].vl_nota[0],
                    vl_nota_2: dadosGridNotaDijit[i].vl_nota_2[0],
                    isChildren: dadosGridNotaDijit[i].isChildren,
                    peso: dadosGridNotaDijit[i].peso
                }

                var novoChildrenSegundoNivel = new Array();
                for (var k = 0; k < dadosGridNotaDijit[i].children.length; k++) {
                    novoChildrenSegundoNivel[k] = {
                        cd_tipo_avaliacao: dadosGridNotaDijit[i].children[k].cd_tipo_avaliacao,
                        dc_nome: dadosGridNotaDijit[i].children[k].dc_nome,
                        dc_tipo_avaliacao: dadosGridNotaDijit[i].children[k].dc_tipo_avaliacao,
                        id: dadosGridNotaDijit[i].children[k].id,
                        idPai: dadosGridNotaDijit[i].children[k].idPai,
                        pai: dadosGridNotaDijit[i].children[k].pai
                    }
                    retorno[i].children = novoChildrenSegundoNivel;

                    var novoChildrenPrimeiroNivel = new Array();
                    for (var j = 0; j < dadosGridNotaDijit[i].children[k].children.length; j++) {
                        novoChildrenPrimeiroNivel[j] = {
                            participacoesAluno: dadosGridNotaDijit[i].children[k].children[j].participacoesAluno[0],
                            participacoesDisponiveis: dadosGridNotaDijit[i].children[k].children[j].participacoesDisponiveis[0],
                            id_participacao: dadosGridNotaDijit[i].children[k].children[j].id_participacao[0],
                            ativo: dadosGridNotaDijit[i].children[k].children[j].ativo[0],
                            cd_aluno: dadosGridNotaDijit[i].children[k].children[j].cd_aluno[0],
                            cd_avaliacao: dadosGridNotaDijit[i].children[k].children[j].cd_avaliacao[0],
                            cd_avaliacao_aluno: dadosGridNotaDijit[i].children[k].children[j].cd_avaliacao_aluno[0],
                            cd_avaliacao_turma: dadosGridNotaDijit[i].children[k].children[j].cd_avaliacao_turma[0],
                            cd_conceito: dadosGridNotaDijit[i].children[k].children[j].cd_conceito[0],
                            cd_criterio_avaliacao: dadosGridNotaDijit[i].children[k].children[j].cd_criterio_avaliacao[0],
                            cd_funcionario: dadosGridNotaDijit[i].children[k].children[j].cd_funcionario[0],
                            cd_produto: dadosGridNotaDijit[i].children[k].children[j].cd_produto[0],
                            cd_tipo_avaliacao: dadosGridNotaDijit[i].children[k].children[j].cd_tipo_avaliacao[0],
                            dc_avaliacao_turma: dadosGridNotaDijit[i].children[k].children[j].dc_avaliacao_turma[0],
                            dc_conceito: dadosGridNotaDijit[i].children[k].children[j].dc_conceito[0],
                            dc_nome: dadosGridNotaDijit[i].children[k].children[j].dc_nome[0],
                            dc_nome_avaliador: dadosGridNotaDijit[i].children[k].children[j].dc_nome_avaliador[0],
                            dc_observacao: dadosGridNotaDijit[i].children[k].children[j].dc_observacao[0],
                            dc_observacao_aux: dadosGridNotaDijit[i].children[k].children[j].dc_observacao_aux[0],
                            dt_avaliacao_turma: dadosGridNotaDijit[i].children[k].children[j].dt_avaliacao_turma[0],
                            dt_cadastro: dadosGridNotaDijit[i].children[k].children[j].dt_cadastro[0],
                            dt_desistencia: dadosGridNotaDijit[i].children[k].children[j].dt_desistencia[0],
                            dta_desistencia: dadosGridNotaDijit[i].children[k].children[j].dta_desistencia[0],
                            dta_avaliacao_turma: dadosGridNotaDijit[i].children[k].children[j].dta_avaliacao_turma[0],
                            dta_termino_turma: dadosGridNotaDijit[i].children[k].children[j].dta_termino_turma[0],
                            dt_matricula: dadosGridNotaDijit[i].children[k].children[j].dt_matricula[0],
                            dta_matricula: dadosGridNotaDijit[i].children[k].children[j].dta_matricula[0],
                            dt_movimento: dadosGridNotaDijit[i].children[k].children[j].dt_movimento[0],
                            dta_movimento: dadosGridNotaDijit[i].children[k].children[j].dta_movimento[0],
                            dt_transferencia: dadosGridNotaDijit[i].children[k].children[j].dt_transferencia[0],
                            dta_transferencia: dadosGridNotaDijit[i].children[k].children[j].dta_transferencia[0],
                            id: dadosGridNotaDijit[i].children[k].children[j].id[0],
                            idPai: dadosGridNotaDijit[i].children[k].children[j].idPai[0],
                            id_segunda_prova: dadosGridNotaDijit[i].children[k].children[j].id_segunda_prova[0],
                            isConceito: dadosGridNotaDijit[i].children[k].children[j].isConceito[0],
                            isConceitoNota: dadosGridNotaDijit[i].children[k].children[j].isConceitoNota[0],
                            isModified: dadosGridNotaDijit[i].children[k].children[j].isModified[0],
                            isModifiedA: dadosGridNotaDijit[i].children[k].children[j].isModifiedA[0],
                            maximoNotaTurma: dadosGridNotaDijit[i].children[k].children[j].maximoNotaTurma[0],
                            mediaNotas: dadosGridNotaDijit[i].children[k].children[j].mediaNotas[0],
                            notaMaxima: dadosGridNotaDijit[i].children[k].children[j].notaMaxima[0],
                            pai: dadosGridNotaDijit[i].children[k].children[j].pai[0],
                            somaNotas: dadosGridNotaDijit[i].children[k].children[j].somaNotas[0],
                            vl_nota: dadosGridNotaDijit[i].children[k].children[j].vl_nota[0],
                            vl_nota_2: dadosGridNotaDijit[i].children[k].children[j].vl_nota_2[0],
                            isChildren: dadosGridNotaDijit[i].children[k].children[j].isChildren,
                            peso: dadosGridNotaDijit[i].children[k].children[j].peso
                        }//for j
                        retorno[i].children[k].children = novoChildrenPrimeiroNivel;
                    }//for k
                }
            }// else
        }//for

        //cria uma propriedade historico no retorno para ser usado na gridNota e fazer as consistencia da situação do aluno na turma.
        retorno.historico = historico;
        if (arvore)
            dadosGridInicial = retorno
        else
            dadosGridNota = retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

function inverteVisaoNotaAvaliacao(data, tipoAvaliacao) {
    try{
        var novoDataPrimeiroNivel = [];
        var novoDataSegundoNivel = [];
        var isInFocus = dojo.byId('isInFocus').value == 'true';
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        var dadosGridNota = cloneArray(data);
        var cod_aluno = 0;
        if ((data[0] != undefined) && (data[0].cd_aluno != undefined))
            cod_aluno = data[0].cd_aluno;

        if (cod_aluno > 0) {
            for (var i = 0; i < dadosGridNota.length; i++) {
                novoDataPrimeiroNivel = dadosGridNota;
                for (var j = 0; j < dadosGridNota[i].children.length; j++)
                    novoDataSegundoNivel = dadosGridNota[i].children;
            }

            var cloneDataPrimeiroNivel = cloneArray(dadosGridNota);
            for (var i = 0; i < cloneDataPrimeiroNivel.length; i++)
                delete cloneDataPrimeiroNivel[i].children;

            for (var r = 0; r < novoDataSegundoNivel.length; r++) {
                var novosItensSegNivel = cloneArray(cloneDataPrimeiroNivel);
                for (var s = 0; s < novoDataSegundoNivel[r].children.length; s++)
                    novoDataSegundoNivel[r].children[s].children = novosItensSegNivel;
                novoDataSegundoNivel[r].pai = 1;
                novoDataSegundoNivel[r].isChildren = 0;
            }

            //Corrigi o id para a visão do aluno.
            var novodata = clone(novoDataSegundoNivel);
            var contador = 0;
            for (var d = 0; d < novodata.length; d++) {
                novodata[d].id = novodata[d].id + '' + contador;
                for (var e = 0; e < novodata[d].children.length; e++) {
                    novodata[d].children[e].id = novodata[d].children[e].id + '' + contador;
                    novodata[d].children[e].pai = 1
                    novodata[d].children[e].isChildren = 1;
                    for (var f = 0; f < novodata[d].children[e].children.length; f++) {
                        contador++;
                        novodata[d].children[e].children[f].id = novodata[d].children[e].children[f].id + '' + contador;
                        novodata[d].children[e].children[f].pai = 0;
                        novodata[d].children[e].children[f].isChildren = 1

                    }
                }
            }
            var numeroNotas = 0;
            var totalNotas = 0;
            var ultimaAval = "";
            for (var i = 0; i < data.length; i++)
                for (var u = 0; u < data[i].children.length; u++)
                    for (var v = 0; v < data[i].children[u].children.length; v++)
                        for (var d = 0; d < novodata.length; d++)
                            for (var c = 0; c < novodata[d].children.length; c++)
                                for (var e = 0; e < novodata[d].children[c].children.length; e++) {
                                    if (data[i].cd_avaliacao == 0 //Significa que a visão original tem o nível pai como avaliação:
                                          && data[i].cd_aluno == novodata[d].children[c].children[e].cd_aluno
                                          && data[i].children[u].children[v].cd_avaliacao == novodata[d].children[c].cd_avaliacao) {
                                        novodata[d].children[c].children[e].dc_observacao = data[i].children[u].children[v].dc_observacao;
                                        novodata[d].children[c].children[e].id_segunda_prova = data[i].children[u].children[v].id_segunda_prova;
                                        novodata[d].children[c].children[e].cd_avaliacao_turma = data[i].children[u].children[v].cd_avaliacao_turma;
                                        novodata[d].children[c].children[e].vl_nota = data[i].children[u].children[v].vl_nota;
                                        novodata[d].children[c].children[e].cd_avaliacao_aluno = data[i].cd_avaliacao_aluno;
                                        novodata[d].children[c].children[e].dt_movimento = data[i].dt_movimento;
                                        novodata[d].children[c].children[e].dt_desistencia = data[i].dt_desistencia;
                                        novodata[d].children[c].children[e].dt_transferencia = data[i].dt_transferencia;
                                        novodata[d].children[c].dt_avaliacao_turma = data[i].children[u].children[v].dt_avaliacao_turma;
                                        novodata[d].children[c].dt_termino_turma = data[i].children[u].children[v].dt_termino_turma;
                                        if(!isInFocus)
                                            novodata[d].children[c].mediaNotas = data[i].children[u].children[v].mediaNotas;
                                        else
                                            novodata[d].children[c].mediaNotas = 0;
                                        novodata[d].children[c].children[e].dt_matricula = data[i].dt_matricula;
                                        novodata[d].children[c].dc_observacao = data[i].children[u].children[v].dc_observacao_aux;
                                        novodata[d].children[c].children[e].participacoesAluno = data[i].children[u].children[v].participacoesAluno;
                                        novodata[d].children[c].children[e].participacoesDisponiveis = data[i].children[u].children[v].participacoesDisponiveis;
                                        novodata[d].children[c].children[e].id_participacao = data[i].children[u].children[v].id_participacao;
                                        //novodata[d].children[c].somaNotas = data[i].children[u].children[v].somaNotas;
                                        novoDataSegundoNivel[d].children[c].children[e].somaNotas = data[i].children[u].children[v].vl_nota * data[i].children[u].children[v].peso;
                                        novodata[d].children[c].children[e].isModifiedA = data[i].children[u].children[v].isModifiedA;
                                        if (tipoAvaliacao == NOTA) {
                                            novodata[d].children[c].children[e].vl_nota = data[i].children[u].children[v].vl_nota;
                                            novodata[d].children[c].children[e].vl_nota_2 = data[i].children[u].children[v].vl_nota_2;
                                        }
                                        else {
                                            novodata[d].children[c].children[e].cd_conceito = data[i].children[u].children[v].cd_conceito;
                                            novodata[0].isConceito = true;
                                        }
                                        //Calcula o total das notas:

                                        var notaAval = data[i].children[u].children[v].vl_nota;

                                        if (notaAval >= 0 && notaAval != '' && notaAval != null) {
                                            if (ultimaAval == novodata[d].children[c].dc_nome) {
                                                numeroNotas++;
                                                notaAval = parseFloat(notaAval * data[i].children[u].children[v].peso[0]);
                                                totalNotas += notaAval;
                                                novodata[d].children[c].somaNotas = totalNotas;
                                                ultimaAval = data[i].children[u].dc_tipo_avaliacao[0];
                                            } else {
                                                numeroNotas = 0;
                                                totalNotas = 0;
                                                numeroNotas++;
                                                notaAval = parseFloat(notaAval * data[i].children[u].children[v].peso[0]);
                                                totalNotas += notaAval;
                                                novodata[d].children[c].somaNotas = totalNotas;
                                                ultimaAval = data[i].children[u].children[v].dc_nome;
                                            }

                                        }

                                    }
                                }
            return novodata;
        } else
            return data;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function inverteVisaoNota(data, tipoAvaliacao) {
    try{
        var novoDataPrimeiroNivel = [];
        var novoDataSegundoNivel = [];
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        var dadosGridNota = cloneArray(data);
        var isInFocus = dojo.byId('isInFocus').value == 'true';

        //Pega os filhos do primeiro pai e transforma eles nos novos pais:
        for (var i = 0; i < dadosGridNota.length; i++) {
            novoDataPrimeiroNivel = dadosGridNota;
            for (var j = 0; j < dadosGridNota[i].children.length; j++) {
                novoDataSegundoNivel = dadosGridNota[i].children[j].children;
            //for (var j = 0; j < dadosGridNota[i].children[i].children.length; j++) {
            //    novoDataSegundoNivel = dadosGridNota[i].children[j].children;
            }
        }

        //Remove todos os filhos dos pais do data original para transformá-lo em um vetor de novos filhos:
        var cloneData = cloneArray(dadosGridNota);
        for (var j = 0; j < cloneData.length; j++) {
            for (var n = 0; n < cloneData[j].children.length; n++)
                delete cloneData[j].children[n].children;
            cloneData[j].pai = 0;// deixa de ser pai
            if (cloneData[j].cd_tipo_avaliacao == 0) cloneData[j].isChildren = 1;
        }

        for (var r = 0; r < novoDataSegundoNivel.length; r++) {
            var novosItensSegNivel = cloneArray(cloneData);
            for (var j = 0; j < novosItensSegNivel.length; j++) {
                novosItensSegNivel[j].id = novosItensSegNivel[j].id + '' + r;
            }
            novoDataSegundoNivel[r].children = novosItensSegNivel;
            novoDataSegundoNivel[r].pai = 1;
            if (novoDataSegundoNivel[r].cd_tipo_avaliacao == 0) novoDataSegundoNivel[r].isChildren = 0;
        }

        //Corrigi o id para a visão do aluno.
        var contador = 0;
        for (var d = 0; d < novoDataSegundoNivel.length; d++) {
            novoDataSegundoNivel[d].id = novoDataSegundoNivel[d].id + '' + contador + '' + d;
            for (var e = 0; e < novoDataSegundoNivel[d].children.length; e++) {
                novoDataSegundoNivel[d].children[e].id = novoDataSegundoNivel[d].children[e].id + '' + contador;
                for (var f = 0; f < novoDataSegundoNivel[d].children[e].children.length; f++) {
                    contador++;
                    novoDataSegundoNivel[d].children[e].children[f].id = novoDataSegundoNivel[d].children[e].children[f].id + '' + contador;
                    novoDataSegundoNivel[d].children[e].children[f].pai = 0;
                }
            }
        }

        for (var i = 0; i < data.length; i++)
            for (var u = 0; u < data[i].children.length; u++)
                for (var v = 0; v < data[i].children[u].children.length; v++)
                    for (var d = 0; d < novoDataSegundoNivel.length; d++)
                        for (var c = 0; c < novoDataSegundoNivel[d].children.length; c++)
                            for (var e = 0; e < novoDataSegundoNivel[d].children[c].children.length; e++)
                                if (data[i].children[u].cd_avaliacao != 0 //Significa que a visão original tem o nível pai como aluno:
                                        && data[i].children[u].cd_avaliacao == novoDataSegundoNivel[d].children[c].children[e].cd_avaliacao
                                        && data[i].children[u].children[v].cd_aluno == novoDataSegundoNivel[d].cd_aluno) {
                                    novoDataSegundoNivel[d].children[c].children[e].dc_observacao_aux = data[i].children[u].dc_observacao;
                                    novoDataSegundoNivel[d].children[c].children[e].dt_termino_turma = data[i].children[u].dt_termino_turma;
                                    novoDataSegundoNivel[d].children[c].children[e].dc_observacao = data[i].children[u].children[v].dc_observacao;
                                    novoDataSegundoNivel[d].dt_matricula = data[i].children[u].children[v].dt_matricula;
                                    novoDataSegundoNivel[d].dt_movimento = data[i].children[u].children[v].dt_movimento;
                                    novoDataSegundoNivel[d].dt_desistencia = data[i].children[u].children[v].dt_desistencia;
                                    novoDataSegundoNivel[d].dt_transferencia = data[i].children[u].children[v].dt_transferencia;
                                    novoDataSegundoNivel[d].children[c].children[e].dt_avaliacao_turma = data[i].children[u].dt_avaliacao_turma;
                                    novoDataSegundoNivel[d].children[c].children[e].cd_avaliacao_turma = data[i].children[u].children[v].cd_avaliacao_turma;
                                    novoDataSegundoNivel[d].children[c].children[e].cd_avaliacao_aluno = data[i].children[u].children[v].cd_avaliacao_aluno;
                                    //novoDataSegundoNivel[d].children[c].children[e].mediaNotas = data[i].children[u].vl_nota;
                                    if(!isInFocus)
                                        novoDataSegundoNivel[d].children[c].children[e].somaNotas = data[i].children[u].somaNotas;
                                    else
                                        novoDataSegundoNivel[d].children[c].children[e].somaNotas = 0;
                                    novoDataSegundoNivel[d].children[c].children[e].id_segunda_prova = data[i].children[u].children[v].id_segunda_prova;
                                    novoDataSegundoNivel[d].children[c].children[e].participacoesAluno = data[i].children[u].children[v].participacoesAluno;
                                    novoDataSegundoNivel[d].children[c].children[e].id_participacao = data[i].children[u].children[v].id_participacao;
                                    novoDataSegundoNivel[d].children[c].children[e].participacoesDisponiveis = data[i].children[u].children[v].participacoesDisponiveis;
                                    novoDataSegundoNivel[d].vl_nota = data[i].children[u].children[v].vl_nota;
                                    novoDataSegundoNivel[d].children[c].children[e].isModifiedA = data[i].children[u].children[v].isModifiedA;
                                    if (tipoAvaliacao == NOTA) {
                                        novoDataSegundoNivel[d].children[c].children[e].vl_nota = data[i].children[u].children[v].vl_nota;
                                        novoDataSegundoNivel[d].children[c].children[e].vl_nota_2 = data[i].children[u].children[v].vl_nota_2;
                                    }
                                    else {
                                        novoDataSegundoNivel[d].children[c].children[e].cd_conceito = data[i].children[u].children[v].cd_conceito;
                                        novoDataSegundoNivel[0].isConceito = true;
                                    }
                                }
        //Atualiza a soma das notas para mostrar na visão do aluno:
        if (tipoAvaliacao == NOTA)
            for (var i = 0; i < novoDataSegundoNivel.length; i++) {
                var notaAluno = 0;
                for (var j = 0; j < novoDataSegundoNivel[i].children.length; j++)
                    for (var l = 0; l < novoDataSegundoNivel[i].children[j].children.length; l++) {
                        var valorNota = novoDataSegundoNivel[i].children[j].children[l].vl_nota;
                        var peso = novoDataSegundoNivel[i].children[j].children[l].peso;
                        if (isNaN(valorNota))
                            valorNota = '';
                        if (valorNota != '' && valorNota != null) {
                            valorNota = parseFloat(valorNota * peso);
                            notaAluno += valorNota;
                            novoDataSegundoNivel[i].vl_nota = notaAluno;
                        }
                    }
            }
        return novoDataSegundoNivel;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clearChildrenLenthZero(dataRetorno) {
    try{
        for (var i = 0; i < dataRetorno.length; i++)
            for (var j = 0; j < dataRetorno[i].children.length; j++)
                for (var m = 0; m < dataRetorno[i].children[j].children.length; m++)
                    delete dataRetorno[i].children[j].children[m].children;
        return dataRetorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizaParticipacoesAlunos(dataRetorno) {
    try {
        for (var i = 0; i < dataRetorno.length; i++)
            for (var j = 0; j < dataRetorno[i].children.length; j++){
                for (var m = 0; m < dataRetorno[i].children[j].children.length; m++) {
                    var participacoesDisponiveis = new Array();

                    if(hasValue(dataRetorno[i].children[j].participacoesDisponiveis))
                        for (var n = 0; n < dataRetorno[i].children[j].participacoesDisponiveis.length; n++) {
                            var participacao_selecionada = false; //Participação selecionada parcialmente. Na verdade seta que há uma participação no aluno.
                            var cd_conceito_participacao = 0;
                            var vl_nota_participacao = 0;
                            var existeParticipacaoAnterior = false;
                            for (var o = 0; o < dataRetorno[i].children[j].children[m].participacoesAluno.length; o++) {
                                existeParticipacaoAnterior = true;
                                if (dataRetorno[i].children[j].children[m].participacoesAluno[o].cd_participacao_avaliacao == dataRetorno[i].children[j].participacoesDisponiveis[n].cd_participacao) {
                                    participacao_selecionada = true;
                                    cd_conceito_participacao = dataRetorno[i].children[j].children[m].participacoesAluno[o].cd_conceito_participacao;
                                    vl_nota_participacao = dataRetorno[i].children[j].children[m].participacoesAluno[o].vl_nota_participacao;
                                }
                            }
                            //Se for edição da avaliação da turma do aluno, não deve considerar novas participações:
                            //Se for inclusão da avaliação da turma do aluno, não deve considerar as participações inativas:
                            if ((!existeParticipacaoAnterior && dataRetorno[i].children[j].participacoesDisponiveis[n].id_avaliacao_participacao_vinc_ativa)
                                || (existeParticipacaoAnterior && participacao_selecionada))
                            participacoesDisponiveis.push({
                                cd_participacao: dataRetorno[i].children[j].participacoesDisponiveis[n].cd_participacao,
                                id_participacao_ativa: dataRetorno[i].children[j].participacoesDisponiveis[n].id_participacao_ativa,
                                no_participacao: dataRetorno[i].children[j].participacoesDisponiveis[n].no_participacao,
                                //participacao_ativa: dataRetorno[i].children[j].participacoesDisponiveis[n].participacao_ativa,
                                participacao_selecionada: participacao_selecionada,
                                cd_conceito_participacao: cd_conceito_participacao,
                                vl_nota_participacao: vl_nota_participacao
                            });
                        }
                    if(participacoesDisponiveis.length > 0)
                        dataRetorno[i].children[j].children[m].participacoesDisponiveis = JSON.stringify(participacoesDisponiveis);
                    dataRetorno[i].children[j].children[m].participacoesAluno = JSON.stringify(dataRetorno[i].children[j].children[m].participacoesAluno);
                }
                dataRetorno[i].children[j].participacoesDisponiveis = null;
            }
        return dataRetorno;
    }
    catch (e) {
        postGerarLog(e);
    } 
}

function atualizaIdGridNota(dataRetorno) {
    try{
        var contador = 0;
        for (var d = 0; d < dataRetorno.length; d++) {
            contador++;
            dataRetorno[d].id = contador;
            for (var e = 0; e < dataRetorno[d].children.length; e++) {
                contador++;
                dataRetorno[d].children[e].id = contador;
                for (var f = 0; f < dataRetorno[d].children[e].children.length; f++) {
                    contador++;
                    dataRetorno[d].children[e].children[f].id = contador;
                }
            }
        }
        return dataRetorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loagGridNota(idTurma, xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, idTipo, reload) {
    try {
        if (reload) {
            xhr.get({
                preventCache: true,
                handleAs: "json",
                url: Endereco() + "/api/turma/getAvaliacaoTurmaArvore?idTurma=" + idTurma + "&idConceito=" + idTipo + "&tipoForm=2" + 
                    "&cd_escola_combo=0", //+ (dijit.byId("escolaFiltroTelaAvaliacao").value || dojo.byId("_ES0").value),
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataAvaliacaoTurma) {
                try {       
                    var dataAvaliacaoTurma = hasValue(dataAvaliacaoTurma.retorno) ? dataAvaliacaoTurma : JSON.parse(dataAvaliacaoTurma);
                    if (dataAvaliacaoTurma != null && dataAvaliacaoTurma.retorno != null && dataAvaliacaoTurma.retorno.tipoAvaliacaoTurma != null) {
                        destroyCreateGridNotas();
                        //Configura o avaliador:
                        dojo.byId('cbAvaliadorAvaliacao').value = JSON.stringify(dataAvaliacaoTurma.retorno.funcionarioAvaliador);

                        loadGridNotaAux(idTipo, dataAvaliacaoTurma.retorno.tipoAvaliacaoTurma, reload, dataAvaliacaoTurma.retorno.historicoAluno, dataAvaliacaoTurma.retorno.conceitosDisponiveis);
                        armazenaDadosGridNota(true);
                    }
                    else{
					    destroyCreateGridNotas();
                        apresentaMensagem('apresentadorMensagemAvaliacaoTurma', dataAvaliacaoTurma.MensagensWeb);
				    }
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
             function (error) {
                  apresentaMensagem('apresentadorMensagemAvaliacaoTurma', error.response.data);
				  showCarregando();
             });
        }
        else {
            var conceitosDisponiveis = dijit.byId("gridNota").conceitosDisponiveis; //LBM estava dadosGridNota
            destroyCreateGridNotas();
            var historico = dadosGridNota.historico;
            
            dadosGridNota.historico = null;
            dadosGridNota.conceitosDisponiveis = null;

            loadGridNotaAux(idTipo, dadosGridNota, reload, historico, conceitosDisponiveis);
			showCarregando();
        }
    }
    catch (e) {
        postGerarLog(e);
        showCarregando();
    }
}

function loadGridNotaAux(idTipo, dataRetorno, load, historico, conceitosDisponiveis) {
    try {       
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        var tituloVisao = "";
        var isInFocus = dojo.byId('isInFocus').value == 'true';
        dataRetorno = atualizaIdGridNota(dataRetorno);
        dataRetorno = atualizaParticipacoesAlunos(dataRetorno);
        dataRetorno = clearChildrenLenthZero(dataRetorno);
        /* set up layout */
        var NotaConceito = isInFocus ? "Pontos" : "Nota";
        tituloVisao = "Alunos por Avaliação";
        
        if ((hasValue(dataRetorno[0]) && dataRetorno != null)) {
            if (hasValue(dataRetorno[0].children[0].isConceito))
                if (dataRetorno[0].children[0].isConceito)
                    NotaConceito = "Conceito";
                else
                    NotaConceito = isInFocus ? "Pontos" : "Nota";
            else
                if (hasValue(dataRetorno[0].children[0].children[0]) && hasValue(dataRetorno[0].children[0].children[0].isConceito))
                    if (dataRetorno[0].children[0].children[0].isConceito)
                        NotaConceito = "Conceito";
                    else
                        NotaConceito = isInFocus ? "Pontos" : "Nota";

            if (idTipo == 0) {
                if (dataRetorno[0].children[0].isConceito) idTipo = CONCEITO;
                else idTipo = NOTA;
            }

			switch (idTipo) {
				case NOTA:
					if (visao == VISAO_ALUNO)
						tituloVisao = "Avaliações por Aluno";
					else
						tituloVisao = "Alunos por Avaliação";
					break;
				case CONCEITO:
					if (visao == VISAO_ALUNO)
						tituloVisao = "Avaliações por Aluno";
					else
						tituloVisao = "Alunos por Avaliação";
					break;
			}
			//if (!load)
			//    dataRetorno = visao == VISAO_ALUNO ? inverteVisaoNota(dataRetorno, idTipo) : inverteVisaoNotaAvaliacao(dataRetorno, idTipo);
            //LBM Quando voltar da visão Avaliações por Aluno  vai colocar as notas eventualmente digitadas lá
			if (!load) {
			    if (visao == VISAO_ALUNO)
			        dataRetorno = inverteVisaoNota(dataRetorno, idTipo)
			    else {
			        dataRetorno = inverteVisaoNotaAvaliacao(dataRetorno, idTipo)
			        for (var i = 0; i < dadosGridInicial.length; i++) {
			            for (var d = 0; d < dataRetorno.length; d++) {
			                if (dadosGridInicial[i].cd_tipo_avaliacao[0] == dataRetorno[d].cd_tipo_avaliacao[0]) {
			                    for (var j = 0; j < dadosGridInicial[i].children.length; j++) {
			                        for (var e = 0; e < dataRetorno[d].children.length; e++) {
			                            if (dadosGridInicial[i].children[j].cd_aluno == 0 &&
                                            dadosGridInicial[i].children[j].cd_aluno == dataRetorno[d].children[e].cd_aluno &&
                                            dadosGridInicial[i].children[j].cd_avaliacao == dataRetorno[d].children[e].cd_avaliacao) {
			                                dadosGridInicial[i].children[j].mediaNotas = dataRetorno[d].children[e].mediaNotas
			                                dadosGridInicial[i].children[j].notaMaxima = dataRetorno[d].children[e].notaMaxima
			                                dadosGridInicial[i].children[j].somaNotas = dataRetorno[d].children[e].somaNotas
			                                dadosGridInicial[i].children[j].cd_funcionario = dataRetorno[d].children[e].cd_funcionario
			                                dadosGridInicial[i].children[j].dt_avaliacao_turma = dataRetorno[d].children[e].dt_avaliacao_turma
			                                dadosGridInicial[i].children[j].dta_avaliacao_turma = dataRetorno[d].children[e].dta_avaliacao_turma
			                                dadosGridInicial[i].children[j].isModified = dataRetorno[d].children[e].isModified

			                                for (var k = 0; k < dadosGridInicial[i].children[j].children.length; k++) {
			                                    for (var f = 0; f < dataRetorno[d].children[e].children.length; f++) {
			                                        if (dadosGridInicial[i].children[j].children[k].cd_aluno > 0 &&
                                                        dadosGridInicial[i].children[j].children[k].cd_aluno == dataRetorno[d].children[e].children[f].cd_aluno) {
			                                            dadosGridInicial[i].children[j].children[k].vl_nota = dataRetorno[d].children[e].children[f].vl_nota
			                                            dadosGridInicial[i].children[j].children[k].vl_nota_2 = dataRetorno[d].children[e].children[f].vl_nota_2
			                                            dadosGridInicial[i].children[j].children[k].cd_conceito = dataRetorno[d].children[e].children[f].cd_conceito
			                                            dadosGridInicial[i].children[j].children[k].isModifiedA = dataRetorno[d].children[e].children[f].isModifiedA
			                                            break
			                                        }
			                                    }
			                                }
			                                break
			                            }
			                        }
			                    }
			                    break
			                }
			            }
			        }
			        dataRetorno = dadosGridInicial;
			    }
			};
			var isConceito = false;

            if (visao == VISAO_AVALIACAO)
                if (hasValue(dataRetorno[0]) && dataRetorno[0].children[0].isConceito == true)
                    isConceito = true;
            if (visao == VISAO_ALUNO)
                if (hasValue(dataRetorno[0])) {
                    if (dataRetorno[0].isConceito == null)
                        isConceito = dataRetorno[0].children[0].isConceito;
                    else
                        isConceito = dataRetorno[0].isConceito;
                }
        }

        //Monta a grade
        var data = {
            identifier: 'id',
            label: 'cd_avaliacao_turma',
            items: dataRetorno
        };

        var store = new dojo.data.ItemFileWriteStore({ data: data });
        var model = new dijit.tree.ForestStoreModel({
            store: store, childrenAttrs: ['children']
        });


        if (isConceito) {
            var layout = [
              { name: tituloVisao, field: 'dc_nome', width: '40%' },
              { name: 'Data', field: 'dta_avaliacao_turma', width: '18%', styles: "text-align: center;", formatter: formatDataAvaliacao },
              { name: NotaConceito, field: 'vl_nota', width: '32%', styles: "text-align: left;", formatter: !isConceito ? formatInputNotaAvaliacaoTurma : formatDropDownConceitoAvaliacaoTurma },
              //{ name: '2ª Nota', field: 'vl_nota_2', width: '16%', styles: "display:none;" },
              //{ name: '2ª', field: 'id_segunda_prova', width: '4%', styles: "text-align: center;", formatter: formatCheckIdSegunda },
              { name: 'Obs.', field: 'dc_observacao', width: '8%', styles: "text-align: center;", formatter: formatImage },
              { name: 'Avaliador', field: 'cd_funcionario', width: '25%', styles: "text-align: left;", formatter: formatAvaliador },
              { name: 'Msg.', field: 'dc_mensagem', width: '8%', styles: "text-align: center;", formatter: formatMsg },
              { name: '', field: 'id', width: '0%', styles: "display: none;" },
              { name: '', field: 'pai', width: '0%', styles: "display: none;" }
            ];
        }
        else {
            if (!isInFocus)
                var layout = [
                  { name: tituloVisao, field: 'dc_nome', width: '45%' },
                  { name: 'Data', field: 'dta_avaliacao_turma', width: '18%', styles: "text-align: center;", formatter: formatDataAvaliacao },
                  { name: NotaConceito, field: 'vl_nota', width: '16%', styles: "text-align: left;", formatter: !isConceito ? formatInputNotaAvaliacaoTurma : formatDropDownConceitoAvaliacaoTurma },
                  { name: '2ª Chance', field: 'vl_nota_2', width: '16%', styles: "text-align: left;", formatter: formatInputNotaAvaliacaoTurma2 },
                  //{ name: '2ª', field: 'id_segunda_prova', width: '4%', styles: "text-align: center;", formatter: formatCheckIdSegunda },
                  { name: 'Obs.', field: 'dc_observacao', width: '8%', styles: "text-align: center;", formatter: formatImage },
                  { name: 'Avaliador', field: 'cd_funcionario', width: '25%', styles: "text-align: left;", formatter: formatAvaliador },
                  { name: 'Msg.', field: 'dc_mensagem', width: '8%', styles: "text-align: center;", formatter: formatMsg },
                  { name: '', field: 'id', width: '0%', styles: "display: none;" },
                  { name: '', field: 'pai', width: '0%', styles: "display: none;" }
                ];
            else
                var layout = [
                    { name: tituloVisao, field: 'dc_nome', width: '45%' },
                    { name: 'Data', field: 'dta_avaliacao_turma', width: '18%', styles: "text-align: center;", formatter: formatDataAvaliacao },
                    { name: NotaConceito, field: 'vl_nota', width: '16%', styles: "text-align: left;", formatter: !isConceito ? formatInputNotaAvaliacaoTurma : formatDropDownConceitoAvaliacaoTurma },
                    //{ name: '2ª Chance', field: 'vl_nota_2', width: '1%', styles: "text-align: left;display: none"},
                    { name: 'Obs.', field: 'dc_observacao', width: '8%', styles: "text-align: center;", formatter: formatImage },
                    { name: 'Avaliador', field: 'cd_funcionario', width: '25%', styles: "text-align: left;", formatter: formatAvaliador },
                    { name: 'Msg.', field: 'dc_mensagem', width: '8%', styles: "text-align: center;", formatter: formatMsg },
                    { name: '', field: 'id', width: '0%', styles: "display: none;" },
                    { name: '', field: 'pai', width: '0%', styles: "display: none;" }
                ];
        }
        var gridNota = new dojox.grid.LazyTreeGrid({
            id: 'gridNota',
            treeModel: model,
            structure: layout
        }, document.createElement('div'));
        dojo.byId("gridNota").appendChild(gridNota.domNode);
        gridNota.canSort = function (col) { return false; };
        gridNota.historico = historico;
        gridNota.conceitosDisponiveis = conceitosDisponiveis;
        gridNota.startup();
        //dijit.byId("gridNota").on("mouseOver", function (e) {
        //    $('#gridNota').children()[0].children[1].children[0].children[2].children[0].style.cssText = "height: 100%; width: 668px;"
        //});


    }
    catch (e) {
        postGerarLog(e);
    }
}

function AvaliacaoTurmaUI(isPesquisa,ivisao) {
    try{
        var visao = hasValue(dijit.byId('visao')) ? dijit.byId('visao').value : 0;
        if (ivisao < 0) visao = 3 - visao; //Os dados estão com a visão antiga
        var gridNota = hasValue(dijit.byId("gridNota")) ? dijit.byId("gridNota") : 0;
        var tipo = dijit.byId('tipoAvaliacao').get('value');
        var listAvalicacoesTurma = [];
        var listAvalicacoesAluno = [];
        var dataAvaliacao = null;
        var nom_aluno = "";

        if (!isPesquisa) {
            if (hasValue(gridNota) && visao == VISAO_AVALIACAO) {
                for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++) {
                    for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++) {
                        if (gridNota.store._arrayOfTopLevelItems[i].children[j].pai[0] == 1 && gridNota.store._arrayOfTopLevelItems[i].children[j].cd_tipo_avaliacao == 0) {
                            dataAvaliacao = gridNota.store._arrayOfTopLevelItems[i].children[j].dt_avaliacao_turma[0];
                            listAvalicacoesTurma.push({
                                cd_avaliacao_turma: gridNota.store._arrayOfTopLevelItems[i].children[j].cd_avaliacao_turma[0],
                                tx_obs_aval_turma: gridNota.store._arrayOfTopLevelItems[i].children[j].dc_observacao[0],
                                cd_turma: hasValue(dojo.byId('cd_turma').value > 0) ? dojo.byId('cd_turma').value : 0,
                                cd_avaliacao: gridNota.store._arrayOfTopLevelItems[i].children[j].cd_avaliacao[0],
                                cd_funcionario: gridNota.store._arrayOfTopLevelItems[i].children[j].cd_funcionario[0],
                                dt_avaliacao_turma: dataAvaliacao,
                                nm_nota_total_turma: gridNota.store._arrayOfTopLevelItems[i].children[j].vl_nota[0],
                                isModified: gridNota.store._arrayOfTopLevelItems[i].children[j].isModified[0] ||
                                    gridNota.store._arrayOfTopLevelItems[i].children[j].dc_observacao[0] != ""
                            });
                        }//if gridNota.store._arrayOfTopLevelItems[i].pai[0]
                        for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                            var nota = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0];
                            var nota2 = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota_2[0];
                            var isModifiedA = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].isModifiedA[0];

                            if (nota != '' && nota != null)
                                dataAvaliacao = gridNota.store._arrayOfTopLevelItems[i].children[j].dt_avaliacao_turma[0];
                            dataAvaliacao = dataAvaliacao;
                            if (gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].isChildren == 1 && gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_aluno > 0)
                                var dataAvalicao =
                                listAvalicacoesAluno.push({
                                    cd_avaliacao_aluno: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_avaliacao_aluno[0],
                                    cd_avaliacao_turma: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_avaliacao_turma[0],
                                    cd_aluno: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_aluno[0],
                                    dt_avalicao_aluno: dataAvaliacao,
                                    nm_nota_aluno: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0],
                                    nm_nota_aluno_2: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota_2[0],
                                    cd_conceito: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_conceito[0],
                                    tx_obs_nota_aluno: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].dc_observacao[0],
                                    id_segunda_prova: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id_segunda_prova[0],
                                    isModifiedA: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].isModifiedA[0] ||
                                        gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].dc_observacao[0] != "",
                                    AvaliacoesAlunoParticipacao: jQuery.parseJSON(gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].participacoesAluno[0])
                                });
                        }// for k
                    }//for j
                }//for i
            }
            if (hasValue(gridNota) && visao == VISAO_ALUNO) {
                for (var i = 0; i < gridNota.store._arrayOfTopLevelItems.length; i++)
                    for (var j = 0; j < gridNota.store._arrayOfTopLevelItems[i].children.length; j++)
                        for (var k = 0; k < gridNota.store._arrayOfTopLevelItems[i].children[j].children.length; k++) {
                            //variaveis para monta o objeto do aluno, esses valores são remontados no criterio.
                            var notaAluno = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota[0];
                            var notaAluno2 = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].vl_nota_2[0];
                            var isModifiedA = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].isModifiedA[0];
                            var isSegunda = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].id_segunda_prova[0];
                            var obsAluno = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].dc_observacao[0];
                            if (notaAluno != '' && notaAluno != null)
                                dataAvaliacao = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].dt_avaliacao_turma[0];
                            var idAvaliacaoTurma = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_avaliacao_turma[0];
                            var idAvaliacaoAluno = gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_avaliacao_aluno[0];

                            listAvalicacoesTurma.push({
                                cd_avaliacao_turma: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].idPai[0],
                                tx_obs_aval_turma: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].dc_observacao_aux[0],
                                cd_turma: hasValue(dojo.byId('cd_turma').value > 0) ? dojo.byId('cd_turma').value : 0,
                                cd_avaliacao: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_avaliacao[0],
                                cd_funcionario: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_funcionario[0],
                                dt_avaliacao_turma: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].dt_avaliacao_turma[0],
                                isModified: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].isModified[0] ||
                                    gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].dc_observacao_aux[0] != "",
                                nm_nota_total_turma: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].somaNotas[0]
                            });

                            if (gridNota.store._arrayOfTopLevelItems[i].cd_aluno > 0)
                                listAvalicacoesAluno.push({
                                    cd_avaliacao_aluno: idAvaliacaoAluno,
                                    cd_avaliacao_turma: idAvaliacaoTurma,
                                    cd_aluno: gridNota.store._arrayOfTopLevelItems[i].cd_aluno[0],
                                    dt_avalicao_aluno: dataAvaliacao,
                                    nm_nota_aluno: notaAluno,
                                    nm_nota_aluno_2: notaAluno2,
                                    cd_conceito: gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].cd_conceito[0],
                                    tx_obs_nota_aluno: obsAluno,
                                    id_segunda_prova: isSegunda,
                                    isModifiedA: isModifiedA || (obsAluno != "" && obsAluno != null),
                                    AvaliacoesAlunoParticipacao: jQuery.parseJSON(gridNota.store._arrayOfTopLevelItems[i].children[j].children[k].participacoesAluno[0])    //LBM Incluido para a segunda visão
                            });
                        }
            }
            this.avalialcoesTurmas = listAvalicacoesTurma;
            this.avalialcoesAlunos = listAvalicacoesAluno;
        }
        this.cd_turma = hasValue(dojo.byId('cd_turma').value > 0) ? dojo.byId('cd_turma').value : 0;
        this.no_turma = hasValue(dojo.byId('turma').value != "") ? dojo.byId('turma').value : "";
        this.no_tipo_criterio = hasValue(dojo.byId('tipoAvaliacao').value != "") ? dojo.byId('tipoAvaliacao').value : "";
        this.idTurma = hasValue(dojo.byId('cd_turma_pesquisa').value) ? dojo.byId('cd_turma_pesquisa').value : 0;
        this.idTipoCriterioNota = hasValue(dijit.byId('cbTipoAvaliacao').value) ? dijit.byId('cbTipoAvaliacao').value : 0;
        this.idTipoAvaliacao = hasValue(dijit.byId('cbAvaliacaoCurso').value > 0) ? dijit.byId('cbAvaliacaoCurso').value : 0;
        this.idCriterioAvaliacao = hasValue(dijit.byId('cbNomes').value > 0) ? dijit.byId('cbNomes').value : 0;
        this.idCurso = hasValue(dijit.byId('cbCurso').value > 0) ? dijit.byId('cbCurso').value : 0;
        this.idFuncionario = hasValue(dijit.byId('cbAvaliador').value > 0) ? dijit.byId('cbAvaliador').value : 0;
        this.isConceito = parseInt(tipo) == 1 ? true : false;
        this.isInFocus = dojo.byId('isInFocus').value == 'true';
        if (this.isInFocus) this.no_tipo_criterio = PONTOS;
        if (hasValue(dojo.byId("dtaIni").value)) {
            this.dtaInicial = dojo.byId("dtaIni").value;
        } else {
            this.dtaInicial = minDate;
        }
        if (hasValue(dojo.byId("dtaFim").value)) {
            this.dtaFinal = dojo.byId("dtaFim").value;
        } else {
            this.dtaFinal = maxDate;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function searchAvaliacaoTurma(limparItens) {
    var variaveisPesquisa = new AvaliacaoTurmaUI(true,0);
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    //int cd_tipo_avaliacao, int cd_criterio_avaliacao, int cd_curso, int cd_funcionario, string dta_inicial, string dta_final
                    target: Endereco() + "/api/turma/getSearchAvaliacaoTurma?idTurma=" + variaveisPesquisa.idTurma + "&idTipoAvaliacao=" + variaveisPesquisa.idTipoCriterioNota + "&cd_tipo_avaliacao=" + variaveisPesquisa.idTipoAvaliacao + "&cd_criterio_avaliacao=" + variaveisPesquisa.idCriterioAvaliacao + "&cd_curso=" + variaveisPesquisa.idCurso + "&cd_funcionario=" + variaveisPesquisa.idFuncionario + "&dta_inicial=" + variaveisPesquisa.dtaInicial + "&dta_final=" + variaveisPesquisa.dtaFinal + "&cd_escola_combo=0",
                    //+ (dijit.byId("escolaFiltroTelaAvaliacao").value || dojo.byId("_ES0").value),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_avaliacao_turma"
                }), Memory({ idProperty: "cd_avaliacao_turma" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridAvaliacaoTurma = dijit.byId("gridAvaliacaoTurma");
            if (limparItens) {
                gridAvaliacaoTurma.itensSelecionados = [];
            }
            gridAvaliacaoTurma.noDataMessage = msgNotRegEnc;
            gridAvaliacaoTurma.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function editarAvaliacaoTurma() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemAvaliacaoTurma', null);
        if(hasValue(dijit.byId('gridAvaliacaoTurma').itemSelecionado))
            dojo.byId("cd_turma").value = dijit.byId('gridAvaliacaoTurma').itemSelecionado.cd_turma;
		var avaliacaoTurma = new AvaliacaoTurmaUI(false,0);
        var conceito = dijit.byId('tipoAvaliacao').value;
        var gridNota = dijit.byId('gridNota');
        if (gridNota == undefined && gridNota == null) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Não existe notas para serem lançadas.');
            apresentaMensagem('apresentadorMensagemAvaliacaoTurma', mensagensWeb)
            return false
        }
        if (conceito != 1)
            if (verificarNotaAlunoMaiorNotaTurma() == false) {
                return false
            }

        if (verificaExistNotaSemAvaliador() == false)
            return false;
        showCarregando();
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            xhr.post(Endereco() + "/api/turma/postAlterarAvaliacaoTurma", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(avaliacaoTurma)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridAvaliacaoTurma';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadAvaliacaoTurma").hide();
                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        removeObjSort(grid.itensSelecionados, "cd_turma", dom.byId("cd_turma").value);
                        insertObjSort(grid.itensSelecionados, "cd_turma", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionadoAvaliacaoTurma', 'cd_turma', 'selecionaTodosAvaliacaoTurma', ['pesquisarAvaliacaoTurma', 'relatorioAvaliacaoTurma'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_turma");
                    } else
                        apresentaMensagem('apresentadorMensagemAvaliacaoTurma', data);
                    showCarregando();
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAvaliacaoTurma', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function salvarAvaliacao() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemAvaliacaoTurma', null);
        var avaliacaoTurma = new AvaliacaoTurmaUI(false,-1);
        showCarregando();
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            xhr.post(Endereco() + "/api/turma/postAlterarAvaliacaoTurma", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(avaliacaoTurma)
            }).then(function (data) {
                try {
                    showCarregando();
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAvaliacaoTurma', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function DeletarAvaliacaoTurma(itensSelecionados) {
    try{
        var tipo = dijit.byId('tipoAvaliacao').get('value');
        require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                itensSelecionados = new Array();
                itensSelecionados.push(dijit.byId('gridAvaliacaoTurma').itemSelecionado);
            }
            xhr.post({
                url: Endereco() + "/api/turma/postDeleteAvaliacaoTurma",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItens");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadAvaliacaoTurma").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        if(hasValue(itensSelecionados[r]))
                            removeObjSort(dijit.byId('gridAvaliacaoTurma').itensSelecionados, "cd_turma", itensSelecionados[r].cd_turma);
                    searchAvaliacaoTurma(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarAvaliacaoTurma").set('disabled', false);
                    dijit.byId("relatorioAvaliacaoTurma").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadAvaliacaoTurma").style.display))
                    apresentaMensagem('apresentadorMensagemAvaliacaoTurma', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region eventos para link e FK- eventoEditarAvaliacao - retornarTurmaFK

function eventoEditarAvaliacaoTurma(itensSelecionados, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            dijit.byId('gridAvaliacaoTurma').itemSelecionado = itensSelecionados[0];
            montarCadastroEdicaoAvaliacaoTurma(dijit.byId('gridAvaliacaoTurma'), xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarCadastroEdicaoAvaliacaoTurma(grid, xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr) {
    showCarregando();

    apresentaMensagem('apresentadorMensagem', null);
    dijit.byId('tipoAvaliacao')._onChangeActive = false;
    dijit.byId('visao')._onChangeActive = false;
    dijit.byId("cbAvaliacaoCurso")._onChangeActive = false;
    dijit.byId('tipoAvaliacao').reset();
    dijit.byId('tipoAvaliacao').set('value', 0);
    dijit.byId('tipoAvaliacao').set('disabled', true);
    dijit.byId('turma').set('disabled', true);
    dojo.byId('isUpdate').value = true;
    dijit.byId('cadProTurmaFK').set('disabled', true);
    keepValues(grid, xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr);
    dijit.byId('visao')._onChangeActive = true;
    dijit.byId('tipoAvaliacao')._onChangeActive = true;
    dijit.byId("cadAvaliacaoTurma").show();
}

function retornarTurmaFK(fieldID, fieldNome) {
    try{
        var valido = true;
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        if (gridPesquisaTurmaFK.itensSelecionados[0].cd_pessoa_escola != dojo.byId('_ES0').value) {
            caixaDialogo(DIALOGO_AVISO, 'Turmas de outra escola somente podem ter lançadas avaliações, na escola de origem.', null);
            return false;
        }
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            if (dojo.byId('cadAvaliacaoTurma').style.display == 'none') {
                dojo.byId("cd_turma_pesquisa").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("cbTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dojo.byId("produto").value = gridPesquisaTurmaFK.itensSelecionados[0].no_produto;
                dojo.byId("curso").value = gridPesquisaTurmaFK.itensSelecionados[0].no_curso;
                dojo.byId("cd_produto").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_produto;
                dojo.byId("cd_curso").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_curso;
            }
            else {
                dojo.byId("cd_turma").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("turma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dojo.byId("produto").value = gridPesquisaTurmaFK.itensSelecionados[0].no_produto;
                dojo.byId("curso").value = gridPesquisaTurmaFK.itensSelecionados[0].no_curso;
                dojo.byId("cd_produto").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_produto;
                dojo.byId("cd_curso").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_curso;
                montarAvaliacoesORPesquisarTiposAvaliacao(gridPesquisaTurmaFK.itensSelecionados[0].cd_turma);
            }
            dijit.byId('limparProTurmaFK').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTurmaFK() {
    try {
        limparFiltrosTurmaFK();
        dojo.byId("idOrigemCadastro").value = PESAVALIACAOTURMA;
        dijit.byId("proTurmaFK").show();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            var compProfpesquisa = dijit.byId('pesProfessorFK');
            if (compProfpesquisa.disabled == true && hasValue(compProfpesquisa.value))
                compProfpesquisa.set('disabled', true);
        }
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
        /* --combo_escola_fk
        dojo.byId("trEscolaTurmaFiltroFk").style.display = "";
        dojo.byId("lbEscolaTurmaFiltroFk").style.display = "";
        require(['dojo/dom-style', 'dijit/registry'],
            function (domStyle, registry) {

                domStyle.set(registry.byId("escolaTurmaFiltroFK").domNode, 'display', '');
            });*/

        pesquisarTurmaFK();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTurmaFKCadastro() {
    try {
        limparFiltrosTurmaFK();
        dojo.byId("idOrigemCadastro").value = CADAVALIACAOTURMA;
        dijit.byId("proTurmaFK").show();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            var compProfpesquisa = dijit.byId('pesProfessorFK');
            if (compProfpesquisa.disabled == true && hasValue(compProfpesquisa.value))
                compProfpesquisa.set('disabled', true);
        }
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        dojo.byId("legendTurmaFK").style.visibility = "hidden";

        /* --combo_escola_fk
        dojo.byId("trEscolaTurmaFiltroFk").style.display = "";
        dojo.byId("lbEscolaTurmaFiltroFk").style.display = "";
        require(['dojo/dom-style', 'dijit/registry'],
            function (domStyle, registry) {

                domStyle.set(registry.byId("escolaTurmaFiltroFK").domNode, 'display', '');
            });*/
        pesquisarTurmaFKCadastroAvalicao();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//essa função serve para montar as avaliacões ou buscar os tipos, a letra "e" é o codigo da turma
function montarAvaliacoesORPesquisarTiposAvaliacao(e) {
    try{
        dojo.byId('isConceitoNota').value = '';
        apresentaMensagem('apresentadorMensagemAvaliacaoTurma', null);
        var tipo = 0;
        var isConceitoNota = false;
        setIsConceitoNota(isConceitoNota);
        dojo.byId('cd_turma').value = e;
        if (e > 0)
            require(["dojo/dom",
                     "dojo/dom-attr",
                     "dojo/_base/xhr",
                     "dijit/registry",
                     "dojox/grid/EnhancedGrid",
                     "dojo/data/ObjectStore",
                     "dojo/store/Cache",
                     "dojo/store/Memory",
                     "dojo/query"
            ], function (dom, domAttr, xhr, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query) {
                xhr.get({
                    preventCache: true,
                    url: Endereco() + "/api/turma/returnAvaliacoesNotaOrConceito?idTurma=" + e,
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dataAvaliacaoTurma) {
                    try{
                        apresentaMensagem("apresentadorMensagemAvaliacaoTurma", dataAvaliacaoTurma);

                        var dataRetorno = jQuery.parseJSON(eval(dataAvaliacaoTurma)).retorno;

                        if (hasValue(dataRetorno)) {
                            isConceitoNota = dataRetorno[0].is_conceito_nota;
                            dojo.byId('isConceitoNota').value = dataRetorno[0].is_conceito_nota;
                            dojo.byId('isConceito').value = dataRetorno[0].isConceito;
                            dojo.byId('isInFocus').value = dataRetorno[0].isInFocus;
                            setIsConceitoNota(isConceitoNota);

                            if (!isConceitoNota) {
                                var tipoAvaliacao = dijit.byId("tipoAvaliacao");

                                tipoAvaliacao._onChangeActive = false;
                                if (dataRetorno[0].isConceito)
                                    tipoAvaliacao.set('value', CONCEITO);
                                else
                                    tipoAvaliacao.set('value', NOTA);
                                tipoAvaliacao._onChangeActive = true;
                            }

                            //verifica se é conceito/nota - conceito e nota
                            if (hasValue(dijit.byId("tipoAvaliacao")) && isConceitoNota) {
                                tipo = dijit.byId("tipoAvaliacao").value;
                            } else {
                                if (dataRetorno[0].isConceito)
                                    tipo = 1;
                                else tipo = 0;
                            }

                            if (isConceitoNota) {
                                if (hasValue(e > 0) && tipo > 0) {
                                    showCarregando();
                                    loagGridNota(e, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, tipo, true);
                                    dijit.byId("salvarAvaliacaoTurma").set('disabled', false);
                                    dijit.byId("tipoAvaliacao").set('disabled', false);
                                }
                            } else {
                                showCarregando();
                                loagGridNota(e, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, tipo, true);
                                dijit.byId("salvarAvaliacaoTurma").set('disabled', false);
                            }
                            //dijit.byId('visao').set('disabled', false);
                        }
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagemAvaliacaoTurma', error);
                    dijit.byId('visao').set('disabled', true);
                });
            });

        if (dojo.byId('isUpdate').value = false)
            if (hasValue(e <= 0) || !isConceitoNota) {
                destroyCreateGridNotas();
                if (dojo.byId('isUpdate').value == "false"){
                    var dijitTipoAvaliacao = dijit.byId("tipoAvaliacao");
                    dijitTipoAvaliacao._onChangeActive = false;
                    dijitTipoAvaliacao.reset();
                    dijitTipoAvaliacao._onChangeActive = true;
                }
                dijit.byId("salvarAvaliacaoTurma").set('disabled', true);
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion


function pesquisarTurmaFKCadastroAvalicao(cdProfDefault) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var cdCurso = hasValue(dijit.byId("pesCursoFK").value) ? dijit.byId("pesCursoFK").value : 0;
            var cdProduto = hasValue(dijit.byId("pesProdutoFK").value) ? dijit.byId("pesProdutoFK").value : 0;
            var cdDuracao = hasValue(dijit.byId("pesDuracaoFK").value) ? dijit.byId("pesDuracaoFK").value : 0;
            var cdProfessor = hasValue(dijit.byId("pesProfessorFK").value) ? dijit.byId("pesProfessorFK").value : 0;
            var cdProg = hasValue(dijit.byId("sPogramacaoFK").value) ? dijit.byId("sPogramacaoFK").value : 0;
            var cdSitTurma = hasValue(dijit.byId("pesSituacaoFK").value) ? dijit.byId("pesSituacaoFK").value : 0;
            var cdTipoTurma = hasValue(dijit.byId("tipoTurmaFK").value) ? dijit.byId("tipoTurmaFK").value : 0;
            var cdAluno = dojo.byId("cdAlunoFKTurmaFK").value > 0 ? dojo.byId("cdAlunoFKTurmaFK").value : 0;
            if (hasValue(cdProfDefault) && cdProfDefault > 0)
                cdProfessor = cdProfDefault;
            //(string descricao, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, bool sProg)
            /* --combo_escola_fk
            //Mostra a combo de escola
            dojo.byId("trEscolaTurmaFiltroFk").style.display = "";
            dojo.byId("lbEscolaTurmaFiltroFk").style.display = "";
            require(['dojo/dom-style', 'dijit/registry'],
                function (domStyle, registry) {

                    domStyle.set(registry.byId("escolaTurmaFiltroFK").domNode, 'display', '');
                });
            */
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getTurmaSearchFKAvaliacao?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value +
                                             "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + cdTipoTurma +
                                            "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor +
                                            "&prog=" + cdProg + "&turmasFilhas=" + document.getElementById("pesTurmasFilhasFK").checked + "&cdAluno=" + cdAluno +
                                            "&dtInicial=&dtFinal=&cd_turma_PPT=null&semContrato=false&dataInicial=&dataFinal=" + "&cd_escola_combo_fk=0",
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaTurmaFK");
            grid.itensSelecionados = [];
            grid.setStore(dataStore);

            if (dijit.byId("tipoTurmaFK").get('value') == PPT) {
                grid.layout.setColumnVisibility(2, true)
                grid.layout.setColumnVisibility(3, false)
                grid.turmasFilhas = true;
            }
            else {
                grid.layout.setColumnVisibility(2, false)
                grid.layout.setColumnVisibility(3, true)
                grid.turmasFilhas = false;
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}



function findAllEmpresasUsuarioComboFiltroEscolaTelaAvaliacao() {
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/empresa/findAllEmpresasUsuarioComboFK",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_pessoa"
        }).then(function (data) {
                try {
                    showCarregando();

                    var dataRetorno = jQuery.parseJSON(data);
                    if (hasValue(dataRetorno)) {
                        var dataCombo = dataRetorno.map(function (item, index, array) {
                            return { name: item.dc_reduzido_pessoa, id: item.cd_pessoa + "" };
                        });
                        loadSelect(dataRetorno, "escolaFiltroTelaAvaliacao", 'cd_pessoa', 'dc_reduzido_pessoa', dojo.byId("_ES0").value);
                    }
                    showCarregando();
                }
                catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
    });
}
