//Os tipos de formulário:
var FORM_ESCOLARIDADE = 1;
var FORM_MIDIA = 2;
var FORM_TIPOCONTATO = 3;
var FORM_MOTIVOMATRICULA = 4;
var FORM_MOTIVONAOMATRICULA = 5;
var FORM_MOTIVOBOLSA = 6;
var FORM_MOTIVOCANCELBOLSA = 7;
var FORM_ACAOFOLLOWUP = 8;
var FORM_ANOESCOLAR = 9;
var FORM_MOTIVOTRANSF = 10;

function mostraTabs(Permissoes) {
    require([
         "dijit/registry",
         "dojo/ready"
    ], function (registry, ready) {
        ready(function () {
            try {
                if (!possuiPermissao('escd', Permissoes)) {
                    registry.byId('tabEscolaridade').set('disabled', !registry.byId('tabEscolaridade').get('disabled'));
                    document.getElementById('tabEscolaridade').style.visibility = "hidden";
                }

                if (!possuiPermissao('mid', Permissoes)) {
                    registry.byId('tabMidia').set('disabled', !registry.byId('tabMidia').get('disabled'));
                    document.getElementById('tabMidia').style.visibility = "hidden";
                }

                if (!possuiPermissao('tpctt', Permissoes)) {
                    registry.byId('tabTipoContato').set('disabled', !registry.byId('tabTipoContato').get('disabled'));
                    document.getElementById('tabTipoContato').style.visibility = "hidden";
                }

                if (!possuiPermissao('mtmt', Permissoes)) {
                    registry.byId('tabMotivoMatricula').set('disabled', !registry.byId('tabMotivoMatricula').get('disabled'));
                    document.getElementById('tabMotivoMatricula').style.visibility = "hidden";
                }

                if (!possuiPermissao('mtnm', Permissoes)) {
                    registry.byId('tabMotivoNMatricula').set('disabled', !registry.byId('tabMotivoNMatricula').get('disabled'));
                    document.getElementById('tabMotivoNMatricula').style.visibility = "hidden";
                }

                if (!possuiPermissao('mtb', Permissoes)) {
                    registry.byId('tabMotivoBolsa').set('disabled', !registry.byId('tabMotivoBolsa').get('disabled'));
                    document.getElementById('tabMotivoBolsa').style.visibility = "hidden";
                }

                if (!possuiPermissao('mtcb', Permissoes)) {
                    registry.byId('tabMotivoCancelBolsa').set('disabled', !registry.byId('tabMotivoCancelBolsa').get('disabled'));
                    document.getElementById('tabMotivoCancelBolsa').style.visibility = "hidden";
                }
                if (!possuiPermissao('acfup', Permissoes)) {
                    registry.byId('tabAcaoFollow').set('disabled', !registry.byId('tabAcaoFollow').get('disabled'));
                    document.getElementById('tabAcaoFollow').style.visibility = "hidden";
                }
                if (!possuiPermissao('anesc', Permissoes)) {
                    registry.byId('tabAnoEscolar').set('disabled', !registry.byId('tabAnoEscolar').get('disabled'));
                    document.getElementById('tabAnoEscolar').style.visibility = "hidden";
                }
                
                if (!possuiPermissao('mttr', Permissoes)) {
                    registry.byId('tabMotivoTransf').set('disabled', !registry.byId('tabMotivoTransf').get('disabled'));
                    document.getElementById('tabMotivoTransf').style.visibility = "hidden";
                }
            } catch (e) {
                postGerarLog(e);
            }
        })
    });
}

