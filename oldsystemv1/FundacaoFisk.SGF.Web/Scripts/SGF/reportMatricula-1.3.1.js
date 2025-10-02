//#region Monta relatório
var RELMATRICULA = 16, AGUARDANDO_MATRICULA = 9, ATIVO = 1, REMATRICULA = 8, MATRICULAATENDENTE = 2, MATRICULAPORMOTIVO = 1;
var RELMAT = 9, SITUACAO = 3;

function montarTipoVinculado(ready, Memory, filteringSelect) {
    try {
        var statusTipo = new Memory({
            data: [
                { name: "Todos", id: 0 },
                { name: "Sim", id: 1 },
                { name: "Não", id: 2 },
                { name: "Não c/NF", id: 3 }
            ]
        });

        var _tipo = new filteringSelect({
            id: "_tipoVinculado",
            name: "_tipoVinculado",
            store: statusTipo,
            searchAttr: "name",
            value: 0,
            style: "width: 90px;"
        }, "_tipoVinculado");
    } catch (e) {
        postGerarLog(e);
    }
}

function montarMetodosReportMatricula() {
    require([
    "dojo/ready",
    "dojo/store/Memory",
    "dijit/form/Button",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/on",
    "dijit/registry",
    "dojo/data/ItemFileReadStore",
    "dojo/store/Memory",
    "dijit/form/FilteringSelect",
    ], function (ready, Memory, Button, JsonRest, ObjectStore, Cache, on, registry, ItemFileReadStore, Memory, FilteringSelect) {
        ready(function () {
            try {
                findIsLoadComponetesPesquisaReportMatricula();
                setMenssageMultiSelect(SITUACAO, 'situacaoAlunoTurma', true, 0);
                montarTipoVinculado(ready, Memory, FilteringSelect);
                sugereDataCorrente();
                //montarStatus("statusAluno");
                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                montarGridPesquisaAluno(false, function () {
                                    abrirAlunoFK();
                                });
                            }
                            else
                                abrirAlunoFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesAluno");
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
                }, "pesTurma");
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("cdAlunoPes").value = 0;
                        dojo.byId("noAluno").value = "";
                        apresentaMensagem('apresentadorMensagem', null);
                        dijit.byId("limparAluno").set('disabled', true);
                        apresentaMensagem('apresentadorMensagem', null);
                    }
                }, "limparAluno");
                if (hasValue(document.getElementById("limparAluno"))) {
                    document.getElementById("limparAluno").parentNode.style.minWidth = '40px';
                    document.getElementById("limparAluno").parentNode.style.width = '40px';
                }
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("noTurma").value = "";
                        dojo.byId("cdTurmaPes").value = 0;
                        dijit.byId("limparTurma").set('disabled', true);
                        apresentaMensagem('apresentadorMensagem', null);
                    }
                }, "limparTurma");
                if (hasValue(document.getElementById("limparTurma"))) {
                    document.getElementById("limparTurma").parentNode.style.minWidth = '40px';
                    document.getElementById("limparTurma").parentNode.style.width = '40px';
                }

                dijit.byId("situacaoAlunoTurma").on("change", function (e) {
                    try {
                        var existeAtivoRematricula = false;
                        sugereDataCorrente();
                        var situacoes = dijit.byId('situacaoAlunoTurma').value;
                        if (situacoes.length == 2 && ((situacoes[0] == ATIVO || situacoes[0] == REMATRICULA) && (situacoes[1] == ATIVO || situacoes[1] == REMATRICULA)) ||
                            situacoes.length == 1)
                            apresentaMensagem('apresentadorMensagem', null);
                        else {
                            if (situacoes.length > 0) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroMultisselecao);
                                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                dijit.byId('situacaoAlunoTurma').setStore(dijit.byId('situacaoAlunoTurma').store, []);
                            }
                        }
                        setMenssageMultiSelectOpcao(SITUACAO, 'situacaoAlunoTurma', true, 0);
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("pesRelatorio").on("change", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                dijit.byId("tipo").on("change", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                dijit.byId("ckSemTurma").on("click", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                dijit.byId("ckTransferir").on("click", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                dijit.byId("ckRetorno").on("click", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                dijit.byId("dtInicial").on("click", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                dijit.byId("dtFinal").on("click", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaUsuarioFK")))
                                montarGridPesquisaUsuarioFK(function () {
                                    dojo.query("#nomPessoaFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisarUsuarioFKGeral();
                                    });
                                    abrirUsuarioFK(false);
                                });
                            else
                                abrirUsuarioFK(false);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cadPessoaAtendente");
                var buttonFkArray = ['cadPessoaAtendente', 'pesAluno', 'pesTurma'];
                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }
                new Button({
                    label: "Limpar", iconClass: '', disabled: true, type: "reset", onClick: function () {
                        try {
                            dijit.byId('cd_pessoa_atendente').reset();
                            dijit.byId("cd_pessoa_atendente").value = 0;
                            dojo.byId("cd_pessoa_atendente").value = "";
                            dijit.byId('limparPessoaAtendente').set("disabled", true);
                            apresentaMensagem('apresentadorMensagem', null);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparPessoaAtendente");

                if (hasValue(document.getElementById("limparPessoaAtendente"))) {
                    document.getElementById("limparPessoaAtendente").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPessoaAtendente").parentNode.style.width = '40px';
                }
                dijit.byId("pesRelatorio").on("change", function (e) {
                    dijit.byId('cd_pessoa_atendente').reset();
                    dijit.byId("cd_pessoa_atendente").value = 0;
                    dojo.byId("cd_pessoa_atendente").value = "";
                    dijit.byId('limparPessoaAtendente').set("disabled", true);
                    if (e == MATRICULAATENDENTE) {
                        apresentaMensagem('apresentadorMensagem', "");
                        dojo.byId("lbAtendente").style.display = "";
                        dojo.byId("tdAtendente").style.display = "";
                        dijit.byId("cd_pessoa_atendente").set("required", true);
                        dojo.byId("lbProdutoFiltro").style.display = "none";
                        dojo.byId("tdProdutoFiltro").style.display = "none";
                    }
                    else if (e == MATRICULAPORMOTIVO) {
                        apresentaMensagem('apresentadorMensagem', "");
                        dojo.byId("lbAtendente").style.display = "none";
                        dojo.byId("tdAtendente").style.display = "none";
                        dijit.byId("cd_pessoa_atendente").set("required", false);
                        dojo.byId("lbProdutoFiltro").style.display = "none";
                        dojo.byId("tdProdutoFiltro").style.display = "none";
                    }
                    else {
                        apresentaMensagem('apresentadorMensagem', "");
                        dojo.byId("lbAtendente").style.display = "none";
                        dojo.byId("tdAtendente").style.display = "none";
                        dijit.byId("cd_pessoa_atendente").set("required", false);
                        dojo.byId("lbProdutoFiltro").style.display = "";
                        dojo.byId("tdProdutoFiltro").style.display = "";
                    }
                })

                montarSituacaoAlunoTurma("situacaoAlunoTurma", Memory, on, registry, ItemFileReadStore);
                montarMovimentos("pesRelatorio", Memory, on);
                montarTipoItens("tipo", Memory, on);
                adicionarAtalhoPesquisa(['pesRelatorio', 'tipo', 'situacaoAlunoTurma', 'dtInicial', 'dtFinal'], 'relatorio', ready);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323078', '765px', '771px');
                        });
                }
            } catch (e) {
                postGerarLog(e);
            }
        });
        
    });
}

function findIsLoadComponetesPesquisaReportMatricula() {
    dojo.xhr.get({
        url: Endereco() + "/api/Coordenacao/componentesPesquisaReportMatricula",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        showCarregando();
        try {
            data = jQuery.parseJSON(data);
            apresentaMensagem("apresentadorMensagem", null);
            if (data.retorno != null && data.retorno != null) {
                if (hasValue(data.retorno))
                    criarOuCarregarCompFiltering("pesProdutoFiltro", data.retorno, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_produto', 'no_produto', 2);
            }
            showCarregando();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem("apresentadorMensagem", error);
    });
}

function testaPeriodo(dataInicial, dataFinal) {
    try{
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
        apresentaMensagem('apresentadorMensagem', null);
        var dataInicial = dojo.byId("dtInicial").value;
        var dataFinal = dojo.byId("dtFinal").value;
    
        if (!hasValue(dataFinal) && hasValue(dataInicial))
            dijit.byId('dtFinal').set('value', dataInicial);

        if (!testaPeriodo(dataInicial, dataFinal))
            return false;
        var cdResp = 0;

        var cdAtendente = 0;
        if (dijit.byId("pesRelatorio").value == MATRICULAATENDENTE) {
            if (dijit.byId("cd_pessoa_atendente").validate())
                cdAtendente = dijit.byId("cd_pessoa_atendente").value;
            else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEmitirRelMatAtSemAtendente);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }
        }
        var situacoes = "";
        if (!hasValue(dijit.byId('situacaoAlunoTurma').value) || dijit.byId('situacaoAlunoTurma').value.length <= 0)
            situacoes = "100";
        else
            for (var i = 0; i < dijit.byId('situacaoAlunoTurma').value.length; i++) {
                if (situacoes == "")
                    situacoes = dijit.byId('situacaoAlunoTurma').value[i];
                else
                    situacoes = situacoes + "|" + dijit.byId('situacaoAlunoTurma').value[i];
            }

       
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getUrlRelatorioMat?cd_turma=" + dojo.byId("cdTurmaPes").value + "&cd_aluno=" + dojo.byId("cdAlunoPes").value + "&tipo=" + dijit.byId("pesRelatorio").value +
                                           "&situacaoAlunoTurma=" + situacoes + "&semTurma=" + dijit.byId("ckSemTurma").checked +
                                            "&tranferencia=" + dijit.byId("ckTransferir").checked + "&retorno=" + dijit.byId("ckRetorno").checked +
                                            "&situacaoContrato=" + dijit.byId("tipo").value + "&dtIni=" + dataInicial + "&dtFim=" + dataFinal + "&cdAtendente=" + cdAtendente 
                + "&bolsaCem=" + dijit.byId("ckOcultaBolsista100").checked + "&ckContratoDigitalizado=" + dijit.byId("ckContratoDigitalizado").checked + "&cd_produto=" + dijit.byId("pesProdutoFiltro").value + "&no_produto=" + dojo.byId("pesProdutoFiltro").value + "&exibirEnderecos=" + dijit.byId("ckExibirEnderecos").checked
                                           + "&vinculado=" + dijit.byId("_tipoVinculado").value,


            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                abrePopUp(Endereco() + '/Relatorio/RelatorioMatriculaAnalitico?' + data, '1024px', '750px', 'popRelatorio');
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    } catch (e) {
        postGerarLog(e);
    }

}
function montarSituacaoAlunoTurma(nomElement, Memory, on, registry, ItemFileReadStore) {
    try{
        var w = registry.byId('situacaoAlunoTurma');
        var dados = [
                     { name: "Matriculado", id: "1" },
                     { name: "Rematriculado", id: "8" },
                     { name: "Desistente", id: "2" },
                     { name: "Encerrado", id: "4" },
                     { name: "Movido", id: "0" }
        ]
        var store = new ItemFileReadStore({
            data: {
                identifier: "id",
                label: "name",
                items: dados
            }
        });
        w.setStore(store, []);
    } catch (e) {
        postGerarLog(e);
    }
};

function montarTipoItens(nomElement, Memory, on) {
    try{
        var dados = [{ name: "Todos", id: "0" },
                     { name: "Matrícula", id: "1" },
                     { name: "Rematrícula", id: "2" }
        ]
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId(nomElement).store = statusStore;
        dijit.byId(nomElement).set("value", 0);
    } catch (e) {
        postGerarLog(e);
    }
};

function montarMovimentos(nomElement, Memory, on) {
    try{
        var dados = [
            { name: "Matrícula Analítico", id: "0" },
            { name: "Matrícula Por Motivo", id: "1" },
            { name: "Matrícula X Atendente", id: "2" }
        ];
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId(nomElement).store = statusStore;
        dijit.byId(nomElement).set("value", 0);
    } catch (e) {
        postGerarLog(e);
    }
};

function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = RELMATRICULA;
        dijit.byId("proAluno").show();
        limparPesquisaAlunoFK();
        pesquisaAlunoFKRel();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaAlunoFKRel() {
    try{
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        var turma = hasValue(dojo.byId("noTurma").value) && dojo.byId("noTurma").value > 0 ? dojo.byId("noTurma").value : 0;

        var myStore = dojo.store.Cache(
            dojo.store.JsonRest({
                target: Endereco() + "/api/aluno/getAlunoPorTurmaSearch?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
                        document.getElementById("inicioAlunoFK").checked + "&origemFK=0" + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
                        "&cdSituacao=0&sexo=" + sexo + "&cdTurma=" + turma + "&opcao=0",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_aluno"
            }), dojo.store.Memory({ idProperty: "cd_aluno" }));

        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var gridAluno = dijit.byId("gridPesquisaAluno");
        gridAluno.itensSelecionados = [];
        gridAluno.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoRelFK() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        var valido = true;
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);

            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("cdAlunoPes").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAluno').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTurmaFK() {
    try {
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = RELMATRICULA;
        dijit.byId("proTurmaFK").show();
        limparFiltrosTurmaFK();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            if (hasValue(dijit.byId("pesProfessorFK").value)) {
                dijit.byId('pesProfessorFK').set('disabled', true);
            } else {
                dijit.byId('pesProfessorFK').set('disabled', false);
            }
        }
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        limparFiltrosTurmaFK();
        dojo.byId("trAluno").style.display = "";
        dojo.byId("idOrigemCadastro").value = RELMATRICULA;
        dijit.byId("proTurmaFK").show();
        pesquisarTurmaFK();

    } catch (e) {
        postGerarLog(e);
    }
}
function retornarTurmaFKRelMatricula() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        var valido = true;
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("noTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
            dojo.byId("cdTurmaPes").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
            dijit.byId('limparTurma').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function sugereDataCorrente() {
    dojo.xhr.post({
        url: Endereco() + "/util/PostDataHoraCorrente",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var dataCorrente = jQuery.parseJSON(data).retorno;
            var dataSugerida = dataCorrente.dataPortugues.split(" ");
            if (dataSugerida.length > 0) {
                var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicial').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", date);
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirUsuarioFK(isPesquisa) {
    try {
        limparPesquisaUsuarioFK();
        dojo.byId("idOrigemUsuarioFK").value = RELMAT;
        pesquisarUsuarioFKGeral();
        dijit.byId("proUsuario").show();
        apresentaMensagem('apresentadorMensagemProUsuario', null);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarUsuarioFKRelMat() {
    try {
        var valido = true;
        var gridUsuarioSelec = dijit.byId("gridPesquisaUsuarioFK");
        if (!hasValue(gridUsuarioSelec.itensSelecionados))
            gridUsuarioSelec.itensSelecionados = [];
        if (!hasValue(gridUsuarioSelec.itensSelecionados) || gridUsuarioSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else {
            //TODO Melhorar esse código  HAS_RESPONSAVEL_TIT?? karol  não é muito usual setar na mesma variavél, seria bom criar uma para cada responsabilidade.
            dijit.byId("cd_pessoa_atendente").value = gridUsuarioSelec.itensSelecionados[0].cd_usuario;
            dojo.byId("cd_pessoa_atendente").value = gridUsuarioSelec.itensSelecionados[0].no_login;
            dijit.byId('limparPessoaAtendente').set("disabled", false);
        }

        if (!valido)
            return false;
        dijit.byId("proUsuario").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarUsuarioFKGeral() {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/domReady!",
        "dojo/parser"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            myStore = Cache(
            JsonRest({
                target: Endereco() + "/Escola/getUsuarioSearchAtendenteFK?descricao=" + encodeURIComponent(document.getElementById("pesquisatextFK").value) + "&nome=" + encodeURIComponent(document.getElementById("nomPessoaFK").value) + "&inicio=" + document.getElementById("inicioUsuarioFK").checked,
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridUsuario = dijit.byId("gridPesquisaUsuarioFK");
            gridUsuario.setStore(dataStore);
            gridUsuario.itensSelecionados = [];

            gridUsuario.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}