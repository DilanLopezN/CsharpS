//Enuns
var EnumTipoFollowUp = {
	TODOS: 0
};

var EnumTipoLidoFollowUp = {
	NAO: 2
};

var EnumTipoResolvidoFollowUp = {
	NAO: 2
};

var EnumNomeUsuarioOrigem = {
	 NOMEUSUARIO:""
}


//#region Monta relatório

function montarMetodosReportFollowUp() {
    require([
        "dojo/ready",
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
        "dijit/form/Button",
        "dojo/on",
        "dijit/form/FilteringSelect",
    ], function (ready, xhr, ObjectStore, Memory, registry, ItemFileReadStore, Button, on, FilteringSelect) {
        ready(function () {
            try {

	            loadFiltroTipoFollowUp();
	            loadFiltroLidoFollowUp();
                loadFiltroResolvidoFollowUp();


                new Button({
	                label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
	                onClick: function () {
		                try {
			                if (!hasValue(dijit.byId("gridPesquisaUsuarioFK")))
				                montarGridPesquisaUsuarioFK(function () {
					                abrirUsuarioFKFollowUp(PESQUSERORIGFOLLOWUP);
				                });
			                else
				                abrirUsuarioFKFollowUp(PESQUSERORIGFOLLOWUP);
		                } catch (e) {
			                postGerarLog(e);
		                }
	                }
                }, "FKUsuarioOrigemPesq");
                new Button({
	                label: "Limpar", iconClass: '', type: "reset", disabled: true,
	                onClick: function () {
		                try {
			                dijit.byId("usuarioOrgPesq").reset();
			                dojo.byId("cdUsuarioOrgPesq").value = 0;
			                dijit.byId("limparUsuarioOrigemPesq").set("disabled", true);
		                }
		                catch (e) {
			                postGerarLog(e);
		                }
	                }
                }, "limparUsuarioOrigemPesq");
                decreaseBtn(document.getElementById("limparUsuarioOrigemPesq"), '40px');
                decreaseBtn(document.getElementById("FKUsuarioOrigemPesq"), '18px');

                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio");

                

                dijit.byId('dtaInicialPesq').on("change", function (e) {
                    try {
                        var dtFinal = dijit.byId('dtaFinalPesq').get('value');
                        var retorno = testaPeriodo(e, dtFinal);
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId('dtaFinalPesq').on("change", function (e) {
                    try {
                        var dtInicial = dijit.byId('dtaInicialPesq').get('value');
                        var retorno = testaPeriodo(dtInicial, e);
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                

                adicionarAtalhoPesquisa(['dtaInicialPesq', 'dtaFinalPesq'], 'relatorio', ready);
                
                
                showCarregando();

            } catch (e) {
	            showCarregando();
                postGerarLog(e);
            }
        });

    });
}


function loadFiltroTipoFollowUp() {
	try {
		var tipoFollowUpFK = dijit.byId("tipoFollowUpPesq");
		var storeTipo = new dojo.store.Memory({
			data: [
				{ name: "Todos", id: "0" },
				{ name: "Interno", id: "1" },
				{ name: "Prospect/Aluno", id: "2" },
				{ name: "Administração Geral", id: "3" }
			]
		});
		tipoFollowUpFK.store = storeTipo;
		tipoFollowUpFK.set("value", EnumTipoFollowUp.TODOS);
	}
	catch (e) {
		postGerarLog(e);
	}
}

function loadFiltroResolvidoFollowUp() {
	try {
		var resolvidoPesq = dijit.byId("resolvidoPesq");
		var storeTipo = new dojo.store.Memory({
			data: [
				{ name: "Todos", id: "0" },
				{ name: "Sim", id: "1" },
				{ name: "Não", id: "2" }
			]
		});
		resolvidoPesq.store = storeTipo;
		resolvidoPesq.set("value", EnumTipoResolvidoFollowUp.NAO);
	}
	catch (e) {
		postGerarLog(e);
	}
}

function loadFiltroLidoFollowUp() {
	try {
		var lidoPesq = dijit.byId("lidoPesq");
		var storeTipo = new dojo.store.Memory({
			data: [
				{ name: "Todos", id: "0" },
				{ name: "Sim", id: "1" },
				{ name: "Não", id: "2" }
			]
		});
		lidoPesq.store = storeTipo;
		lidoPesq.set("value", EnumTipoLidoFollowUp.NAO);
	}
	catch (e) {
		postGerarLog(e);
	}
}




function retornarUsuarioFKPesqFollowUp() {
    try {
        var valido = true;
        var gridUsuarioSelec = dijit.byId("gridPesquisaUsuarioFK");
        var gridUsuarios = dijit.byId("gridUsuarioGrupo");
        if (!hasValue(gridUsuarioSelec.itensSelecionados))
            gridUsuarioSelec.itensSelecionados = [];
        if (!hasValue(gridUsuarioSelec.itensSelecionados) || gridUsuarioSelec.itensSelecionados.length <= 0 || gridUsuarioSelec.itensSelecionados.length > 1) {
            if (gridUsuarioSelec.itensSelecionados != null && gridUsuarioSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridUsuarioSelec.itensSelecionados != null && gridUsuarioSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        } else {
            var tipoOrigem = dojo.byId("idOrigemUsuarioFK").value;
            if (hasValue(tipoOrigem))
                switch (parseInt(tipoOrigem)) {
                    case PESQUSERORIGFOLLOWUP:
                        dijit.byId("usuarioOrgPesq").set("value", gridUsuarioSelec.itensSelecionados[0].no_login);
                        dojo.byId("cdUsuarioOrgPesq").value = gridUsuarioSelec.itensSelecionados[0].cd_usuario;
                        dijit.byId("limparUsuarioOrigemPesq").set("disabled", false);
                        
                        break;
                    
                    case PESQUSERORIGMASTERFOLLOWUP:
                        dijit.byId("usuarioOrgAdmPesq").set("value", gridUsuarioSelec.itensSelecionados[0].no_login);
                        dojo.byId("cdUsuarioOrgAdmPesq").value = gridUsuarioSelec.itensSelecionados[0].cd_usuario;
                        dijit.byId("limparFKUsuarioOrgAdmPesq").set("disabled", false);
                        break;
                    default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi informado/encontrado.");
                        return false;
                        break;
                }
        }
        if (!valido)
            return false;
        dijit.byId("proUsuario").hide();
    } catch (e) {
        postGerarLog(e);
    }
}


function testaPeriodo(dataInicial, dataFinal) {
    try {
        var retorno = true;
        if (dojo.date.compare(dataInicial, dataFinal) > 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            retorno = false;
        } else
            apresentaMensagem('apresentadorMensagem', "");
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion


function emitirRelatorio() {
    try {
        var dataInicial = dojo.byId("dtaInicialPesq").value;// hasValue(dojo.byId("dtInicial").value) ? dojo.date.locale.parse(dojo.byId("dtInicial").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var dataFinal = dojo.byId("dtaFinalPesq").value;// hasValue(dojo.byId("dtFinal").value) ? dojo.date.locale.parse(dojo.byId("dtFinal").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;

        if (!hasValue(dataFinal) && hasValue(dataInicial))
	        apresentaMensagem('apresentadorMensagem', "");

        if (hasValue(dataFinal) && hasValue(dataFinal) && !testaPeriodo(dijit.byId('dtaInicialPesq').get('value'), dijit.byId('dtaFinalPesq').get('value')))
            return false;

        var cdUsuarioOrgPesq = hasValue(dojo.byId("cdUsuarioOrgPesq").value) ? dojo.byId("cdUsuarioOrgPesq").value : 0;
        var tipoFollowUpPesq = hasValue(dijit.byId("tipoFollowUpPesq").value) ? dijit.byId("tipoFollowUpPesq").value : 0;
        var resolvidoPesq = hasValue(dijit.byId("resolvidoPesq").value) ? dijit.byId("resolvidoPesq").value : 0;
        var lidoPesq = hasValue(dijit.byId("lidoPesq").value) ? dijit.byId("lidoPesq").value : 0;
        var no_usuario_org = hasValue(dijit.byId("usuarioOrgPesq").value) ? dijit.byId("usuarioOrgPesq").value : "";
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/secretaria/GetUrlRelFollowUp?id_tipo_follow=" + tipoFollowUpPesq + "&cd_usuario_org=" + cdUsuarioOrgPesq +
                    "&no_usuario_org=" + no_usuario_org + "&resolvido=" + resolvidoPesq + "&lido=" + lidoPesq + "&dtaIni=" + dojo.byId("dtaInicialPesq").value +
	                "&dtaFinal=" + dojo.byId("dtaFinalPesq").value,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                abrePopUp(Endereco() + '/Relatorio/RelatorioFollowUp?' + data, '1000px', '750px', 'popRelatorio');
            },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
        })
    } catch (e) {
        postGerarLog(e);
    }

}