//Pega os Antigos dados do Formulário
function keepValues(tipoForm, value, grid, ehLink) {
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

        switch (tipoForm) {
            //1
            case FORM_ESCOLARIDADE: {
                getLimpar('#formEscolaridade');
                dojo.byId("cd_escolaridade").value = value.cd_escolaridade;
                dojo.byId("no_escolaridade").value = value.no_escolaridade;
                dojo.byId("id_escolaridade_ativa").value = value.id_escolaridade_ativa == true ? dijit.byId("id_escolaridade_ativa").set("value", true) : dijit.byId("id_escolaridade_ativa").set("value", false);

                break;
            }//2
            case FORM_MIDIA: {
                getLimpar('#formMidia');
                dojo.byId("cd_midia").value = value.cd_midia;
                dojo.byId("no_midia").value = value.no_midia;
                dojo.byId("id_midia_ativa").value = value.id_midia_ativa == true ? dijit.byId("id_midia_ativa").set("value", true) : dijit.byId("id_midia_ativa").set("value", false);

                break;
            }//3
            case FORM_TIPOCONTATO: {
                getLimpar('#formTpContato');
                dojo.byId("cd_tipo_contato").value = value.cd_tipo_contato;
                dojo.byId("dc_tipo_contato").value = value.dc_tipo_contato;
                dojo.byId("id_tipo_contato_ativo").value = value.id_tipo_contato_ativo == true ? dijit.byId("id_tipo_contato_ativo").set("value", true) : dijit.byId("id_tipo_contato_ativo").set("value", false);

                break;
            }//4
            case FORM_MOTIVOMATRICULA: {
                getLimpar('#formMtvMat');
                dojo.byId("cd_motivo_matricula").value = value.cd_motivo_matricula;
                dojo.byId("dc_motivo_matricula").value = value.dc_motivo_matricula;
                dojo.byId("id_motivo_matricula_ativo").value = value.id_motivo_matricula_ativo == true ? dijit.byId("id_motivo_matricula_ativo").set("value", true) : dijit.byId("id_motivo_matricula_ativo").set("value", false);

                break;
            }//5
            case FORM_MOTIVONAOMATRICULA: {
                getLimpar('#formMtvNMat');
                dojo.byId("cd_motivo_nao_matricula").value = value.cd_motivo_nao_matricula;
                dojo.byId("dc_motivo_nao_matricula").value = value.dc_motivo_nao_matricula;
                dojo.byId("id_motivo_nao_matricula_ativo").value = value.id_motivo_nao_matricula_ativo == true ? dijit.byId("id_motivo_nao_matricula_ativo").set("value", true) : dijit.byId("id_motivo_nao_matricula_ativo").set("value", false);

                break;
            }//6
            case FORM_MOTIVOBOLSA: {
                getLimpar('#formMtvBolsa');
                dojo.byId("cd_motivo_bolsa").value = value.cd_motivo_bolsa;
                dojo.byId("dc_motivo_bolsa").value = value.dc_motivo_bolsa;
                dojo.byId("id_motivo_bolsa_ativo").value = value.id_motivo_bolsa_ativo == true ? dijit.byId("id_motivo_bolsa_ativo").set("value", true) : dijit.byId("id_motivo_bolsa_ativo").set("value", false);
                return false;
            }//7
            case FORM_MOTIVOCANCELBOLSA: {
                getLimpar('#formMtvCancelBolsa');
                dojo.byId("cd_motivo_cancelamento_bolsa").value = value.cd_motivo_cancelamento_bolsa;
                dojo.byId("dc_motivo_cancelamento_bolsa").value = value.dc_motivo_cancelamento_bolsa;
                dojo.byId("id_motivo_cancelamento_bolsa_ativo").value = value.id_motivo_cancelamento_bolsa_ativo == true ? dijit.byId("id_motivo_cancelamento_bolsa_ativo").set("value", true) : dijit.byId("id_motivo_cancelamento_bolsa_ativo").set("value", false);

                break;
            }//8
            case FORM_ACAOFOLLOWUP: {
                getLimpar('#formAcaoFollowup');
                dojo.byId("cd_acao_follow_up").value = value.cd_acao_follow_up;
                dojo.byId("dc_acao_follow_up").value = value.dc_acao_follow_up;
                dojo.byId("id_acao_ativa").value = value.id_motivo_cancelamento_bolsa_ativo == true ? dijit.byId("id_motivo_cancelamento_bolsa_ativo").set("value", true) : dijit.byId("id_motivo_cancelamento_bolsa_ativo").set("value", false);

                break;
            }//9
            case FORM_ANOESCOLAR: {
                getLimpar('#formAnoEscolar');
                dojo.byId("cd_ano_escolar").value = value.cd_ano_escolar;
                dijit.byId('cd_escolaridade_ano_escolar').set('value', value.cd_escolaridade);
                dojo.byId("dc_ano_escolar").value = value.dc_ano_escolar;
                dojo.byId("nm_ordem").value = value.nm_ordem;
                dojo.byId("id_ativo").value = value.id_ativo == true ? dijit.byId("id_ativo").set("value", true) : dijit.byId("id_ativo").set("value", false);

                break;
            }//10
            case FORM_MOTIVOTRANSF: {
                getLimpar('#formMtvTransf');
                dojo.byId("cd_motivo_transferencia_aluno").value = value.cd_motivo_transferencia_aluno;
                dojo.byId("dc_motivo_transferencia_aluno").value = value.dc_motivo_transferencia_aluno;
                dojo.byId("id_motivo_transferencia_ativo").value = value.id_motivo_transferencia_ativo == true ? dijit.byId("id_motivo_transferencia_ativo").set("value", true) : dijit.byId("id_motivo_transferencia_ativo").set("value", false);

                break;
            }
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBox(value, rowIndex, obj) {
    try{
        var gridName = 'gridEsc';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_escolaridade", grid._by_idx[rowIndex].item.cd_escolaridade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_escolaridade', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_escolaridade', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMid(value, rowIndex, obj) {
    try{
        var gridName = 'gridMid';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMid');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_midia", grid._by_idx[rowIndex].item.cd_midia);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_midia', 'selecionadoMid', -1, 'selecionaTodosMid', 'selecionaTodosMid', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_midia', 'selecionadoMid', " + rowIndex + ", '" + id + "', 'selecionaTodosMid', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxTpContato(value, rowIndex, obj) {
    try{
        var gridName = 'gridTpContato';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTpContato');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_contato", grid._by_idx[rowIndex].item.cd_tipo_contato);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_contato', 'selecionadoTpContato', -1, 'selecionaTodosTpContato', 'selecionaTodosTpContato', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_contato', 'selecionadoTpContato', " + rowIndex + ", '" + id + "', 'selecionaTodosTpContato', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMtvMat(value, rowIndex, obj) {
    try{
        var gridName = 'gridMtvMat';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMtvMat');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_matricula", grid._by_idx[rowIndex].item.cd_motivo_matricula);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_matricula', 'selecionadoMtvMat', -1, 'selecionaTodosMtvMat', 'selecionaTodosMtvMat', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_matricula', 'selecionadoMtvMat', " + rowIndex + ", '" + id + "', 'selecionaTodosMtvMat', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMtvNMat(value, rowIndex, obj) {
    try{
        var gridName = 'gridMtvNMat';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMtvNMat');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_nao_matricula", grid._by_idx[rowIndex].item.cd_motivo_nao_matricula);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_nao_matricula', 'selecionadoMtvNMat', -1, 'selecionaTodosMtvNMat', 'selecionaTodosMtvNMat', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_nao_matricula', 'selecionadoMtvNMat', " + rowIndex + ", '" + id + "', 'selecionaTodosMtvNMat', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMtvBolsa(value, rowIndex, obj) {
    try{
        var gridName = 'gridMtvBolsa';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMtvBolsa');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_bolsa", grid._by_idx[rowIndex].item.cd_motivo_bolsa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_bolsa', 'selecionadoMtvBolsa', -1, 'selecionaTodosMtvBolsa', 'selecionaTodosMtvBolsa', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_bolsa', 'selecionadoMtvBolsa', " + rowIndex + ", '" + id + "', 'selecionaTodosMtvBolsa', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMtvCancelBolsa(value, rowIndex, obj) {
    try{
        var gridName = 'gridMtvCancelBolsa';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMtvCancelBolsa');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_cancelamento_bolsa", grid._by_idx[rowIndex].item.cd_motivo_cancelamento_bolsa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_cancelamento_bolsa', 'selecionadoMtvCancelBolsa', -1, 'selecionaTodosMtvCancelBolsa', 'selecionaTodosMtvCancelBolsa', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_cancelamento_bolsa', 'selecionadoMtvCancelBolsa', " + rowIndex + ", '" + id + "', 'selecionaTodosMtvCancelBolsa', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAcaoFollowup(value, rowIndex, obj) {
    try {
        var gridName = 'gridAcaoFollowUp';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAcaoFollowup');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_acao_follow_up", grid._by_idx[rowIndex].item.cd_acao_follow_up);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_acao_follow_up', 'selecionaAcaoFollowup', -1, 'selecionaTodosAcaoFollowup', 'selecionaTodosAcaoFollowup', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_acao_follow_up', 'selecionaAcaoFollowup', " + rowIndex + ", '" + id + "', 'selecionaTodosAcaoFollowup', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAnoEscolar(value, rowIndex, obj) {
    try {
        var gridName = 'gridAnoEscolar';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAnoEscolar');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_ano_escolar", grid._by_idx[rowIndex].item.cd_ano_escolar);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_ano_escolar', 'selecionaAnoEscolar', -1, 'selecionaTodosAnoEscolar', 'selecionaTodosAnoEscolar', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_ano_escolar', 'selecionaAnoEscolar', " + rowIndex + ", '" + id + "', 'selecionaTodosAnoEscolar', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMtvTransf(value, rowIndex, obj) {
    try {
        var gridName = 'gridMtvTransf';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMtvTransf');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_transferencia_aluno", grid._by_idx[rowIndex].item.cd_motivo_transferencia_aluno);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_transferencia_aluno', 'selecionadoMtvTransf', -1, 'selecionaTodosMtvTransf', 'selecionaTodosMtvTransf', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_transferencia_aluno', 'selecionadoMtvTransf', " + rowIndex + ", '" + id + "', 'selecionaTodosMtvTransf', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montaCadastroSec() {
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
        "dojo/domReady!",
        "dojo/parser"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        ready(function () {
            try{
                dijit.byId('tabContainer').resize();
                montarStatus("statusEsc");
                $(".Dialogo").css('display', 'none');
                var myStore = Cache(
                    JsonRest({
                        target: !hasValue(document.getElementById("pesquisaEsc").value) ? Endereco() + "/api/secretaria/getescolaridadesearch?descricao=&inicio=" + document.getElementById("inicioDescEsc").checked + "&status=" + 0 : Endereco() + "/api/secretaria/getescolaridadesearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaEsc").value) + "&inicio=" + document.getElementById("inicioDescEsc").checked + "&status=" + +retornaStatus("statusEsc"),
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                ), Memory({}));

                var gridEscolaridade = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                                { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                                //{ name: "Código", field: "cd_escolaridade", width: "75px", styles: "text-align: right; min-width:60px; max-width:75px;" },
                                { name: "Escolaridade", field: "no_escolaridade", width: "80%", styles: "min-width:80px;" },
                                { name: "Ativa", field: "escolaridade_ativa", width: "80px", styles: "text-align:center; min-width:40px; max-width: 50px;" }
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
                }, "gridEsc");
                // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                gridEscolaridade.pagination.plugin._paginator.plugin.connect(gridEscolaridade.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridEscolaridade, 'cd_escolaridade', 'selecionaTodos'); });
                gridEscolaridade.canSort = function (col) { return Math.abs(col) != 1; };
                gridEscolaridade.startup();
                gridEscolaridade.on("RowDblClick", function (evt) {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        getLimpar('#formEscolaridade');
                        apresentaMensagem('apresentadorMensagem', '');
                        keepValues(FORM_ESCOLARIDADE, item, gridEscolaridade, false);
                        dijit.byId("formularioEsc").show();
                        IncluirAlterar(0, 'divAlterarEsc', 'divIncluirEsc', 'divExcluirEsc', 'apresentadorMensagemEsc', 'divCancelarEsc', 'divClearEsc');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                IncluirAlterar(1, 'divAlterarEsc', 'divIncluirEsc', 'divExcluirEsc', 'apresentadorMensagemEsc', 'divCancelarEsc', 'divClearEsc');

                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosEsc(); } }, "incluirEsc");
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosEsc(); } }, "alterarEsc");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarEsc() });
                    }
                }, "deleteEsc");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparEscolaridade(); } }, "limparEsc");
                new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(FORM_ESCOLARIDADE, null, gridEscolaridade, null) } }, "cancelarEsc");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formularioEsc").hide(); } }, "fecharEsc");

                require(["dojo/aspect"], function(aspect){
                    aspect.after(gridEscolaridade, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_escolaridade', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridEsc')", gridEscolaridade.rowsPerPage * 3);
                    });
                });

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        try{
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            eventoEditar(gridEscolaridade.itensSelecionados);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        try{
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            eventoRemover(gridEscolaridade.itensSelecionados, 'DeletarEsc(itensSelecionados)');
                        } catch (e) {
                            postGerarLog(e);
                        }
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
                        buscarTodosItens(gridEscolaridade, 'todosItens', ['pesquisarEsc', 'relatorioEsc']);
                        PesquisarEsc(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridEsc', 'selecionado', 'cd_escolaridade', 'selecionaTodos', ['pesquisarEsc', 'relatorioEsc'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                montaBotoesSec();

                dijit.byId("tabContainer_tablist_menuBtn").on("click", function () {
                    try{
                        //dijit.byId("tabContainer_menu").on("_create", function () {
                        if (hasValue(dijit.byId("tabContainer_menu")) && dijit.byId("tabContainer_menu")._created) {
                            //alert("doido");
                            dijit.byId("tabEscolaridade_stcMi").on("click", function () {
                                abrirTabEscolaridade();
                            });
                            dijit.byId("tabMidia_stcMi").on("click", function () {
                                abrirTabMidia();
                            });
                            dijit.byId("tabTipoContato_stcMi").on("click", function () {
                                abrirTabTipoContato();
                            });
                            dijit.byId("tabMotivoMatricula_stcMi").on("click", function () {
                                abrirTabMotivoMatricula();
                            });
                            dijit.byId("tabMotivoNMatricula_stcMi").on("click", function () {
                                abrirTabMotivoNMatricula();
                            });
                            dijit.byId("tabMotivoBolsa_stcMi").on("click", function () {
                                abrirTabMotivoBolsa();
                            });
                            dijit.byId("tabMotivoCancelBolsa_stcMi").on("click", function () {
                                abrirTabMotivoCancelamentoBolsa();
                            });
                            dijit.byId("tabMotivoTransf_stcMi").on("click", function () {
                                abrirTabMotivoTransf();
                            });
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                var params = getParamterosURL();

                // Verifica pelo parametro get da URL se tem um link originado pela edição de aluno (motivo de não matrícula):
                if (hasValue(params["tipoRetornoLinkMotivoNao"])) {
                    // Posiciona na tab de motivo de não matrícula:
                    var tabs = dijit.byId("tabContainer");
                    var pane = dijit.byId("tabMotivoNMatricula");
                    tabs.selectChild(pane);
                    abrirTabMotivoNMatricula();

                    configuraMotivosNaoMatricula(dijit.byId('gridMtvNMat'));
                }
                    // Verifica pelo parametro get da URL se tem um link originado pela edição de aluno (motivo de não matrícula):
                else if (hasValue(params["tipoRetornoLinkMotivo"])) {
                    // Posiciona na tab de motivo de não matrícula:
                    var tabs = dijit.byId("tabContainer");
                    var pane = dijit.byId("tabMotivoMatricula");
                    tabs.selectChild(pane);
                    abrirTabMotivoMatricula();

                    configuraMotivosMatricula(dijit.byId('gridMtvMat'));
                }
                if (hasValue(dijit.byId("menuManual"))) {
                    var menuManual = dijit.byId("menuManual");
                    if (hasValue(menuManual.handler))
                        menuManual.handler.remove();
                    menuManual.handler = menuManual.on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323009', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['pesquisaEsc', 'statusEsc'], 'pesquisarEsc', ready);
                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function configuraMotivosNaoMatricula(gridMtvNMat) { //Método de link proveniente de aluno - motivos de não matrícula.
    require(["dojo/request/xhr", "dojox/json/ref", "dojo/ready"], function (xhr, ref, ready) {
        ready(function () {
            try{
                var parametros = getParamterosURL();
                var link = {
                    TipoRetorno: parametros['tipoRetornoLinkMotivoNao'],
                    Selecionados: null,
                    DadosRetorno: null
                };
                var parametroLink = '';

                if (hasValue(parametros['tipoRetornoLinkMotivoNao']))
                    parametroLink = '?tipoRetornoLinkMotivoNao=' + parametros['tipoRetornoLinkMotivoNao'];

                xhr.post(Endereco() + "/aluno/linkMotivoNaoMatricula" + parametroLink, {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson({
                        linkMotivoNaoMatricula: link
                    })
                }).then(function (data) {
                    try{
                        data = data.retorno;

                        if (hasValue(data.Selecionados))
                            gridMtvNMat.itensSelecionadosLink = data.Selecionados;
                        else
                            gridMtvNMat.itensSelecionadosLink = [];

                        // Atualiza com timeout para funcionar no IE:
                        setTimeout('atualizaGridMtvNMat()', 10);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, function (error) {
                    apresentaMensagem('apresentadorMensagem', error.response.data);
                });
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function configuraMotivosMatricula(gridMtvMat) { //Método de link proveniente de aluno - motivos de matrícula.
    require(["dojo/request/xhr", "dojox/json/ref", "dojo/ready"], function (xhr, ref, ready) {
        ready(function () {
            try{
                var parametros = getParamterosURL();
                var link = {
                    TipoRetorno: parametros['tipoRetornoLinkMotivo'],
                    Selecionados: null,
                    DadosRetorno: null
                };
                var parametroLink = '';

                if (hasValue(parametros['tipoRetornoLinkMotivo']))
                    parametroLink = '?tipoRetornoLinkMotivo=' + parametros['tipoRetornoLinkMotivo'];

                xhr.post(Endereco() + "/aluno/linkMotivoMatricula" + parametroLink, {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson({
                        linkMotivoMatricula: link
                    })
                }).then(function (data) {
                    data = data.retorno;

                    if (hasValue(data.Selecionados))
                        gridMtvMat.itensSelecionadosLink = data.Selecionados;
                    else
                        gridMtvMat.itensSelecionadosLink = [];

                    // Atualiza com timeout para funcionar no IE:
                    setTimeout('atualizaGridMtvMat()', 10);
                }, function (error) {
                    apresentaMensagem('apresentadorMensagem', error.response.data);
                });
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function atualizaGridMtvMat() {
    dijit.byId('gridMtvMat').update();
}

function atualizaGridMtvNMat() {
    dijit.byId('gridMtvNMat').update();
}

function eventoEditar(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formEscolaridade');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_ESCOLARIDADE, null, dijit.byId('gridEsc'), true);
            dijit.byId("formularioEsc").show();
            IncluirAlterar(0, 'divAlterarEsc', 'divIncluirEsc', 'divExcluirEsc', 'apresentadorMensagemEsc', 'divCancelarEsc', 'divClearEsc');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarMid(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formMidia');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_MIDIA, null, dijit.byId('gridMid'), true);
            dijit.byId("formularioMid").show();
            IncluirAlterar(0, 'divAlterarMid', 'divIncluirMid', 'divExcluirMid', 'apresentadorMensagemMid', 'divCancelarMid', 'divClearMid');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarTpContato(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formTpContato');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_TIPOCONTATO, null, dijit.byId('gridTpContato'), true);
            dijit.byId("formularioTpContato").show();
            IncluirAlterar(0, 'divAlterarTpContato', 'divIncluirTpContato', 'divExcluirTpContato', 'apresentadorMensagemTpContato', 'divCancelarTpContato', 'divClearTpContato');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarMtvMat(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formMtvMat');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_MOTIVOMATRICULA, null, dijit.byId('gridMtvMat'), true);
            dijit.byId("formularioMtvMat").show();
            IncluirAlterar(0, 'divAlterarMtvMat', 'divIncluirMtvMat', 'divExcluirMtvMat', 'apresentadorMensagemMtvMat', 'divCancelarMtvMat', 'divClearMtvMat');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarMtvNMat(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formMtvNMat');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_MOTIVONAOMATRICULA, null, dijit.byId('gridMtvNMat'), true);
            dijit.byId("formularioMtvNMat").show();
            IncluirAlterar(0, 'divAlterarMtvNMat', 'divIncluirMtvNMat', 'divExcluirMtvNMat', 'apresentadorMensagemMtvNMat', 'divCancelarMtvNMat', 'divClearMtvNMat');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarMtvBolsa(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formMtvBolsa');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_MOTIVOBOLSA, null, dijit.byId('gridMtvBolsa'), true);
            dijit.byId("formularioMtvBolsa").show();
            IncluirAlterar(0, 'divAlterarMtvBolsa', 'divIncluirMtvBolsa', 'divExcluirMtvBolsa', 'apresentadorMensagemMtvBolsa', 'divCancelarMtvBolsa', 'divClearMtvBolsa');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarMtvCancelBolsa(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formMtvCancelBolsa');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_MOTIVOCANCELBOLSA, null, dijit.byId('gridMtvCancelBolsa'), true);
            dijit.byId("formularioMtvCancelBolsa").show();
            IncluirAlterar(0, 'divAlterarMtvCancelBolsa', 'divIncluirMtvCancelBolsa', 'divExcluirMtvCancelBolsa', 'apresentadorMensagemMtvCancelBolsa', 'divCancelarMtvCancelBolsa', 'divClearMtvCancelBolsa');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarAcaoFollowup(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            limparAcaoFollowup();
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_ACAOFOLLOWUP, null, dijit.byId('gridAcaoFollowUp'), true);
            dijit.byId("formularioAcaoFollowup").show();
            IncluirAlterar(0, 'divAlterarAcaoFollow', 'divIncluirAcaoFollow', 'divExcluirAcaoFollow', 'apresentadorMensageAcaoFollow', 'divCancelarAcaoFollow', 'divClearAcaoFollow');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarAnoEscolar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {

            limparFormAnoEscolar();
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_ANOESCOLAR, null, dijit.byId('gridAnoEscolar'), true);

            dijit.byId("formNovoAnoEscolar").show();
            IncluirAlterar(0, 'divAlterarAnoEscolar', 'divIncluirAnoEscolar', 'divExcluirAnoEscolar', 'apresentadorMensageAnoEscolar', 'divCancelarAnoEscolar', 'divClearAnoEscolar');

        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarMtvTransf(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formMtvTransf');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_MOTIVOTRANSF, null, dijit.byId('gridMtvTransf'), true);
            dijit.byId("formularioMtvTransf").show();
            IncluirAlterar(0, 'divAlterarMtvTransf', 'divIncluirMtvTransf', 'divExcluirMtvTransf', 'apresentadorMensagemMtvTransf', 'divCancelarMtvTransf', 'divClearMtvTransf');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function montarCadastroMidia() {
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
        try{
            montarStatus("statusMid");
            $(".Dialogo").css('display', 'none');
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/secretaria/getmidiasearch?descricao=&inicio=" + document.getElementById("inicioDescMid").checked + "&status=1",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_midia"
                }), Memory({ idProperty: "cd_midia" })
            );

            var gridMidia = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosMid' style='display:none'/>", field: "selecionadoMid", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMid },
                    //{ name: "Código", field: "cd_midia", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                    { name: "Mídia", field: "no_midia", width: "75%" },
                    { name: "Ativa", field: "midia_ativa", width: "80px", styles: "text-align: center;" }
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
            }, "gridMid"); // make sure you have a target HTML element with this id
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridMidia.pagination.plugin._paginator.plugin.connect(gridMidia.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridMidia, 'cd_midia', 'selecionaTodosMid');
            });
            gridMidia.startup();
            gridMidia.canSort = function (col) { return Math.abs(col) != 1; };
            gridMidia.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formMidia');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_MIDIA, item, gridMidia, false);
                    dijit.byId("formularioMid").show();
                    IncluirAlterar(0, 'divAlterarMid', 'divIncluirMid', 'divExcluirMid', 'apresentadorMensagemMid', 'divCancelarMid', 'divClearMid');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarMid', 'divIncluirMid', 'divExcluirMid', 'apresentadorMensagemMid', 'divCancelarMid', 'divClearMid');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMid(); } }, "incluirMid");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMid(); } }, "alterarMid");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarMid() });
                }
            }, "deleteMid");
            new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparMidia(); } }, "limparMid");
            new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(FORM_MIDIA, null, gridMidia, null) } }, "cancelarMidia");
            new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formularioMid").hide(); } }, "fecharMidia");

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridMidia, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosMid').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_midia', 'selecionadoMid', -1, 'selecionaTodosMid', 'selecionaTodosMid', 'gridMid')", gridMidia.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarMid(gridMidia.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridMidia.itensSelecionados, 'DeletarMid(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasMid",
                dropDown: menu,
                id: "acoesRelacionadasMid"
            });
            dom.byId("linkAcoesMid").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridMidia, 'todosItensMid', ['pesquisarMidia', 'relatorioMid']); PesquisarMid(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridMid', 'selecionadoMid', 'cd_midia', 'selecionaTodosMid', ['pesquisarMidia', 'relatorioMid'], 'todosItensMid'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensMid",
                dropDown: menu,
                id: "todosItensMid"
            });
            dom.byId("linkSelecionadosMid").appendChild(button.domNode);
            adicionarAtalhoPesquisa(['pesquisaMidia', 'statusMid'], 'pesquisarMidia', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function montarCadastroTipoContato() {
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
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/secretaria/gettipocontatosearch?descricao=&inicio=" + document.getElementById("inicioDescTpContato").checked + "&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_contato"
                }), Memory({ idProperty: "cd_tipo_contato" })
           );
            montarStatus("statusTpContato");
            $(".Dialogo").css('display', 'none');
            var gridTipoContato = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosTpContato' style='display:none'/>", field: "selecionadoTpContato", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTpContato },
                   // { name: "Código", field: "cd_tipo_contato", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                    { name: "Tipo Contato", field: "dc_tipo_contato", width: "75%" },
                    { name: "Ativo", field: "tipo_contato_ativo", width: "80px", styles: "text-align: center;" }
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
            }, "gridTpContato"); // make sure you have a target HTML element with this id
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridTipoContato.pagination.plugin._paginator.plugin.connect(gridTipoContato.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridTipoContato, 'cd_tipo_contato', 'selecionadoTpContato');
            });
            gridTipoContato.startup();
            gridTipoContato.canSort = function (col) { return Math.abs(col) != 1; };
            gridTipoContato.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formTpContato');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_TIPOCONTATO, item, gridTipoContato, false);
                    dijit.byId("formularioTpContato").show();
                    IncluirAlterar(0, 'divAlterarTpContato', 'divIncluirTpContato', 'divExcluirTpContato', 'apresentadorMensagemTpContato', 'divCancelarTpContato', 'divClearTpContato');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarTpContato', 'divIncluirTpContato', 'divExcluirTpContato', 'apresentadorMensagemTpContato', 'divCancelarTpContato', 'divClearTpContato');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosTpContato(); } }, "incluirTpContato");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosTpContato(); } }, "alterarTpContato");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarTpContato() });
                }
            }, "deleteTpContato");
            new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparTpContato(); } }, "limparTpContato");
            new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(FORM_TIPOCONTATO, null, gridTipoContato, null) } }, "cancelarTpContato");
            new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formularioTpContato").hide(); } }, "fecharTpContato");

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridTipoContato, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosTpContato').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_tipo_contato', 'selecionadoTpContato', -1, 'selecionaTodosTpContato', 'selecionaTodosTpContato', 'gridTpContato')", gridTipoContato.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarTpContato(gridTipoContato.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridTipoContato.itensSelecionados, 'DeletarTpContato(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasTpContato",
                dropDown: menu,
                id: "acoesRelacionadasTpContato"
            });
            dom.byId("linkAcoesTpContato").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridTipoContato, 'todosItensTpContato', ['pesquisarTipoContato', 'relatorioTipoContato']); PesquisarTpContato(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridTpContato', 'selecionadoTpContato', 'cd_tipo_contato', 'selecionaTodosTpContato', ['pesquisarTipoContato', 'relatorioTipoContato'], 'todosItensTpContato'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensTpContato",
                dropDown: menu,
                id: "todosItensTpContato"
            });
            dom.byId("linkSelecionadosTpContato").appendChild(button.domNode);
            adicionarAtalhoPesquisa(['pesquisaTipoContato', 'statusTpContato'], 'pesquisarTipoContato', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function montarCadastroMotivoMatricula() {
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
        "dojo/domReady!",
        "dojo/parser"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/secretaria/getmotivomatriculasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvMat").checked + "&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_motivo_matricula"
                }
                ), Memory({ idProperty: "cd_motivo_matricula" }));
            montarStatus("statusMtvMat");
            $(".Dialogo").css('display', 'none');
            var gridMotivoMatricula = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                            { name: "<input id='selecionaTodosMtvMat' style='display:none'/>", field: "selecionadoMtvMat", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMtvMat },
                            //{ name: "Código", field: "cd_motivo_matricula", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                            { name: "Motivo da Matrícula", field: "dc_motivo_matricula", width: "75%" },
                            { name: "Ativo", field: "motivo_matricula_ativo", width: "80px", styles: "text-align: center;" }
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
            }, "gridMtvMat");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridMotivoMatricula.pagination.plugin._paginator.plugin.connect(gridMotivoMatricula.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridMotivoMatricula, 'cd_motivo_matricula', 'selecionaTodosMtvMat');
            });
            gridMotivoMatricula.startup();
            gridMotivoMatricula.canSort = function (col) { return Math.abs(col) != 1; };
            gridMotivoMatricula.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formMtvMat');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_MOTIVOMATRICULA, item, gridMotivoMatricula, false);
                    dijit.byId("formularioMtvMat").show();
                    IncluirAlterar(0, 'divAlterarMtvMat', 'divIncluirMtvMat', 'divExcluirMtvMat', 'apresentadorMensagemMtvMat', 'divCancelarMtvMat', 'divClearMtvMat');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarMtvMat', 'divIncluirMtvMat', 'divExcluirMtvMat', 'apresentadorMensagemMtvMat', 'divCancelarMtvMat', 'divClearMtvMat');
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMtvMat(); } }, "incluirMtvMat");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMtvMat(); } }, "alterarMtvMat");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarMtvMat() });
                }
            }, "deleteMtvMat");
            new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparMtvMat(); } }, "limparMtvMat");
            new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(FORM_MOTIVOMATRICULA, null, gridMotivoMatricula, null) } }, "cancelarMtvMat");
            new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formularioMtvMat").hide(); } }, "fecharMtvMat");

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridMotivoMatricula, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosMtvMat').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_motivo_matricula', 'selecionadoMtvMat', -1, 'selecionaTodosMtvMat', 'selecionaTodosMtvMat', 'gridMtvMat')", gridMotivoMatricula.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarMtvMat(gridMotivoMatricula.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridMotivoMatricula.itensSelecionados, 'DeletarMtvMat(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);
            var parametros = getParamterosURL();

            if (hasValue(parametros['tipoRetornoLinkMotivo'])) {
                var acaoAluno = new MenuItem({
                    label: "Aluno",
                    onClick: function () { redirecionaAlunoMotivo(parametros['tipoRetorno']); }
                });
                menu.addChild(acaoAluno);
            }

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasMtvMat",
                dropDown: menu,
                id: "acoesRelacionadasMtvMat"
            });
            dom.byId("linkAcoesMtvMat").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridMotivoMatricula, 'todosItensMtvMat', ['pesquisarMtvMat', 'relatorioMtvMat']); PesquisarMtvMat(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridMtvMat', 'selecionadoMtvMat', 'cd_motivo_matricula', 'selecionaTodosMtvMat', ['pesquisarMtvMat', 'relatorioMtvMat'], 'todosItensMtvMat'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensMtvMat",
                dropDown: menu,
                id: "todosItensMtvMat"
            });
            dom.byId("linkSelecionadosMtvMat").appendChild(button.domNode);
            adicionarAtalhoPesquisa(['pesquisaMtvMat', 'statusMtvMat'], 'pesquisarMtvMat', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function montarCadastroMotivoNMatricula() {
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
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/secretaria/getmotivonmatriculasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvNMat").checked + "&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_motivo_nao_matricula"
                }
            ), Memory({ idProperty: "cd_motivo_nao_matricula" }));
            montarStatus("statusMtvNMat");
            $(".Dialogo").css('display', 'none');
            var gridMotivoNMatricula = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                            { name: "<input id='selecionaTodosMtvNMat' style='display:none'/>", field: "selecionadoMtvNMat", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMtvNMat },
                           // { name: "Código", field: "cd_motivo_nao_matricula", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                            { name: "Motivo da Não Matrícula", field: "dc_motivo_nao_matricula", width: "75%" },
                            { name: "Ativo", field: "motivo_nao_matricula_ativo", width: "80px", styles: "text-align: center;" }
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
            }, "gridMtvNMat");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridMotivoNMatricula.pagination.plugin._paginator.plugin.connect(gridMotivoNMatricula.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridMotivoNMatricula, 'cd_motivo_nao_matricula', 'selecionaTodosMtvNMat');
            });
            gridMotivoNMatricula.startup();
            gridMotivoNMatricula.canSort = function (col) { return Math.abs(col) != 1; };
            gridMotivoNMatricula.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formMtvNMat');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_MOTIVONAOMATRICULA, item, gridMotivoNMatricula, false);
                    dijit.byId("formularioMtvNMat").show();
                    IncluirAlterar(0, 'divAlterarMtvNMat', 'divIncluirMtvNMat', 'divExcluirMtvNMat', 'apresentadorMensagemMtvNMat', 'divCancelarMtvNMat', 'divClearMtvNMat');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarMtvNMat', 'divIncluirMtvNMat', 'divExcluirMtvNMat', 'apresentadorMensagemMtvNMat', 'divCancelarMtvNMat', 'divClearMtvNMat');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMtvNMat(); } }, "incluirMtvNMat");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMtvNMat(); } }, "alterarMtvNMat");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarMtvNMat() });
                }
            }, "deleteMtvNMat");
            new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparMtvNMat(); } }, "limparMtvNMat");
            new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(FORM_MOTIVONAOMATRICULA, null, gridMotivoNMatricula, null) } }, "cancelarMtvNMat");
            new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formularioMtvNMat").hide(); } }, "fecharMtvNMat");

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridMotivoNMatricula, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosMtvNMat').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_motivo_nao_matricula', 'selecionadoMtvNMat', -1, 'selecionaTodosMtvNMat', 'selecionaTodosMtvNMat', 'gridMtvNMat')", gridMotivoNMatricula.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarMtvNMat(dijit.byId("gridMtvNMat").itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(dijit.byId("gridMtvNMat").itensSelecionados, 'DeletarMtvNMat(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            // Inclui o link de Aluno (retorno do link de motivo de não matricula):
            var parametros = getParamterosURL();

            if (hasValue(parametros['tipoRetornoLinkMotivoNao'])) {
                var acaoAluno = new MenuItem({
                    label: "Aluno",
                    onClick: function () { redirecionaAluno(parametros['tipoRetorno']); }
                });
                menu.addChild(acaoAluno);
            }

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasMtvNMat",
                dropDown: menu,
                id: "acoesRelacionadasMtvNMat"
            });
            dom.byId("linkAcoesMtvNMat").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridMotivoNMatricula, 'todosItensMtvNMat', ['pesquisarMtvNMat', 'relatorioMtvNMat']); PesquisarMtvNMat(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridMtvNMat', 'selecionadoMtvNMat', 'cd_motivo_nao_matricula', 'selecionaTodosMtvNMat', ['pesquisarMtvNMat', 'relatorioMtvNMat'], 'todosItensMtvNMat'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensMtvNMat",
                dropDown: menu,
                id: "todosItensMtvNMat"
            });
            dom.byId("linkSelecionadosMtvNMat").appendChild(button.domNode);
            adicionarAtalhoPesquisa(['pesquisaMtvNMat', 'statusMtvNMat'], 'pesquisarMtvNMat', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function redirecionaAlunoMotivo(tipoRetorno) {
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        try{
            var selecionados = [];
            var selecionadosRetorno = [];
            var gridMotvoMat = dijit.byId('gridMtvMat');
            var itensSelecionados = dijit.byId('gridMtvMat').itensSelecionados;

            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Selecione algum motivo de matrícula para relacioná-lo ao aluno.');
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }

            if (hasValue(gridMotvoMat.itensSelecionadosLink) && gridMotvoMat.itensSelecionadosLink.length > 0) {
                selecionados = gridMotvoMat.itensSelecionadosLink;
                for (var i = 0; i < selecionados.length; i++) {
                    var itemRetorno = {
                        cd_motivo_matricula: selecionados[i].cd_motivo_matricula,
                        dc_motivo_matricula: selecionados[i].dc_motivo_matricula
                    }
                    selecionadosRetorno.push(itemRetorno)
                }
            }

            for (var i = 0; i < itensSelecionados.length; i++) {
                var newSelecionado = {
                    cd_motivo_matricula: itensSelecionados[i].cd_motivo_matricula,
                    dc_motivo_matricula: itensSelecionados[i].dc_motivo_matricula
                }
                // selecionados.push(newSelecionado);
                insertObjSort(selecionadosRetorno, 'cd_motivo_matricula', newSelecionado, false);
            }

            xhr.post(Endereco() + "/aluno/linkMotivoMatriculaRetorno", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    TipoRetorno: tipoRetorno,
                    Selecionados: selecionadosRetorno,
                    DadosRetorno: null
                })
            }).then(function (data) {
                try{
                    if (hasValue(data.retorno))
                        window.location = data.retorno;
                    else
                        apresentaMensagem('apresentadorMensagem', data);
                } catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function redirecionaAluno(tipoRetorno) {
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        try{
            var selecionados = [];
            var selecionadosRetorno = [];
            var gridMotvoNaoMat = dijit.byId('gridMtvNMat');
            var itensSelecionados = dijit.byId('gridMtvNMat').itensSelecionados;

            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Selecione algum motivo de não matrícula para relacioná-lo ao aluno.');
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }

            if (hasValue(gridMotvoNaoMat.itensSelecionadosLink) && gridMotvoNaoMat.itensSelecionadosLink.length > 0){
                selecionados = gridMotvoNaoMat.itensSelecionadosLink;
                for (var i = 0; i < selecionados.length; i++) {
                    var itemRetorno = {
                        cd_motivo_nao_matricula: selecionados[i].cd_motivo_nao_matricula,
                        dc_motivo_nao_matricula: selecionados[i].dc_motivo_nao_matricula,
                        id_motivo_nao_matricula_ativo: selecionados[i].id_motivo_nao_matricula_ativo,
                        motivo_nao_matricula_ativo: selecionados[i].motivo_nao_matricula_ativo
                    }
                    selecionadosRetorno.push(itemRetorno);
                }
            }
            for (var i = 0; i < itensSelecionados.length; i++) {
                var newSelecionado = {
                    cd_motivo_nao_matricula: itensSelecionados[i].cd_motivo_nao_matricula,
                    dc_motivo_nao_matricula: itensSelecionados[i].dc_motivo_nao_matricula,
                    id_motivo_nao_matricula_ativo: itensSelecionados[i].id_motivo_nao_matricula_ativo,
                    motivo_nao_matricula_ativo: itensSelecionados[i].motivo_nao_matricula_ativo
                }
                //selecionados.push(newSelecionado);
                insertObjSort(selecionadosRetorno, 'cd_motivo_nao_matricula', newSelecionado, false);
            }

            xhr.post(Endereco() + "/aluno/linkMotivoNaoMatriculaRetorno", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    TipoRetorno: tipoRetorno,
                    Selecionados: selecionadosRetorno,
                    DadosRetorno: null
                })
            }).then(function (data) {
                try{
                    if (hasValue(data.retorno))
                        window.location = data.retorno;
                    else
                        apresentaMensagem('apresentadorMensagem', data);
                } catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function montarCadastroMotivoBolsa() {
    require([
            "dojo/dom",
            "dojox/grid/EnhancedGrid",
            "dojox/grid/enhanced/plugins/Pagination",
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory",
            "dojo/dom-attr",
            "dijit/form/Button",
            "dijit/form/TextBox",
            "dijit/form/DropDownButton",
            "dijit/DropDownMenu",
            "dijit/MenuItem",
            "dojo/ready",
            "dojo/domReady!",
            "dojo/parser"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, domAttr, Button, TextBox, DropDownButton, DropDownMenu, MenuItem, ready) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/secretaria/getmotivobolsasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvBolsa").checked + "&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_motivo_bolsa"
                }
            ), Memory({ idProperty: "cd_motivo_bolsa" }));
            montarStatus("statusMtvBolsa");
            $(".Dialogo").css('display', 'none');
            var gridMotivoBolsa = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                            { name: "<input id='selecionaTodosMtvBolsa' style='display:none'/>", field: "selecionadoMtvBolsa", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMtvBolsa },
                            //{ name: "Código", field: "cd_motivo_bolsa", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                            { name: "Motivo da Bolsa", field: "dc_motivo_bolsa", width: "75%" },
                            { name: "Ativo", field: "motivo_bolsa_ativo", width: "80px", styles: "text-align: center;" }
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
            }, "gridMtvBolsa");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridMotivoBolsa.pagination.plugin._paginator.plugin.connect(gridMotivoBolsa.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridMotivoBolsa, 'cd_motivo_bolsa', 'selecionaTodosMtvBolsa');
            });
            gridMotivoBolsa.startup();
            gridMotivoBolsa.canSort = function (col) { return Math.abs(col) != 1; };
            gridMotivoBolsa.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formMtvBolsa');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_MOTIVOBOLSA, item, gridMotivoBolsa, false);
                    dijit.byId("formularioMtvBolsa").show();
                    IncluirAlterar(0, 'divAlterarMtvBolsa', 'divIncluirMtvBolsa', 'divExcluirMtvBolsa', 'apresentadorMensagemMtvBolsa', 'divCancelarMtvBolsa', 'divClearMtvBolsa');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarMtvBolsa', 'divIncluirMtvBolsa', 'divExcluirMtvBolsa', 'apresentadorMensagemMtvBolsa', 'divCancelarMtvBolsa', 'divClearMtvBolsa');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMtvBolsa(); } }, "incluirMtvBolsa");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMtvBolsa(); } }, "alterarMtvBolsa");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarMtvBolsa() });
                }
            }, "deleteMtvBolsa");
            new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparMtvBolsa(); } }, "limparMtvBolsa");
            new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(FORM_MOTIVOBOLSA, null, gridMotivoBolsa, null) } }, "cancelarMtvBolsa");
            new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formularioMtvBolsa").hide(); } }, "fecharMtvBolsa");

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridMotivoBolsa, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosMtvBolsa').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_motivo_bolsa', 'selecionadoMtvBolsa', -1, 'selecionaTodosMtvBolsa', 'selecionaTodosMtvBolsa', 'gridMtvBolsa')", gridMotivoBolsa.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarMtvBolsa(gridMotivoBolsa.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridMotivoBolsa.itensSelecionados, 'DeletarMtvBolsa(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasMtvBolsa",
                dropDown: menu,
                id: "acoesRelacionadasMtvBolsa"
            });
            dom.byId("linkAcoesMtvBolsa").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridMotivoBolsa, 'todosItensMtvBolsa', ['pesquisarMtvBolsa', 'relatorioMtvBolsa']); PesquisarMtvBolsa(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridMtvBolsa', 'selecionadoMtvBolsa', 'cd_motivo_bolsa', 'selecionaTodosMtvBolsa', ['pesquisarMtvBolsa', 'relatorioMtvBolsa'], 'todosItensMtvBolsa'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensMtvBolsa",
                dropDown: menu,
                id: "todosItensMtvBolsa"
            });
            dom.byId("linkSelecionadosMtvBolsa").appendChild(button.domNode);
            adicionarAtalhoPesquisa(['pesquisaMtvBolsa', 'statusMtvBolsa'], 'pesquisarMtvBolsa', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function montarCadastroMotivoCancelamentoBolsa() {
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
        var myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/secretaria/getmotivocancelbolsasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvCancelBolsa").checked + "&status=1",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_motivo_cancelamento_bolsa"
            }
        ), Memory({ idProperty: "cd_motivo_cancelamento_bolsa" }));
        montarStatus("statusMtvCancelBolsa");
        $(".Dialogo").css('display', 'none');
        var gridMotivoCancelBolsa = new EnhancedGrid({
            store: ObjectStore({ objectStore: myStore }),
            structure: [
                        { name: "<input id='selecionaTodosMtvCancelBolsa' style='display:none'/>", field: "selecionadoMtvCancelBolsa", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMtvCancelBolsa },
                        //{ name: "Código", field: "cd_motivo_cancelamento_bolsa", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                        { name: "Motivo Cancelamento da Bolsa", field: "dc_motivo_cancelamento_bolsa", width: "75%" },
                        { name: "Ativo", field: "motivo_cancelamento_bolsa_ativo", width: "80px", styles: "text-align: center;" }
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
        }, "gridMtvCancelBolsa");
        // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
        gridMotivoCancelBolsa.pagination.plugin._paginator.plugin.connect(gridMotivoCancelBolsa.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
            verificaMostrarTodos(evt, gridMotivoCancelBolsa, 'cd_motivo_matricula', 'selecionaTodosMtvCancelBolsa');
        });
        gridMotivoCancelBolsa.startup();
        gridMotivoCancelBolsa.canSort = function (col) { return Math.abs(col) != 1; };
        gridMotivoCancelBolsa.on("RowDblClick", function (evt) {
            try{
                if (!eval(MasterGeral())) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    return;
                }
                getLimpar('#formMtvCancelBolsa');
                apresentaMensagem('apresentadorMensagem', '');
                var idx = evt.rowIndex,
                item = this.getItem(idx),
                store = this.store;
                keepValues(FORM_MOTIVOCANCELBOLSA, item, gridMotivoCancelBolsa, false);
                dijit.byId("formularioMtvCancelBolsa").show();
                IncluirAlterar(0, 'divAlterarMtvCancelBolsa', 'divIncluirMtvCancelBolsa', 'divExcluirMtvCancelBolsa', 'apresentadorMensagemMtvCancelBolsa', 'divCancelarMtvCancelBolsa', 'divClearMtvCancelBolsa');
            } catch (e) {
                postGerarLog(e);
            }
        }, true);
        IncluirAlterar(1, 'divAlterarMtvCancelBolsa', 'divIncluirMtvCancelBolsa', 'divExcluirMtvCancelBolsa', 'apresentadorMensagemMtvCancelBolsa', 'divCancelarMtvCancelBolsa', 'divClearMtvCancelBolsa');
        new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMtvCancelBolsa(); } }, "incluirMtvCancelBolsa");
        new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMtvCancelBolsa(); } }, "alterarMtvCancelBolsa");
        new Button({
            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarMtvCancelBolsa() });
            }
        }, "deleteMtvCancelBolsa");
        new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparMtvCancelBolsa(); } }, "limparMtvCancelBolsa");
        new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(FORM_MOTIVOCANCELBOLSA, null, gridMotivoCancelBolsa, null) } }, "cancelarMtvCancelBolsa");
        new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formularioMtvCancelBolsa").hide(); } }, "fecharMtvCancelBolsa");

        require(["dojo/aspect"], function (aspect) {
            aspect.after(gridMotivoCancelBolsa, "_onFetchComplete", function () {
                // Configura o check de todos:
                if (dojo.byId('selecionaTodosMtvCancelBolsa').type == 'text')
                    setTimeout("configuraCheckBox(false, 'cd_motivo_cancelamento_bolsa', 'selecionadoMtvCancelBolsa', -1, 'selecionaTodosMtvCancelBolsa', 'selecionaTodosMtvCancelBolsa', 'gridMtvCancelBolsa')", gridMotivoCancelBolsa.rowsPerPage * 3);
            });
        });

        // Adiciona link de ações:
        var menu = new DropDownMenu({ style: "height: 25px" });
        var acaoEditar = new MenuItem({
            label: "Editar",
            onClick: function () {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    eventoEditarMtvCancelBolsa(gridMotivoCancelBolsa.itensSelecionados);
                } catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menu.addChild(acaoEditar);

        var acaoRemover = new MenuItem({
            label: "Excluir",
            onClick: function () {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    eventoRemover(gridMotivoCancelBolsa.itensSelecionados, 'DeletarMtvCancelBolsa(itensSelecionados)');
                } catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menu.addChild(acaoRemover);

        var button = new DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasMtvCancelBolsa",
            dropDown: menu,
            id: "acoesRelacionadasMtvCancelBolsa"
        });
        dom.byId("linkAcoesMtvCancelBolsa").appendChild(button.domNode);

        // Adiciona link de selecionados:
        menu = new DropDownMenu({ style: "height: 25px" });
        var menuTodosItens = new MenuItem({
            label: "Todos Itens",
            onClick: function () { buscarTodosItens(gridMotivoCancelBolsa, 'todosItensMtvCancelBolsa', ['pesquisarMtvCancelBolsa', 'relatorioMtvCancelBolsa']); PesquisarMtvCancelBolsa(false); }
        });
        menu.addChild(menuTodosItens);

        var menuItensSelecionados = new MenuItem({
            label: "Itens Selecionados",
            onClick: function () { buscarItensSelecionados('gridMtvCancelBolsa', 'selecionadoMtvCancelBolsa', 'cd_motivo_cancelamento_bolsa', 'selecionaTodosMtvCancelBolsa', ['pesquisarMtvCancelBolsa', 'relatorioMtvCancelBolsa'], 'todosItensMtvCancelBolsa'); }
        });
        menu.addChild(menuItensSelecionados);

        var button = new DropDownButton({
            label: "Todos Itens",
            name: "todosItensMtvCancelBolsa",
            dropDown: menu,
            id: "todosItensMtvCancelBolsa"
        });
        dom.byId("linkSelecionadosMtvCancelBolsa").appendChild(button.domNode);
        adicionarAtalhoPesquisa(['pesquisaMtvCancelBolsa', 'statusMtvCancelBolsa'], 'pesquisarMtvCancelBolsa', ready);
    });
}

function montarCadastroMotivoTransf() {
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
                    target: Endereco() + "/api/secretaria/getmotivotransferenciasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvTransf").checked + "&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_motivo_nao_matricula"
                }
                ), Memory({ idProperty: "cd_motivo_transferencia_aluno" }));
            montarStatus("statusMtvTransf");
            $(".Dialogo").css('display', 'none');
            var gridMotivoTransf = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosMtvTransf' style='display:none'/>", field: "selecionadoMtvTransf", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMtvTransf },
                    { name: "Descrição", field: "dc_motivo_transferencia_aluno", width: "75%" },
                    { name: "Ativo", field: "motivo_transferencia_ativo", width: "80px", styles: "text-align: center;" }
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
            }, "gridMtvTransf");
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridMotivoTransf.pagination.plugin._paginator.plugin.connect(gridMotivoTransf.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridMotivoTransf, 'cd_motivo_transferencia_aluno', 'selecionaTodosMtvTransf');
            });
            gridMotivoTransf.startup();
            gridMotivoTransf.canSort = function (col) { return Math.abs(col) != 1; };
            gridMotivoTransf.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formMtvTransf');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                        item = this.getItem(idx),
                        store = this.store;
                    keepValues(FORM_MOTIVOTRANSF, item, gridMotivoTransf, false);
                    dijit.byId("formularioMtvTransf").show();
                    IncluirAlterar(0, 'divAlterarMtvTransf', 'divIncluirMtvTransf', 'divExcluirMtvTransf', 'apresentadorMensagemMtvTransf', 'divCancelarMtvTransf', 'divClearMtvTransf');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarMtvTransf', 'divIncluirMtvTransf', 'divExcluirMtvTransf', 'apresentadorMensagemMtvTransf', 'divCancelarMtvTransf', 'divClearMtvTransf');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMtvTransf(); } }, "incluirMtvTransf");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosMtvTransf(); } }, "alterarMtvTransf");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarMtvTransf() });
                }
            }, "deleteMtvTransf");
            new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparMtvTransf(); } }, "limparMtvTransf");
            new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(FORM_MOTIVOTRANSF, null, gridMotivoTransf, null) } }, "cancelarMtvTransf");
            new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formularioMtvTransf").hide(); } }, "fecharMtvTransf");

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridMotivoTransf, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosMtvTransf').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_motivo_transferencia_aluno', 'selecionadoMtvTransf', -1, 'selecionaTodosMtvTransf', 'selecionaTodosMtvTransf', 'gridMtvNMat')", gridMotivoTransf.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarMtvTransf(dijit.byId("gridMtvTransf").itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(dijit.byId("gridMtvTransf").itensSelecionados, 'DeletarMtvTransf(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            // Inclui o link de Aluno (retorno do link de motivo de não matricula):
            var parametros = getParamterosURL();

            if (hasValue(parametros['tipoRetornoLinkMotivoTransf'])) {
                var acaoAluno = new MenuItem({
                    label: "Aluno",
                    onClick: function () { redirecionaAluno(parametros['tipoRetorno']); }
                });
                menu.addChild(acaoAluno);
            }

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasMtvTransf",
                dropDown: menu,
                id: "acoesRelacionadasMtvTransf"
            });
            dom.byId("linkAcoesMtvTransf").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridMotivoTransf, 'todosItensMtvTransf', ['pesquisarMtvTransf', 'relatorioMtvTransf']); PesquisarMtvTransf(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridMtvTransf', 'selecionadoMtvTransf', 'cd_motivo_transferencia_aluno', 'selecionaTodosMtvTransf', ['pesquisarMtvTransf', 'relatorioMtvTransf'], 'todosItensMtvTransf'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensMtvTransf",
                dropDown: menu,
                id: "todosItensMtvTransf"
            });
            dom.byId("linkSelecionadosMtvTransf").appendChild(button.domNode);
            adicionarAtalhoPesquisa(['pesquisaMtvTransf', 'statusMtvTransf'], 'pesquisarMtvTransf', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirTabMidia() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirMid').className))
            montarCadastroMidia();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323010', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabTipoContato() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirTpContato').className))
            montarCadastroTipoContato();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323011', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabMotivoMatricula() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirMtvMat').className))
            montarCadastroMotivoMatricula();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323012', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabMotivoNMatricula() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirMtvNMat').className))
            montarCadastroMotivoNMatricula();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323013', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabMotivoBolsa() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirMtvBolsa').className))
            montarCadastroMotivoBolsa();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323014', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabMotivoCancelamentoBolsa() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirMtvCancelBolsa').className))
            montarCadastroMotivoCancelamentoBolsa();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323015', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabEscolaridade() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323009', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabAcaoFollow() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirAcaoFollow').className))
            montarAcaoFollow();
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTabAnoEscolar() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        populaEscolaridadePossuiAnoEscolar();
        populaEscolaridadeNovoAnoEscolar();
        if (!hasValue(document.getElementById('incluirAnoEscolar').className))
            montarAnoEscolar();
      
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTabMotivoTransf() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirMtvTransf').className))
            montarCadastroMotivoTransf();
    } catch (e) {
        postGerarLog(e);
    }
}

function selecionaTab(e) {
    try{
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabEscolaridade')
            abrirTabEscolaridade();
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMidia')
            abrirTabMidia();
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTipoContato')
            abrirTabTipoContato();
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMotivoMatricula')
            abrirTabMotivoMatricula();
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMotivoNMatricula')
            abrirTabMotivoNMatricula();
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMotivoBolsa')
            abrirTabMotivoBolsa();
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMotivoCancelBolsa')
            abrirTabMotivoCancelamentoBolsa();
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabAcaoFollow')
            abrirTabAcaoFollow();
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabAnoEscolar')
            abrirTabAnoEscolar();
        else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMotivoTransf')
            abrirTabMotivoTransf();
    } catch (e) {
        postGerarLog(e);
    }
}

function limparEscolaridade() {
    try {
        apresentaMensagem('apresentadorMensagemEsc', null);
        getLimpar('#formEscolaridade');
        clearForm('formEscolaridade');
        IncluirAlterar(1, 'divAlterarEsc', 'divIncluirEsc', 'divExcluirEsc', 'apresentadorMensagemEsc', 'divCancelarEsc', 'divClearEsc');
        document.getElementById("cd_escolaridade").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function limparMidia() {
    try {
        apresentaMensagem('apresentadorMensagemMid', null);
        getLimpar('#formMidia');
        clearForm('formMidia');
        IncluirAlterar(1, 'divAlterarMid', 'divIncluirMid', 'divExcluirMid', 'apresentadorMensagemMid', 'divCancelarMid', 'divClearMid');
        document.getElementById("cd_midia").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function limparTpContato() {
    try {
        apresentaMensagem('apresentadorMensagemTpContato', null);
        getLimpar('#formTpContato');
        IncluirAlterar(1, 'divAlterarTpContato', 'divIncluirTpContato', 'divExcluirTpContato', 'apresentadorMensagemTpContato', 'divCancelarTpContato', 'divClearTpContato');
        document.getElementById("cd_tipo_contato").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function limparMtvMat() {
    try {
        apresentaMensagem('apresentadorMensagemMtvMat', null);
        getLimpar('#formMtvMat');
        clearForm('formMtvMat');
        IncluirAlterar(1, 'divAlterarMtvMat', 'divIncluirMtvMat', 'divExcluirMtvMat', 'apresentadorMensagemMtvMat', 'divCancelarMtvMat', 'divClearMtvMat');
        document.getElementById("cd_motivo_matricula").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function limparMtvNMat() {
    try {
        apresentaMensagem('apresentadorMensagemMtvNMat', null);
        getLimpar('#formMtvMat');
        clearForm('formMtvMat');
        IncluirAlterar(1, 'divAlterarMtvNMat', 'divIncluirMtvNMat', 'divExcluirMtvNMat', 'apresentadorMensagemMtvNMat', 'divCancelarMtvNMat', 'divClearMtvNMat');
        document.getElementById("cd_motivo_nao_matricula").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function limparMtvBolsa() {
    try {
        apresentaMensagem('apresentadorMensagemMtvBolsa', null);
        getLimpar('#formMtvBolsa');
        clearForm('formMtvBolsa');
        IncluirAlterar(1, 'divAlterarMtvBolsa', 'divIncluirMtvBolsa', 'divExcluirMtvBolsa', 'apresentadorMensagemMtvBolsa', 'divCancelarMtvBolsa', 'divClearMtvBolsa');
        document.getElementById("cd_motivo_bolsa").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function limparMtvCancelBolsa() {
    try {
        apresentaMensagem('apresentadorMensagemMtvCancelBolsa', null);
        getLimpar('#formMtvCancelBolsa');
        clearForm('formMtvCancelBolsa');
        IncluirAlterar(1, 'divAlterarMtvCancelBolsa', 'divIncluirMtvCancelBolsa', 'divExcluirMtvCancelBolsa', 'apresentadorMensagemMtvCancelBolsa', 'divCancelarMtvCancelBolsa', 'divClearMtvCancelBolsa');
        document.getElementById("cd_motivo_cancelamento_bolsa").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function limparAcaoFollowup() {
    try {
        apresentaMensagem('apresentadorMensagemAcaoFollowUp', null);
        getLimpar('#formAcaoFollowup');
        clearForm('formAcaoFollowup');
        IncluirAlterar(1, 'divAlterarAcaoFollow', 'divIncluirAcaoFollow', 'divExcluirAcaoFollow', 'apresentadorMensageAcaoFollow', 'divCancelarAcaoFollow', 'divClearAcaoFollow');
        document.getElementById("cd_acao_follow_up").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}

function limparFormAnoEscolar() {
    try {
        apresentaMensagem('apresentadorMensagemAnoEscolar', null);
        getLimpar('#formAnoEscolar');
        clearForm('formAnoEscolar');
        document.getElementById("cd_ano_escolar").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}


function limparMtvTransf() {
    try {
        apresentaMensagem('apresentadorMensagemMtvTransf', null);
        getLimpar('#formMtvTransf');
        clearForm('formMtvTransf');
        IncluirAlterar(1, 'divAlterarMtvTransf', 'divIncluirMtvTransf', 'divExcluirMtvTransf', 'apresentadorMensagemMtvTransf', 'divCancelarMtvTransf', 'divClearMtvTransf');
        document.getElementById("cd_motivo_transferencia_aluno").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}

function EnviarDadosEsc() {
    try{
        if (document.getElementById("divAlterarEsc").style.display == "")
            AlterarEsc();
        else
            IncluirEsc();
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosMtvMat() {
    try{
        if (document.getElementById("divAlterarMtvMat").style.display == "")
            AlterarMtvMat();
        else
            IncluirMtvMat();
    } catch (e) {
        postGerarLog(e);
    }
}

function EnviarDadosTpContato() {
    try{
        if (document.getElementById("divAlterarTpContato").style.display == "")
            AlterarTpContato();
        else
            IncluirTpContato();
    } catch (e) {
        postGerarLog(e);
    }
}

function EnviarDadosMid() {
    try{
        if (document.getElementById("divAlterarMid").style.display == "")
            AlterarMid();
        else
            IncluirMid();
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosMtvNMat() {
    try{
        if (document.getElementById("divAlterarMtvNMat").style.display == "")
            AlterarMtvNMat();
        else
            IncluirMtvNMat();
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosMtvBolsa() {
    try{
        if (document.getElementById("divAlterarMtvBolsa").style.display == "")
            AlterarMtvBolsa();
        else
            IncluirMtvBolsa();
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosMtvCancelBolsa() {
    try{
        if (document.getElementById("divAlterarMtvCancelBolsa").style.display == "")
            AlterarMtvCancelBolsa();
        else
            IncluirMtvCancelBolsa();
    } catch (e) {
        postGerarLog(e);
    }
}

function EnviarDadosMtvTransf() {
    try {
        if (document.getElementById("divAlterarMtvTransf").style.display == "")
            AlterarMtvTransf();
        else
            IncluirMtvTransf();
    } catch (e) {
        postGerarLog(e);
    }
}

function montaBotoesSec() {
    require([
          "dojo/_base/xhr",
          "dojo/data/ObjectStore",
          "dijit/form/Button"
    ], function (xhr, ObjectStore, Button) {
        try{
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarEsc(true); }
            }, "pesquisarEsc");

            decreaseBtn(document.getElementById("pesquisarEsc"), '32px');
            //btnPesquisar(document.getElementById("pesquisarEsc"));

            // Escolaridade
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
                        dijit.byId("formularioEsc").show();
                        getLimpar('#formEscolaridade');
                        clearForm("formEscolaridade");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarEsc', 'divIncluirEsc', 'divExcluirEsc', 'apresentadorMensagemEsc', 'divCancelarEsc', 'divClearEsc');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoEsc");

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                        xhr.get({
                            url: !hasValue(document.getElementById("pesquisaEsc").value) ? Endereco() + "/api/secretaria/geturlrelatorioesc?" + getStrGridParameters('gridEsc') + "descricao=&inicio=" + document.getElementById("inicioDescEsc").checked + "&status=" + retornaStatus("statusEsc") : Endereco() + "/api/secretaria/geturlrelatorioesc?" + getStrGridParameters('gridEsc') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaEsc").value) + "&inicio=" + document.getElementById("inicioDescEsc").checked + "&status=" + retornaStatus("statusEsc"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
                    function (error) {
                        try{
                            apresentaMensagem('apresentadorMensagem', error);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    });
                }
            }, "relatorioEsc");

            //Mídia
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
                        dijit.byId("formularioMid").show();
                        getLimpar('#formMidia');
                        clearForm("formMidia");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarMid', 'divIncluirMid', 'divExcluirMid', 'apresentadorMensagemMid', 'divCancelarMid', 'divClearMid');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoMidia");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarMid(true); }
            }, "pesquisarMidia");
            decreaseBtn(document.getElementById("pesquisarMidia"), '32px');

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaMidia").value) ? Endereco() + "/api/secretaria/geturlrelatorioMid?" + getStrGridParameters('gridMid') + "descricao=&inicio=" + document.getElementById("inicioDescMid").checked + "&status=" + retornaStatus("statusMid") : Endereco() + "/api/secretaria/geturlrelatorioMid?" + getStrGridParameters('gridMid') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaMidia").value) + "&inicio=" + document.getElementById("inicioDescMid").checked + "&status=" + retornaStatus("statusMid"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
                }
            }, "relatorioMid");
            //Tipo Contato
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarTpContato(true); }
            }, "pesquisarTipoContato");
            decreaseBtn(document.getElementById("pesquisarTipoContato"), '32px');

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
                        dijit.byId("formularioTpContato").show();
                        getLimpar('#formTpContato');
                        clearForm("formTpContato");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarTpContato', 'divIncluirTpContato', 'divExcluirTpContato', 'apresentadorMensagemTpContato', 'divCancelarTpContato', 'divClearTpContato');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoTipoContato");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaTipoContato").value) ? Endereco() + "/api/secretaria/geturlrelatorioTipoContato?" + getStrGridParameters('gridTpContato') + "descricao=&inicio=" + document.getElementById("inicioDescTpContato").checked + "&status=" + retornaStatus("statusTpContato") : Endereco() + "/api/secretaria/geturlrelatorioTipoContato?" + getStrGridParameters('gridTpContato') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaTpContato").value) + "&inicio=" + document.getElementById("inicioDescTpContato").checked + "&status=" + retornaStatus("statusTpContato"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
                }
            }, "relatorioTipoContato");
            // Motivo da Matrícula
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarMtvMat(true); }
            }, "pesquisarMtvMat");
            decreaseBtn(document.getElementById("pesquisarMtvMat"), '32px');
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
                        dijit.byId("formularioMtvMat").show();
                        getLimpar('#formMtvMat');
                        clearForm("formMtvMat");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarMtvMat', 'divIncluirMtvMat', 'divExcluirMtvMat', 'apresentadorMensagemMtvMat', 'divCancelarMtvMat', 'divClearMtvMat');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoMtvMat");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaMtvMat").value) ? Endereco() + "/api/secretaria/geturlrelatorioMtvMat?" + getStrGridParameters('gridMtvMat') + "descricao=&inicio=" + document.getElementById("inicioDescMtvMat").checked + "&status=" + retornaStatus("statusMtvMat") : Endereco() + "/api/secretaria/geturlrelatorioMtvMat?" + getStrGridParameters('gridMtvMat') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvMat").value) + "&inicio=" + document.getElementById("inicioDescMtvMat").checked + "&status=" + retornaStatus("statusMtvMat"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioMtvMat");
            // Motivo da Não Matrícula
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarMtvNMat(true); }
            }, "pesquisarMtvNMat");
            decreaseBtn(document.getElementById("pesquisarMtvNMat"), '32px');

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
                        dijit.byId("formularioMtvNMat").show();
                        getLimpar('#formMtvNMat');
                        clearForm("formMtvNMat");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarMtvNMat', 'divIncluirMtvNMat', 'divExcluirMtvNMat', 'apresentadorMensagemMtvNMat', 'divCancelarMtvNMat', 'divClearMtvNMat');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoMtvNMat");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                        xhr.get({
                            url: !hasValue(document.getElementById("pesquisaMtvNMat").value) ? Endereco() + "/api/secretaria/geturlrelatorioMtvNMat?" + getStrGridParameters('gridMtvNMat') + "descricao=&inicio=" + document.getElementById("inicioDescMtvNMat").checked + "&status=" + retornaStatus("statusMtvNMat") : Endereco() + "/api/secretaria/geturlrelatorioMtvNMat?" + getStrGridParameters('gridMtvNMat') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvNMat").value) + "&inicio=" + document.getElementById("inicioDescMtvNMat").checked + "&status=" + retornaStatus("statusMtvNMat"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            try{
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                    })
                }
            }, "relatorioMtvNMat");
            // Motivo da Bolsa
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarMtvBolsa(true); }
            }, "pesquisarMtvBolsa");
            decreaseBtn(document.getElementById("pesquisarMtvBolsa"), '32px');

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
                        dijit.byId("formularioMtvBolsa").show();
                        getLimpar('#formMtvBolsa');
                        clearForm("formMtvBolsa");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarMtvBolsa', 'divIncluirMtvBolsa', 'divExcluirMtvBolsa', 'apresentadorMensagemMtvBolsa', 'divCancelarMtvBolsa', 'divClearMtvBolsa');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoMtvBolsa");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaMtvBolsa").value) ? Endereco() + "/api/secretaria/geturlrelatorioMtvBolsa?" + getStrGridParameters('gridMtvBolsa') + "descricao=&inicio=" + document.getElementById("inicioDescMtvBolsa").checked + "&status=" + retornaStatus("statusMtvBolsa") : Endereco() + "/api/secretaria/geturlrelatorioMtvBolsa?" + getStrGridParameters('gridMtvBolsa') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvBolsa").value) + "&inicio=" + document.getElementById("inicioDescMtvBolsa").checked + "&status=" + retornaStatus("statusMtvBolsa"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        }catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioMtvBolsa");
            // Motivo do cancelamento da Bolsa
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarMtvCancelBolsa(true); }
            }, "pesquisarMtvCancelBolsa");
            decreaseBtn(document.getElementById("pesquisarMtvCancelBolsa"), '32px');

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
                        dijit.byId("formularioMtvCancelBolsa").show();
                        getLimpar('#formMtvCancelBolsa');
                        clearForm("formMtvCancelBolsa");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarMtvCancelBolsa', 'divIncluirMtvCancelBolsa', 'divExcluirMtvCancelBolsa', 'apresentadorMensagemMtvCancelBolsa', 'divCancelarMtvCancelBolsa', 'divClearMtvCancelBolsa');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoMtvCancelBolsa");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaMtvCancelBolsa").value) ? Endereco() + "/api/secretaria/geturlrelatorioMtvCancelBolsa?" + getStrGridParameters('gridMtvCancelBolsa') + "descricao=&inicio=" + document.getElementById("inicioDescMtvCancelBolsa").checked + "&status=" + retornaStatus("statusMtvCancelBolsa") : Endereco() + "/api/secretaria/geturlrelatorioMtvCancelBolsa?" + getStrGridParameters('gridMtvCancelBolsa') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvCancelBolsa").value) + "&inicio=" + document.getElementById("inicioDescMtvCancelBolsa").checked + "&status=" + retornaStatus("statusMtvCancelBolsa"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        }catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioMtvCancelBolsa");
            // Ação Follow up
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarAcaoFollowUp(true); }
            }, "pesquisarAcaoFollowUp");

            decreaseBtn(document.getElementById("pesquisarAcaoFollowUp"), '32px');
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
                        dijit.byId("formularioAcaoFollowup").show();
                        limparAcaoFollowup();
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarAcaoFollow', 'divIncluirAcaoFollow', 'divExcluirAcaoFollow', 'apresentadorMensageAcaoFollow', 'divCancelarAcaoFollow', 'divClearAcaoFollow');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novaAcaoFollowUp");

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: Endereco() + "/api/secretaria/geturlrelatorioAcaoFollowUp?" + getStrGridParameters('gridAcaoFollowUp') + "descricao=" + encodeURIComponent(document.getElementById("descAcaoFollowUp").value) + "&inicio=" + document.getElementById("inicioAcaoFollowUp").checked + "&status=" + retornaStatus("statusAcao"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                    },
                function (error) {
                    try {
                        apresentaMensagem('apresentadorMensagem', error);
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                }
            }, "relatorioAcaoFollowUp");

            // Ano Escolar
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    // função pesquisar ano escolar.
                    pesquisarAnoEscolar(true);
                }
            }, "pesquisarAnoEscolar");
            decreaseBtn(document.getElementById("pesquisarAnoEscolar"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try {                       
                        // Função para abrir modal novo ano escolar.
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }

                        dijit.byId("formNovoAnoEscolar").show();
                        limparFormAnoEscolar();
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarAnoEscolar', 'divIncluirAnoEscolar', 'divExcluirAnoEscolar', 'apresentadorMensageAnoEscolar', 'divCancelarAnoEscolar', 'divClearAnoEscolar');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoAnoEscolar");

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    // Buscar relatorio.
                    xhr.get({
                        url: Endereco() + "/api/secretaria/GeturlrelatorioAnoEscolar?" + getStrGridParameters('gridAnoEscolar') + "cdEscolaridade=" + $("[name='escAnoEscolar']").val() + "&descricao=" + $("#descAnoEscolar").val() + "&inicio=" + $('#inicioEscAnoEscolar').is(':checked') + "&status=" + retornaStatus("statusAnoEscolar"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                    },
                function (error) {
                    try {
                        apresentaMensagem('apresentadorMensagem', error);
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                }
            }, "relatorioAnoEscolar");

            //Motivo Transferencia
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    // função pesquisar motivo transf.
                    pesquisarMtvTransf(true);
                }
            }, "pesquisarMtvTransf");
            decreaseBtn(document.getElementById("pesquisarMtvTransf"), '32px');

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
                        dijit.byId("formularioMtvTransf").show();
                        getLimpar('#formMtvTransf');
                        clearForm("formMtvTransf");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarMtvTransf', 'divIncluirMtvTransf', 'divExcluirMtvTransf', 'apresentadorMensagemMtvTransf', 'divCancelarMtvTransf', 'divClearMtvTransf');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoMtvTransf");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                        xhr.get({
                            url: !hasValue(document.getElementById("pesquisaMtvTransf").value) ? Endereco() + "/api/secretaria/geturlrelatorioMtvTransf?" + getStrGridParameters('gridMtvTransf') + "descricao=&inicio=" + document.getElementById("inicioDescMtvTransf").checked + "&status=" + retornaStatus("statusMtvTransf") : Endereco() + "/api/secretaria/geturlrelatorioMtvTransf?" + getStrGridParameters('gridMtvTransf') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvTransf").value) + "&inicio=" + document.getElementById("inicioDescMtvTransf").checked + "&status=" + retornaStatus("statusMtvTransf"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            try {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                    })
                }
            }, "relatorioMtvTransf");

        } catch (e) {
            postGerarLog(e);
        }
    });
}

//Tipo Contato
function IncluirTpContato() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemTpContato', null);
        if (!dijit.byId("formTpContato").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/secretaria/posttipocontato", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_tipo_contato: dom.byId("dc_tipo_contato").value,
                    id_tipo_contato_ativo: domAttr.get("id_tipo_contato_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridTpContato';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioTpContato").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusTpContato").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_tipo_contato", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoTpContato', 'cd_tipo_contato', 'selecionaTodosTpContato', ['pesquisarTipoContato', 'relatorioTipoContato'], 'todosItensTpContato');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_tipo_contato");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTpContato', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function PesquisarTpContato(limparItens) {
    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                    JsonRest({
                        target: !hasValue(document.getElementById("pesquisaTipoContato").value) ? Endereco() + "/api/secretaria/gettipocontatosearch?descricao=&inicio=" + document.getElementById("inicioDescTpContato").checked + "&status=" + retornaStatus("statusTpContato") : Endereco() + "/api/secretaria/gettipocontatosearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaTipoContato").value) + "&inicio=" + document.getElementById("inicioDescTpContato").checked + "&status=" + +retornaStatus("statusTpContato"),
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_tipo_contato"
                    }
            ), Memory({ idProperty: "cd_tipo_contato" }));
            dataStore = new ObjectStore({ objectStore: myStore });

            var gridTipoContato = dijit.byId("gridTpContato");

            if (limparItens)
                gridTipoContato.itensSelecionados = [];

            gridTipoContato.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarTpContato() {
    try{
        var gridName = 'gridTpContato';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formTpContato").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Secretaria/postalterartipocontato",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_tipo_contato: dom.byId("cd_tipo_contato").value,
                    dc_tipo_contato: dom.byId("dc_tipo_contato").value,
                    id_tipo_contato_ativo: domAttr.get("id_tipo_contato_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusTpContato").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioTpContato").hide();
                    removeObjSort(grid.itensSelecionados, "cd_tipo_contato", dom.byId("cd_tipo_contato").value);
                    insertObjSort(grid.itensSelecionados, "cd_tipo_contato", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoTpContato', 'cd_tipo_contato', 'selecionaTodosTpContato', ['pesquisarTipoContato', 'relatorioTipoContato'], 'todosItensTpContato');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_tipo_contato");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTpContato', error);
            });
        });
    }catch (e) {
        postGerarLog(e);
    }
}

function DeletarTpContato(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_tipo_contato').value != 0)
                    itensSelecionados = [{
                        cd_tipo_contato: dom.byId("cd_tipo_contato").value,
                        dc_tipo_contato: dom.byId("dc_tipo_contato").value,
                        id_tipo_contato_ativo: domAttr.get("id_tipo_contato_ativo", "checked")
                    }];

            xhr.post({
                url: Endereco() + "/api/Secretaria/postdeletetipocontato",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensTpContato_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioTpContato").hide();
                    dijit.byId("pesquisaTipoContato").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridTpContato').itensSelecionados, "cd_tipo_contato", itensSelecionados[r].cd_tipo_contato);

                    PesquisarTpContato(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarTipoContato").set('disabled', false);
                    dijit.byId("relatorioTipoContato").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemTpContato', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

//Escolaridade
function IncluirEsc() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemEsc', null);
        if (!dijit.byId("formEscolaridade").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url:Endereco() + "/api/secretaria/postescolaridade",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    no_escolaridade: dom.byId("no_escolaridade").value,
                    id_escolaridade_ativa: domAttr.get("id_escolaridade_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridEsc';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioEsc").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusEsc").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_escolaridade", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionado', 'cd_escolaridade', 'selecionaTodos', ['pesquisarEsc', 'relatorioEsc'], 'todosItens', 2);
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_escolaridade");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemEsc', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//
function PesquisarEsc(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                    JsonRest({
                        target: !hasValue(document.getElementById("pesquisaEsc").value) ? Endereco() + "/api/secretaria/getescolaridadesearch?descricao=&inicio=" + document.getElementById("inicioDescEsc").checked + "&status=" + retornaStatus("statusEsc") : Endereco() + "/api/secretaria/getescolaridadesearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaEsc").value) + "&inicio=" + document.getElementById("inicioDescEsc").checked + "&status=" + retornaStatus("statusEsc"),
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridEscolaridade = dijit.byId("gridEsc");

            if (limparItens)
                gridEscolaridade.itensSelecionados = [];

            gridEscolaridade.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarEsc() {
    try{
        var gridName = 'gridEsc';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formEscolaridade").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Secretaria/postalterarescolaridade",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_escolaridade: dom.byId("cd_escolaridade").value,
                    no_escolaridade: dom.byId("no_escolaridade").value,
                    id_escolaridade_ativa: domAttr.get("id_escolaridade_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusEsc").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioEsc").hide();
                    removeObjSort(grid.itensSelecionados, "cd_escolaridade", dom.byId("cd_escolaridade").value);
                    insertObjSort(grid.itensSelecionados, "cd_escolaridade", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionado', 'cd_escolaridade', 'selecionaTodos', ['pesquisarEsc', 'relatorioEsc'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_escolaridade");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemEsc', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarEsc(itensSelecionados) {
    try{
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_escolaridade').value != 0)
                    itensSelecionados = [{
                        cd_escolaridade: dom.byId("cd_escolaridade").value,
                        no_escolaridade: dom.byId("no_escolaridade").value,
                        id_escolaridade_ativa: domAttr.get("id_escolaridade_ativa", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/Secretaria/postdeleteescolaridade",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItens_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioEsc").hide();
                    dijit.byId("pesquisaEsc").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridEsc').itensSelecionados, "cd_escolaridade", itensSelecionados[r].cd_escolaridade);

                    PesquisarEsc(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarEsc").set('disabled', false);
                    dijit.byId("relatorioEsc").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioEsc").style.display))
                    apresentaMensagem('apresentadorMensagemEsc', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

// Midia
function IncluirMid() {
    try{
        if (!dijit.byId("formMidia").validate())
            return false;
        apresentaMensagem('apresentadorMensagemMid', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/secretaria/postmidia", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    no_midia: dom.byId("no_midia").value,
                    id_midia_ativa: domAttr.get("id_midia_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(eval(data)).retorno;
                    var gridName = 'gridMid';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMid").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMid").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_midia", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMid', 'cd_midia', 'selecionaTodosMid', ['pesquisarMidia', 'relatorioMid'], 'todosItensMid');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_midia");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMid', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function PesquisarMid(limparItens) {
    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                    JsonRest({
                        target: !hasValue(document.getElementById("pesquisaMidia").value) ? Endereco() + "/api/secretaria/getmidiasearch?descricao=&inicio=" + document.getElementById("inicioDescMid").checked + "&status=" + retornaStatus("statusMid") : Endereco() + "/api/secretaria/getmidiasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaMidia").value) + "&inicio=" + document.getElementById("inicioDescMid").checked + "&status=" + retornaStatus("statusMid"),
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                  ), Memory({ }));

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridMidia = dijit.byId("gridMid");

            if (limparItens)
                gridMidia.itensSelecionados = [];

            gridMidia.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarMid() {
    try{
        var gridName = 'gridMid';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formMidia").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Secretaria/postalterarmidia",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_midia: dom.byId("cd_midia").value,
                    no_midia: dom.byId("no_midia").value,
                    id_midia_ativa: domAttr.get("id_midia_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(eval(data)).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMid").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMid").hide();
                    removeObjSort(grid.itensSelecionados, "cd_midia", dom.byId("cd_midia").value);
                    insertObjSort(grid.itensSelecionados, "cd_midia", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMid', 'cd_midia', 'selecionaTodosMid', ['pesquisarMidia', 'relatorioMid'], 'todosItensMid');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_midia");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMid', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarMid(itensSelecionados) {
    try{
        if (!dijit.byId("formMidia").validate()) {
            //        return false;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe Mídia selecionada.");
            apresentaMensagem('apresentadorMensagemMid', mensagensWeb);
        }

        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_midia').value != 0)
                    itensSelecionados = [{
                        cd_midia: dom.byId("cd_midia").value,
                        no_midia: dom.byId("no_midia").value,
                        id_midia_ativa: domAttr.get("id_midia_ativa", "checked")
                    }];

            xhr.post({
                url: Endereco() + "/api/Secretaria/postdeletemidia",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensMid_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMid").hide();
                    dijit.byId("pesquisaMidia").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length-1; r >=0; r--)
                        removeObjSort(dijit.byId('gridMid').itensSelecionados, "cd_midia", itensSelecionados[r].cd_midia);

                    PesquisarMid(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarMidia").set('disabled', false);
                    dijit.byId("relatorioMid").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioMid").style.display))
                    apresentaMensagem('apresentadorMensagemMid', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

//Motivo Matrícula
function IncluirMtvMat() {
    try{
        if (!dijit.byId("formMtvMat").validate())
            return false;
        apresentaMensagem('apresentadorMensagemMtvMat', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/secretaria/postmotivomatricula", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_motivo_matricula: dom.byId("dc_motivo_matricula").value,
                    id_motivo_matricula_ativo: domAttr.get("id_motivo_matricula_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridMtvMat';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvMat").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMtvMat").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_motivo_matricula", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMtvMat', 'cd_motivo_matriculaMtvMat', 'selecionaTodosMtvMat', ['pesquisarMtvMat', 'relatorioMtvMat'], 'todosItensMtvMat');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_motivo_matricula");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMtvMat', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function PesquisarMtvMat(limparItens) {
    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                    JsonRest({
                        target: !hasValue(document.getElementById("pesquisaMtvMat").value) ? Endereco() + "/api/secretaria/getmotivomatriculasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvMat").checked + "&status=" + retornaStatus("statusMtvMat") : Endereco() + "/api/secretaria/getmotivomatriculasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvMat").value) + "&inicio=" + document.getElementById("inicioDescMtvMat").checked + "&status=" + retornaStatus("statusMtvMat"),
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_motivo_matricula"
                    }
            ), Memory({ idProperty: "cd_motivo_matricula" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridMotivoMatricula = dijit.byId("gridMtvMat");

            if (limparItens)
                gridMotivoMatricula.itensSelecionados = [];

            gridMotivoMatricula.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
     });
}

function AlterarMtvMat() {
    try{
        var gridName = 'gridMtvMat';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formMtvMat").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Secretaria/postalterarmotivomatricula",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_motivo_matricula: dom.byId("cd_motivo_matricula").value,
                    dc_motivo_matricula: dom.byId("dc_motivo_matricula").value,
                    id_motivo_matricula_ativo: domAttr.get("id_motivo_matricula_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMtvMat").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvMat").hide();
                    removeObjSort(grid.itensSelecionados, "cd_motivo_matricula", dom.byId("cd_motivo_matricula").value);
                    insertObjSort(grid.itensSelecionados, "cd_motivo_matricula", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMtvMat', 'cd_motivo_matricula', 'selecionaTodosMtvMat', ['pesquisarMtvMat', 'relatorioMtvMat'], 'todosItensMtvMat');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_motivo_matricula");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
          function (error) {
              showCarregando();
              apresentaMensagem('apresentadorMensagemMtvMat', error);
          });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarMtvMat(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_motivo_matricula').value != 0)
                    itensSelecionados = [{
                        cd_motivo_matricula: dom.byId("cd_motivo_matricula").value,
                        dc_motivo_matricula: dom.byId("dc_motivo_matricula").value,
                        id_motivo_matricula_ativo: domAttr.get("id_motivo_matricula_ativo", "checked")
                    }];

            xhr.post({
                url: Endereco() + "/api/Secretaria/postdeletemotivomatricula",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensMtvMat_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvMat").hide();
                    dijit.byId("pesquisaMtvMat").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridMtvMat').itensSelecionados, "cd_motivo_matricula", itensSelecionados[r].cd_motivo_matricula);

                    PesquisarMtvMat(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarMtvMat").set('disabled', false);
                    dijit.byId("relatorioMtvMat").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioMtvMat").style.display))
                    apresentaMensagem('apresentadorMensagemMtvMat', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

//Motivo Não Matrícula
function IncluirMtvNMat() {
    try{
        apresentaMensagem('apresentadorMensagemMtvNMat', null);
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formMtvNMat").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/secretaria/postmotivonmatricula", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_motivo_nao_matricula: dom.byId("dc_motivo_nao_matricula").value,
                    id_motivo_nao_matricula_ativo: domAttr.get("id_motivo_nao_matricula_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridMtvNMat';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvNMat").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMtvNMat").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_motivo_nao_matricula", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMtvNMat', 'cd_motivo_nao_matricula', 'selecionaTodosMtvNMat', ['pesquisarMtvNMat', 'relatorioMtvNMat'], 'todosItensMtvNMat');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_motivo_nao_matricula");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMtvNMat', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function PesquisarMtvNMat(limparItens) {
    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaMtvNMat").value) ? Endereco() + "/api/secretaria/getmotivonmatriculasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvNMat").checked + "&status=" + retornaStatus("statusMtvNMat") : Endereco() + "/api/secretaria/getmotivonmatriculasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvNMat").value) + "&inicio=" + document.getElementById("inicioDescMtvNMat").checked + "&status=" + retornaStatus("statusMtvNMat"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_motivo_nao_matricula"
                }
            ), Memory({ idProperty: "cd_motivo_nao_matricula" }));
            var gridMotivoNMatricula = dijit.byId("gridMtvNMat");

            dataStore = new ObjectStore({ objectStore: myStore });
            if (limparItens)
                gridMotivoNMatricula.itensSelecionados = [];

            gridMotivoNMatricula.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarMtvNMat() {
    try{
        var gridName = 'gridMtvNMat';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formMtvNMat").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Secretaria/postalterarmotivonmatricula",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_motivo_nao_matricula: dom.byId("cd_motivo_nao_matricula").value,
                    dc_motivo_nao_matricula: dom.byId("dc_motivo_nao_matricula").value,
                    id_motivo_nao_matricula_ativo: domAttr.get("id_motivo_nao_matricula_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMtvNMat").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvNMat").hide();
                    removeObjSort(grid.itensSelecionados, "cd_motivo_nao_matricula", dom.byId("cd_motivo_nao_matricula").value);
                    insertObjSort(grid.itensSelecionados, "cd_motivo_nao_matricula", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMtvNMat', 'cd_motivo_nao_matricula', 'selecionaTodosMtvNMat', ['pesquisarMtvNMat', 'relatorioMtvNMat'], 'todosItensMtvNMat');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_motivo_nao_matricula");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMtvNMat', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarMtvNMat(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_motivo_nao_matricula').value != 0)
                    itensSelecionados = [{
                        cd_motivo_nao_matricula: dom.byId("cd_motivo_nao_matricula").value,
                        dc_motivo_nao_matricula: dom.byId("dc_motivo_nao_matricula").value,
                        id_motivo_nao_matricula_ativo: domAttr.get("id_motivo_nao_matricula_ativo", "checked")
                    }];

            xhr.post({
                url: Endereco() + "/api/Secretaria/postdeletemotivonmatricula",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensMtvNMat_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvNMat").hide();
                    dijit.byId("pesquisaMtvNMat").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridMtvNMat').itensSelecionados, "cd_motivo_nao_matricula", itensSelecionados[r].cd_motivo_nao_matricula);

                    PesquisarMtvNMat(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarMtvNMat").set('disabled', false);
                    dijit.byId("relatorioMtvNMat").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioMtvNMat").style.display))
                    apresentaMensagem('apresentadorMensagemMtvNMat', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}
//Motivo Bolsa
function IncluirMtvBolsa() {
    try{
        if (!dijit.byId("formMtvBolsa").validate())
            return false;
        apresentaMensagem('apresentadorMensagemMtvBolsa', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/secretaria/postmotivobolsa", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_motivo_bolsa: dom.byId("dc_motivo_bolsa").value,
                    id_motivo_bolsa_ativo: domAttr.get("id_motivo_bolsa_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridMtvBolsa';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvBolsa").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMtvBolsa").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_motivo_bolsa", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMtvBolsa', 'cd_motivo_bolsa', 'selecionaTodosMtvBolsa', ['pesquisarMtvBolsa', 'relatorioMtvBolsa'], 'todosItensMtvBolsa');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_motivo_bolsa");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMtvBolsa', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function PesquisarMtvBolsa(limparItens) {
    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaMtvBolsa").value) ? Endereco() + "/api/secretaria/getmotivobolsasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvBolsa").checked + "&status=" + retornaStatus("statusMtvBolsa") : Endereco() + "/api/secretaria/getmotivobolsasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvBolsa").value) + "&inicio=" + document.getElementById("inicioDescMtvBolsa").checked + "&status=" + retornaStatus("statusMtvBolsa"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_motivo_bolsa"
                }
            ), Memory({ idProperty: "cd_motivo_bolsa" }));
            var gridMotivoBolsa = dijit.byId("gridMtvBolsa");
            dataStore = new ObjectStore({ objectStore: myStore });

            if (limparItens)
                gridMotivoBolsa.itensSelecionados = [];

            gridMotivoBolsa.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarMtvBolsa() {
    try{
        var gridName = 'gridMtvBolsa';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formMtvBolsa").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Secretaria/postalterarmotivobolsa",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_motivo_bolsa: dom.byId("cd_motivo_bolsa").value,
                    dc_motivo_bolsa: dom.byId("dc_motivo_bolsa").value,
                    id_motivo_bolsa_ativo: domAttr.get("id_motivo_bolsa_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMtvBolsa").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvBolsa").hide();
                    removeObjSort(grid.itensSelecionados, "cd_motivo_bolsa", dom.byId("cd_motivo_bolsa").value);
                    insertObjSort(grid.itensSelecionados, "cd_motivo_bolsa", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMtvBolsa', 'cd_motivo_bolsa', 'selecionaTodosMtvBolsa', ['pesquisarMtvBolsa', 'relatorioMtvBolsa'], 'todosItensMtvBolsa');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_motivo_bolsa");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMtvBolsa', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarMtvBolsa(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_motivo_bolsa').value != 0)
                    itensSelecionados = [{
                        cd_motivo_bolsa: dom.byId("cd_motivo_bolsa").value,
                        dc_motivo_bolsa: dom.byId("dc_motivo_bolsa").value,
                        id_motivo_bolsa_ativo: domAttr.get("id_motivo_bolsa_ativo", "checked")
                    }];

            xhr.post({
                url: Endereco() + "/api/Secretaria/postdeletemotivobolsa",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensMtvBolsa_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvBolsa").hide();
                    dijit.byId("pesquisaMtvBolsa").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridMtvBolsa').itensSelecionados, "cd_motivo_bolsa", itensSelecionados[r].cd_motivo_bolsa);

                    PesquisarMtvBolsa(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarMtvBolsa").set('disabled', false);
                    dijit.byId("relatorioMtvBolsa").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioMtvBolsa").style.display))
                    apresentaMensagem('apresentadorMensagemMtvBolsa', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

//Motivo Cancelamento Bolsa
function IncluirMtvCancelBolsa() {
    try{
        apresentaMensagem('apresentadorMensagemMtvCancelBolsa', null);
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formMtvCancelBolsa").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/secretaria/postmotivocancelbolsa", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_motivo_cancelamento_bolsa: dom.byId("dc_motivo_cancelamento_bolsa").value,
                    id_motivo_cancelamento_bolsa_ativo: domAttr.get("id_motivo_cancelamento_bolsa_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridMtvCancelBolsa';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvCancelBolsa").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMtvCancelBolsa").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_motivo_cancelamento_bolsa", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMtvCancelBolsa', 'cd_motivo_cancelamento_bolsa', 'selecionaTodosMtvCancelBolsa', ['pesquisarMtvCancelBolsa', 'relatorioMtvCancelBolsa'], 'todosItensMtvCancelBolsa');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_motivo_cancelamento_bolsa");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMtvCancelBolsa', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function PesquisarMtvCancelBolsa(limparItens) {
    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
            JsonRest({
                target: !hasValue(document.getElementById("pesquisaMtvCancelBolsa").value) ? Endereco() + "/api/secretaria/getmotivocancelbolsasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvCancelBolsa").checked + "&status=" + retornaStatus("statusMtvCancelBolsa") : Endereco() + "/api/secretaria/getmotivocancelbolsasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvCancelBolsa").value) + "&inicio=" + document.getElementById("inicioDescMtvCancelBolsa").checked + "&status=" + retornaStatus("statusMtvCancelBolsa"),
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_motivo_cancelamento_bolsa"
            }
                  ), Memory({ idProperty: "cd_motivo_cancelamento_bolsa" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridMotivoCancelBolsa = dijit.byId("gridMtvCancelBolsa");

            if (limparItens)
                gridMotivoCancelBolsa.itensSelecionados = [];

            gridMotivoCancelBolsa.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarMtvCancelBolsa() {
    try{
        var gridName = 'gridMtvCancelBolsa';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formMtvCancelBolsa").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Secretaria/postalterarmotivocancelbolsa",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_motivo_cancelamento_bolsa: dom.byId("cd_motivo_cancelamento_bolsa").value,
                    dc_motivo_cancelamento_bolsa: dom.byId("dc_motivo_cancelamento_bolsa").value,
                    id_motivo_cancelamento_bolsa_ativo: domAttr.get("id_motivo_cancelamento_bolsa_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMtvCancelBolsa").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvCancelBolsa").hide();
                    removeObjSort(grid.itensSelecionados, "cd_motivo_cancelamento_bolsa", dom.byId("cd_motivo_cancelamento_bolsa").value);
                    insertObjSort(grid.itensSelecionados, "cd_motivo_cancelamento_bolsa", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMtvCancelBolsa', 'cd_motivo_cancelamento_bolsa', 'selecionaTodosMtvCancelBolsa', ['pesquisarMtvCancelBolsa', 'relatorioMtvCancelBolsa'], 'todosItensMtvCancelBolsa');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_motivo_cancelamento_bolsa");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMtvCancelBolsa', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarMtvCancelBolsa(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_motivo_cancelamento_bolsa').value != 0)
                    itensSelecionados = [{
                        cd_motivo_cancelamento_bolsa: dom.byId("cd_motivo_cancelamento_bolsa").value,
                        dc_motivo_cancelamento_bolsa: dom.byId("dc_motivo_cancelamento_bolsa").value,
                        id_motivo_cancelamento_bolsa_ativo: domAttr.get("id_motivo_cancelamento_bolsa_ativo", "checked")
                    }];

            xhr.post({
                url: Endereco() + "/api/Secretaria/postdeletemotivocancelbolsa",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensMtvCancelBolsa_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvCancelBolsa").hide();
                    dijit.byId("pesquisaMtvCancelBolsa").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridMtvCancelBolsa').itensSelecionados, "cd_motivo_cancelamento_bolsa", itensSelecionados[r].cd_motivo_cancelamento_bolsa);

                    PesquisarMtvCancelBolsa(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarMtvCancelBolsa").set('disabled', false);
                    dijit.byId("relatorioMtvCancelBolsa").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioMtvCancelBolsa").style.display))
                    apresentaMensagem('apresentadorMensagemMtvCancelBolsa', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function montarAcaoFollow() {
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
        var myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/secretaria/getAcaoFollowUpSearch?descricao=&inicio=false&status=1",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_acao_follow_up"
            }
        ), Memory({ idProperty: "cd_acao_follow_up" }));
        montarStatus("statusAcao");
        $(".Dialogo").css('display', 'none');
        var gridAcaoFollowUp = new EnhancedGrid({
            store: ObjectStore({ objectStore: myStore }),
            structure: [
                        { name: "<input id='selecionaTodosAcaoFollowup' style='display:none'/>", field: "selecionaAcaoFollowup", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAcaoFollowup },
                        { name: "Descrição", field: "dc_acao_follow_up", width: "75%" },
                        { name: "Ativa", field: "acao_ativa", width: "80px", styles: "text-align: center;" }
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
        }, "gridAcaoFollowUp");
        // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
        gridAcaoFollowUp.pagination.plugin._paginator.plugin.connect(gridAcaoFollowUp.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
            verificaMostrarTodos(evt, gridAcaoFollowUp, 'cd_acao_follow_up', 'selecionaTodosAcaoFollowup');
        });
        gridAcaoFollowUp.startup();
        gridAcaoFollowUp.canSort = function (col) { return Math.abs(col) != 1; };
        gridAcaoFollowUp.on("RowDblClick", function (evt) {
            try {
                if (!eval(MasterGeral())) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    return;
                }
                limparAcaoFollowup();
                apresentaMensagem('apresentadorMensagem', '');
                var idx = evt.rowIndex,
                item = this.getItem(idx),
                store = this.store;
                keepValues(FORM_ACAOFOLLOWUP, item, gridAcaoFollowUp, false);
                dijit.byId("formularioAcaoFollowup").show();
                IncluirAlterar(0, 'divAlterarAcaoFollow', 'divIncluirAcaoFollow', 'divExcluirAcaoFollow', 'apresentadorMensageAcaoFollow', 'divCancelarAcaoFollow', 'divClearAcaoFollow');
            } catch (e) {
                postGerarLog(e);
            }
        }, true);
        IncluirAlterar(1, 'divAlterarAcaoFollow', 'divIncluirAcaoFollow', 'divExcluirAcaoFollow', 'apresentadorMensageAcaoFollow', 'divCancelarAcaoFollow', 'divClearAcaoFollow');
        new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { IncluirAcaoFollowUp(); } }, "incluirAcaoFollow");
        new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { AlterarAcaoFollowUp(); } }, "alterarAcaoFollow");
        new Button({
            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarAcaoFollowUp() });
            }
        }, "deleteAcaoFollow");
        new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparAcaoFollowup(); } }, "limparAcaoFollow");
        new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(FORM_ACAOFOLLOWUP, null, gridAcaoFollowUp, null) } }, "cancelarAcaoFollow");
        new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formularioAcaoFollowup").hide(); } }, "fecharAcaoFollow");

        require(["dojo/aspect"], function (aspect) {
            aspect.after(gridAcaoFollowUp, "_onFetchComplete", function () {
                // Configura o check de todos:
                if (dojo.byId('selecionaTodosAcaoFollowup').type == 'text')
                    setTimeout("configuraCheckBox(false, 'cd_acao_follow_up', 'selecionaAcaoFollowup', -1, 'selecionaTodosAcaoFollowup', 'selecionaTodosAcaoFollowup', 'gridAcaoFollowUp')", gridAcaoFollowUp.rowsPerPage * 3);
            });
        });

        // Adiciona link de ações:
        var menu = new DropDownMenu({ style: "height: 25px" });
        var acaoEditar = new MenuItem({
            label: "Editar",
            onClick: function () {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    eventoEditarAcaoFollowup(gridAcaoFollowUp.itensSelecionados);
                } catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menu.addChild(acaoEditar);

        var acaoRemover = new MenuItem({
            label: "Excluir",
            onClick: function () {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    eventoRemover(gridAcaoFollowUp.itensSelecionados, 'DeletarAcaoFollowUp(itensSelecionados)');
                } catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menu.addChild(acaoRemover);

        var button = new DropDownButton({
            label: "Ações Relacionadas",
            name: "linkAcoesAcaoFollowUp",
            dropDown: menu,
            id: "linkAcoesAcaoFollowUp"
        });
        dom.byId("linkAcoesAcaoFollowUp").appendChild(button.domNode);

        // Adiciona link de selecionados:
        menu = new DropDownMenu({ style: "height: 25px" });
        var menuTodosItens = new MenuItem({
            label: "Todos Itens",
            onClick: function () { buscarTodosItens(gridAcaoFollowUp, 'todosItensAcaoFollowUp', ['pesquisarAcaoFollowUp', 'relatorioAcaoFollowUp']); PesquisarAcaoFollowUp(false); }
        });
        menu.addChild(menuTodosItens);

        var menuItensSelecionadosAcao = new MenuItem({
            label: "Itens Selecionados",
            onClick: function () {
                buscarItensSelecionados('gridAcaoFollowUp', 'selecionaAcaoFollowup', 'cd_acao_follow_up', 'selecionaTodosAcaoFollowup', ['pesquisarAcaoFollowUp', 'relatorioAcaoFollowUp'], 'todosItensAcaoFollowUp');
            }
        });
        menu.addChild(menuItensSelecionadosAcao);

        var button = new DropDownButton({
            label: "Todos Itens",
            name: "todosItensAcaoFollowUp",
            dropDown: menu,
            id: "todosItensAcaoFollowUp"
        });
        dom.byId("linkSelecionadosAcaoFollowUp").appendChild(button.domNode);
        adicionarAtalhoPesquisa(['descAcaoFollowUp', 'statusAcao'], 'pesquisarAcaoFollowUp', ready);
    });
}

function montarAnoEscolar() {
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
        var myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/secretaria/GetAnoEscolarSearch?cdEscolaridade=0&descricao=&inicio=false&status=1",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_ano_escolar"
            }
        ), Memory({ idProperty: "cd_ano_escolar" }));

        montarStatus("statusAnoEscolar");

        $(".Dialogo").css('display', 'none');
        var gridAnoEscolar = new EnhancedGrid({
            store: ObjectStore({ objectStore: myStore }),
            structure: [
                        { name: "<input id='selecionaTodosAnoEscolar' style='display:none'/>", field: "selecionaAnoEscolar", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAnoEscolar },
                        { name: "Escolaridade", field: "no_escolaridade", width: "40%" },
                        { name: "Descrição", field: "dc_ano_escolar", width: "40%" },
                        { name: "N° Ordem", field: "nm_ordem", width: "80px" },
                        { name: "Ativo", field: "ano_escolar_ativo", width: "80px", styles: "text-align: center;" }
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
        }, "gridAnoEscolar");
        // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
        gridAnoEscolar.pagination.plugin._paginator.plugin.connect(gridAnoEscolar.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
            verificaMostrarTodos(evt, gridAnoEscolar, 'cd_ano_escolar', 'selecionaTodosAnoEscolar');
        });
        gridAnoEscolar.startup();
        gridAnoEscolar.canSort = function (col) { return Math.abs(col) != 1; };
        gridAnoEscolar.on("RowDblClick", function (evt) {
            try {

                if (!eval(MasterGeral())) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    return;
                }
                limparFormAnoEscolar();
                apresentaMensagem('apresentadorMensagem', '');
                var idx = evt.rowIndex,
                item = this.getItem(idx),
                store = this.store;
                keepValues(FORM_ANOESCOLAR, item, gridAnoEscolar, false);
                dijit.byId("formNovoAnoEscolar").show();
                IncluirAlterar(0, 'divAlterarAnoEscolar', 'divIncluirAnoEscolar', 'divExcluirAnoEscolar', 'apresentadorMensageAnoEscolar', 'divCancelarAnoEscolar', 'divClearAnoEscolar');
            } catch (e) {
                postGerarLog(e);
            }
        }, true);   

        new Button({
            label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                IncluirAnoEscolar();
            }
        }, "incluirAnoEscolar");

        new Button({
            label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                AlterarAnoEscolar();
            }
        }, "alterarAnoEscolar");

        new Button({
            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                    DeletarAnoEscolar()
                });
            }
        }, "deleteAnoEscolar");

        new Button({
            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_ANOESCOLAR, null, gridAnoEscolar, null)
            }
        }, "cancelarAnoEscolar");

        new Button({
            label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                limparFormAnoEscolar();
            }
        }, "limparAnoEscolar");

        new Button({
            label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                dijit.byId("formNovoAnoEscolar").hide();
            }
        }, "fecharAnoEscolar");
        

        // Adiciona link de ações:
        var menu = new DropDownMenu({ style: "height: 25px" });
        var acaoEditar = new MenuItem({
            label: "Editar",
            onClick: function () {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    eventoEditarAnoEscolar(gridAnoEscolar.itensSelecionados);

                } catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menu.addChild(acaoEditar);

        var acaoRemover = new MenuItem({
            label: "Excluir",
            onClick: function () {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    eventoRemover(gridAnoEscolar.itensSelecionados, 'DeletarAnoEscolar(itensSelecionados)');
                } catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menu.addChild(acaoRemover);

        var button = new DropDownButton({
            label: "Ações Relacionadas",
            name: "linkAcoesAnoEscolar",
            dropDown: menu,
            id: "linkAcoesAnoEscolar"
        });
        dom.byId("linkAcoesAnoEscolar").appendChild(button.domNode);
         
        // Adiciona link de selecionados:
        menu = new DropDownMenu({ style: "height: 25px" });
        var menuTodosItens = new MenuItem({
            label: "Todos Itens",
            onClick: function () {
                buscarTodosItens(gridAnoEscolar, 'todosItensAnoEscolar', ['pesquisarAnoEscolar', 'relatorioAnoEscolar']);
                pesquisarAnoEscolar(false);
            }
        });
        menu.addChild(menuTodosItens);

        var menuItensSelecionadosAcao = new MenuItem({
            label: "Itens Selecionados",
            onClick: function () {
                buscarItensSelecionados('gridAnoEscolar', 'selecionaAnoEscolar', 'cd_ano_escolar', 'selecionaTodosAnoEscolar', ['pesquisarAnoEscolar', 'relatorioAnoEscolar'], 'todosItensAnoEscolar');
            }
        });
        menu.addChild(menuItensSelecionadosAcao);

        var button = new DropDownButton({
            label: "Todos Itens",
            name: "todosItensAnoEscolar",
            dropDown: menu,
            id: "todosItensAnoEscolar"
        });
        dom.byId("linkSelecionadosAnoEscolar").appendChild(button.domNode);
        //adicionarAtalhoPesquisa(['descAcaoFollowUp', 'statusAcao'], 'pesquisarAcaoFollowUp', ready);
    });
}

function IncluirAcaoFollowUp() {
    try {
        apresentaMensagem('apresentadorMensagemAcaoFollowUp', null);
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formAcaoFollowup").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/secretaria/postAcaoFollowUp", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_acao_follow_up: dom.byId("dc_acao_follow_up").value,
                    id_acao_ativa: domAttr.get("id_acao_ativa", "checked")
                })
            }).then(function (data) {
                try {
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridAcaoFollowUp';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioAcaoFollowup").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusAcao").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_acao_follow_up", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionaAcaoFollowup', 'cd_acao_follow_up', 'selecionaTodosAcaoFollowup', ['pesquisarAcaoFollowUp', 'relatorioAcaoFollowUp'], 'todosItensAcaoFollowUp');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_acao_follow_up");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAcaoFollowUp', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function IncluirAnoEscolar() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formAnoEscolar").validate())
            return false;
        require(
            ["dojo/dom",
            "dojo/dom-attr",
            "dojo/request/xhr",
            "dojox/json/ref"],
            function (dom, domAttr, xhr, ref) {
                showCarregando();
                var model = ref.toJson({
                    dc_ano_escolar: $("#dc_ano_escolar").val(),
                    id_ativo: $('#id_ativo').is(':checked'),
                    nm_ordem: $("#nm_ordem").val(),
                    cd_escolaridade: $("[name='cd_escolaridade_ano_escolar']").val()
                });

                xhr.post(Endereco() + "/api/secretaria/PostAnoEscolar", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: model
            }).then(function (data) {
                try {
                    var itemInserido = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridAnoEscolar';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formNovoAnoEscolar").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusAnoEscolar").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_ano_escolar", itemInserido);

                    populaEscolaridadePossuiAnoEscolar();

                    buscarItensSelecionados(gridName, 'selecionaAnoEscolar', 'cd_ano_escolar', 'selecionaTodosAnoEscolar', ['pesquisarAnoEscolar', 'relatorioAnoEscolar'], 'todosItensAnoEscolar');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemInserido, "cd_ano_escolar");
                    showCarregando();

                } catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function AlterarAnoEscolar() {
    try {
        var gridName = 'gridAnoEscolar';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formAnoEscolar").validate())
            return false;        

        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            var model = ref.toJson({
                cd_ano_escolar: $("#cd_ano_escolar").val(),
                dc_ano_escolar: $("#dc_ano_escolar").val(),
                id_ativo: $('#id_ativo').is(':checked'),
                nm_ordem: $("#nm_ordem").val(),
                cd_escolaridade: $("[name='cd_escolaridade_ano_escolar']").val()
            });
            xhr.post({
                url: Endereco() + "/api/Secretaria/PutAnoEscolar",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: model
            }).then(function (data) {
                try {
                    var itemAlterado = jQuery.parseJSON(data).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusAnoEscolar").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);

                    dijit.byId("formNovoAnoEscolar").hide();
                    removeObjSort(grid.itensSelecionados, "cd_ano_escolar", dom.byId("cd_ano_escolar").value);
                    insertObjSort(grid.itensSelecionados, "cd_ano_escolar", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionaAnoEscolar', 'cd_ano_escolar', 'selecionaTodosAnoEscolar', ['pesquisarAnoEscolar', 'relatorioAnoEscolar'], 'todosItensAnoEscolar');
                    setGridPagination(grid, itemAlterado, "cd_ano_escolar");
                    showCarregando();
                } catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAnoEscolar', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarAnoEscolar(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_ano_escolar').value != 0)
                    itensSelecionados = [{
                        cd_ano_escolar: $("#cd_ano_escolar").val(),
                        dc_ano_escolar: $("#dc_ano_escolar").val(),
                        id_ativo: $('#id_ativo').is(':checked'),
                        nm_ordem: $("#nm_ordem").val(),
                        cd_escolaridade: $("[name='cd_escolaridade_ano_escolar']").val()
                    }];

            xhr.post({
                url: Endereco() + "/api/Secretaria/DeleteAnoEscolar",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItensAnoEscolar_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formNovoAnoEscolar").hide();
                    //dojo.byId("cd_ano_escolar").set("value", '');
                    $("#cd_ano_escolar").val('');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridAnoEscolar').itensSelecionados, "cd_ano_escolar", itensSelecionados[r].cd_ano_escolar);

                    pesquisarAnoEscolar(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarAnoEscolar").set('disabled', false);
                    dijit.byId("relatorioAnoEscolar").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formNovoAnoEscolar").style.display))
                    apresentaMensagem('apresentadorMensagemAnoEscolar', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisarAnoEscolar(limparItens) {
    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {

            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/secretaria/GetAnoEscolarSearch?cdEscolaridade=" + $("[name='escAnoEscolar']").val() + "&descricao=" + $("#descAnoEscolar").val() + "&inicio=" + $('#inicioEscAnoEscolar').is(':checked') + "&status=" + retornaStatus("statusAnoEscolar"),
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_ano_escolar"
                    }
            ), Memory({ idProperty: "cd_ano_escolar" }));

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridAnoEscolar = dijit.byId("gridAnoEscolar");

            if (limparItens)
                gridAnoEscolar.itensSelecionados = [];

            gridAnoEscolar.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function PesquisarAcaoFollowUp(limparItens) {
    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/secretaria/GetAcaoFollowUpSearch?descricao=" + encodeURIComponent(document.getElementById("descAcaoFollowUp").value) + "&inicio=" + document.getElementById("inicioAcaoFollowUp").checked + "&status=" + retornaStatus("statusAcao"),
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_acao_follow_up"
            }
                  ), Memory({ idProperty: "cd_acao_follow_up" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridAcaoFollowUp = dijit.byId("gridAcaoFollowUp");

            if (limparItens)
                gridAcaoFollowUp.itensSelecionados = [];

            gridAcaoFollowUp.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarAcaoFollowUp() {
    try {
        var gridName = 'gridAcaoFollowUp';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formAcaoFollowup").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Secretaria/postAlterarAcaoFollowUp",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_acao_follow_up: dom.byId("cd_acao_follow_up").value,
                    dc_acao_follow_up: dom.byId("dc_acao_follow_up").value,
                    id_acao_ativa: domAttr.get("id_acao_ativa", "checked")
                })
            }).then(function (data) {
                try {
                    var itemAlterado = jQuery.parseJSON(data).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusAcao").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioAcaoFollowup").hide();
                    removeObjSort(grid.itensSelecionados, "cd_acao_follow_up", dom.byId("cd_acao_follow_up").value);
                    insertObjSort(grid.itensSelecionados, "cd_acao_follow_up", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionaAcaoFollowup', 'cd_acao_follow_up', 'selecionaTodosAcaoFollowup', ['pesquisarAcaoFollowUp', 'relatorioAcaoFollowUp'], 'todosItensAcaoFollowUp');
                    setGridPagination(grid, itemAlterado, "cd_acao_follow_up");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAcaoFollowUp', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarAcaoFollowUp(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_acao_follow_up').value != 0)
                    itensSelecionados = [{
                        cd_acao_follow_up: dom.byId("cd_acao_follow_up").value,
                        dc_acao_follow_up: dom.byId("dc_acao_follow_up").value,
                        id_acao_ativa: domAttr.get("id_acao_ativa", "checked")
                    }];

            xhr.post({
                url: Endereco() + "/api/Secretaria/postDeleteAcaoFollowUp",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItensAcaoFollowUp_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioAcaoFollowup").hide();
                    dijit.byId("dc_acao_follow_up").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridAcaoFollowUp').itensSelecionados, "cd_acao_follow_up", itensSelecionados[r].cd_acao_follow_up);

                    PesquisarAcaoFollowUp(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarAcaoFollowUp").set('disabled', false);
                    dijit.byId("relatorioAcaoFollowUp").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioAcaoFollowup").style.display))
                    apresentaMensagem('apresentadorMensagemAcaoFollowUp', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}
//Motivo Não Matrícula
function IncluirMtvTransf() {
    try {
        apresentaMensagem('apresentadorMensagemMtvTransf', null);
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formMtvTransf").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/secretaria/postmotivotransferencia", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_motivo_transferencia_aluno: dom.byId("dc_motivo_transferencia_aluno").value,
                    id_motivo_transferencia_ativo: domAttr.get("id_motivo_transferencia_ativo", "checked")
                })
            }).then(function (data) {
                try {
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridMtvTransf';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvTransf").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMtvTransf").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_motivo_transferencia_aluno", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMtvTransf', 'cd_motivo_transferencia_aluno', 'selecionaTodosMtvTransf', ['pesquisarMtvTransf', 'relatorioMtvTransf'], 'todosItensMtvTransf');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_motivo_transferencia_aluno");
                    hideCarregando();
                } catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                }
            },
                function (error) {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemMtvTransf', error);
                });
        });
    } catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function PesquisarMtvTransf(limparItens) {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaMtvTransf").value) ? Endereco() + "/api/secretaria/getmotivotransferenciasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvTransf").checked + "&status=" + retornaStatus("statusMtvTransf") : Endereco() + "/api/secretaria/getmotivonmatriculasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvTransf").value) + "&inicio=" + document.getElementById("inicioDescMtvTransf").checked + "&status=" + retornaStatus("statusMtvTransf"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_motivo_transferencia_aluno"
                }
                ), Memory({ idProperty: "cd_motivo_transferencia_aluno" }));
            var gridMotivoNMatricula = dijit.byId("gridMtvTransf");

            dataStore = new ObjectStore({ objectStore: myStore });
            if (limparItens)
                gridMotivoNMatricula.itensSelecionados = [];

            gridMotivoNMatricula.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarMtvTransf() {
    try {
        var gridName = 'gridMtvTransf';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formMtvTransf").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Secretaria/postalterarmotivotransferencia",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_motivo_transferencia_aluno: dom.byId("cd_motivo_transferencia_aluno").value,
                    dc_motivo_transferencia_aluno: dom.byId("dc_motivo_transferencia_aluno").value,
                    id_motivo_transferencia_ativo: domAttr.get("id_motivo_transferencia_ativo", "checked")
                })
            }).then(function (data) {
                try {
                    var itemAlterado = jQuery.parseJSON(data).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    dijit.byId("statusMtvTransf").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvTransf").hide();
                    removeObjSort(grid.itensSelecionados, "cd_motivo_transferencia_aluno", dom.byId("cd_motivo_transferencia_aluno").value);
                    insertObjSort(grid.itensSelecionados, "cd_motivo_transferencia_aluno", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMtvTransf', 'cd_motivo_transferencia_aluno', 'selecionaTodosMtvTransf', ['pesquisarMtvTransf', 'relatorioMtvTransf'], 'todosItensMtvTransf');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_motivo_transferencia_aluno");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemMtvTransf', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarMtvTransf(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_motivo_transferencia_aluno').value != 0)
                    itensSelecionados = [{
                        cd_motivo_transferencia_aluno: dom.byId("cd_motivo_transferencia_aluno").value,
                        dc_motivo_transferencia_aluno: dom.byId("dc_motivo_transferencia_aluno").value,
                        id_motivo_transferencia_ativo: domAttr.get("id_motivo_transferencia_ativo", "checked")
                    }];

            xhr.post({
                url: Endereco() + "/api/Secretaria/postdeletemotivotransferencia",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItensMtvTransf_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioMtvTransf").hide();
                    dijit.byId("pesquisaMtvTransf").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridMtvTransf').itensSelecionados, "cd_motivo_transferencia_aluno", itensSelecionados[r].cd_motivo_transferencia_aluno);

                    PesquisarMtvTransf(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarMtvTransf").set('disabled', false);
                    dijit.byId("relatorioMtvTransf").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    if (!hasValue(dojo.byId("formularioMtvTransf").style.display))
                        apresentaMensagem('apresentadorMensagemMtvTransf', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

function populaEscolaridadeNovoAnoEscolar() {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getEscolaridade?status=" + 1,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cd_escolaridade_ano_escolar', 'cd_escolaridade', 'no_escolaridade');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}

function populaEscolaridadePossuiAnoEscolar() {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getEscolaridadePossuiAnoEscolar",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(dados, 'escAnoEscolar', 'cd_escolaridade', 'no_escolaridade');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}