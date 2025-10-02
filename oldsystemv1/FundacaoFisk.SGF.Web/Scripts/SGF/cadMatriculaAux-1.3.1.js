function VerificarValores() {
    var gridTitulo = dijit.byId('gridTitulo');
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemMat', null);
    dijit.byId("tipoContratoAdto").set("required", false);

    // Verifica se existe algum título sem local de movimento:
    if (hasValue(gridTitulo)) {
        var titulos = gridTitulo.store.objectStore.data;
        for (var i = 0; i < titulos.length; i++)
            if (!hasValue(titulos[i].cd_local_movto) || titulos[i].cd_local_movto <= 0) {
                hideCarregando();
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroLocalMovtoObrigatorio);
                apresentaMensagem("apresentadorMensagemMat", mensagensWeb);
                throw new Exception();
                return false;
            }
    }

    if (!hasValue(dijit.byId("gridDataCurso").store.objectStore.data) &&
        dijit.byId("gridDataCurso").store.objectStore.data.length == 0 && !verificaMatriculaNormal()) {
        //dijit.byId('ckProfissional').checked
        hideCarregando();
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroNenhumCursoVinculado);
        apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
        throw new Exception();
        return false;
    }
    if (hasValue(dijit.byId("gridDataCurso").store.objectStore.data) &&
        dijit.byId("gridDataCurso").store.objectStore.data.length == 1 &&
        dijit.byId('ckMultipla').checked) {
        hideCarregando();
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Nos contratos múltiplos deverão ser informados dois cursos que possuem seqüência.");
        apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
        throw new Exception();
        return false;
    }

    if (dijit.byId("dtaMatricula").getValue() != null && dijit.byId("dtaInicioMat").getValue() != null && dojo.date.compare(dijit.byId("dtaMatricula").getValue(), dijit.byId("dtaInicioMat").getValue(), "date") > 0) {
        hideCarregando();
	    var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "A data de início não pode ser inferior a data da matrícula.");
	    apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
	    throw new Exception();
	    return false;
    }

    if (dijit.byId("dtaInicioMat").getValue() != null && dijit.byId("dtaFinalMat").getValue() != null && dojo.date.compare(dijit.byId("dtaInicioMat").getValue(), dijit.byId("dtaFinalMat").getValue(), "date") > 0) {
        hideCarregando();
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "A data final não pode ser inferior a data de início matrícula.");
        apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
        throw new Exception();
        return false;
    }

    if (dijit.byId("dtaMatricula").getValue() == null && dijit.byId("dtaInicioMat").getValue() != null ) {
        dijit.byId('dtaMatricula').attr("value", dijit.byId("dtaInicioMat").getValue());
    }

    if ((verificaMatriculaNormal() || dijit.byId('ckProfissional').checked || dijit.byId('ckInformatica').checked) && ValidaMesAnoInicialFinal()) {

        hideCarregando();
        throw new Exception();
        return false;
    }

    //if (!verificaMatriculaNormal() && (hasValue(dijit.byId("gridDataCurso")) &&
    //    hasValue(dijit.byId("gridDataCurso").store) &&
    //    hasValue(dijit.byId("gridDataCurso").store.objectStore) &&
    //    hasValue(dijit.byId("gridDataCurso").store.objectStore.data.length > 0)) &&
    //    (hasValue(dijit.byId("gridTurmaMat")) &&
    //    hasValue(dijit.byId("gridTurmaMat").store) &&
    //    hasValue(dijit.byId("gridTurmaMat").store.objectStore) &&
    //    hasValue(dijit.byId("gridTurmaMat").store.objectStore.data.length > 0)) &&
    //    (dijit.byId("gridDataCurso").store.objectStore.data.length !== dijit.byId("gridTurmaMat").store.objectStore.data.length)) {
    //    var mensagensWeb = new Array();
    //    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroCursosNaoVinculadosMatriculaComTurma);
    //    apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
    //    showCarregando();
    //    return false;
    //}
    var dtaInicioMat = hasValue(dojo.byId("dtaInicioMat").value) ? dojo.date.locale.parse(dojo.byId("dtaInicioMat").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
    var dtaFinalMat = hasValue(dojo.byId("dtaFinalMat").value) ? dojo.date.locale.parse(dojo.byId("dtaFinalMat").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;

    if (verificaMatriculaNormal()) {

        var valorCurso = dijit.byId("valorCurso").get('value');
        valorCurso = unmaskFixed(valorCurso, 2);
        var valorDesconto = dijit.byId("valorDesconto").get('value');
        valorDesconto = unmaskFixed(valorDesconto, 2);

        var nroParcelas = getNroParcelas("parcelas");

        var valorParcela = calcularRetornarValorParcela(valorCurso, nroParcelas, valorDesconto);

        var valorLiquido = dijit.byId("valorFaturar").get('value');
        if (!isNaN(valorLiquido) && valorLiquido != "") {
            valorLiquido = unmaskFixed(valorLiquido, 2);
        } else valorLiquido = 0;

        var divida = dijit.byId("divida").get('value');
        if (!isNaN(divida) && divida != "") {
            divida = unmaskFixed(divida, 2);
        } else divida = 0;

        var desconto = dijit.byId('desconto').get('value');
        var parcelas = hasValue(dijit.byId('parcelas')) ? dijit.byId('parcelas').get('value') : 1;

        //Verifica o percentual máximo permidito pela escola:
        if ((!dijit.byId("tipoContratoAdto").aditamento) || (!hasValue(dijit.byId("tipoAditamento").value)))
            getValoresParaDesconto(null, null, null, false);

        //Verifica a consistência do desconto máximo e desconto na primeira parcela.
        if (!consistirDescontoMaximo(valorDesconto, valorCurso)) {
            hideCarregando();
            throw new Exception();
            return false;
        }

        //Calcula valor de desconto da primeira parcela:
        var valorDescontoParcela = calcularDescontoPrimeiraParcela(valorCurso);

        if (!consistirDescontosParcela(valorParcela, valorDescontoParcela)) {
            hideCarregando();
            throw new Exception();
            return false;
        } else {
            //Calcula o valor liquido:
            var valorLiquido = calcularERetornarValorFaturar(valorCurso, nroParcelas, valorDesconto);

            dijit.byId('valorFaturar')._onChangeActive = false;
            dijit.byId('valorFaturar').set('value', unmaskFixed(valorLiquido, 2));
            dijit.byId('valorFaturar').old_value = valorFaturar;
            dijit.byId('valorFaturar')._onChangeActive = true;
        }
    } else {
        var valorCurso = dijit.byId("vl_contrato").get('value');
        valorCurso = unmaskFixed(valorCurso, 2);
        var valorDesconto = dijit.byId("valorDesconto").get('value');
        valorDesconto = unmaskFixed(valorDesconto, 2);

        if (dijit.byId('ckProfissional').checked)
            var nroParcelas = getNroParcelas("parcelas");
        else
            var nroParcelas = getNroParcelasCurso();

        var valorParcela = calcularRetornarValorParcela(valorCurso, nroParcelas, valorDesconto);

        var valorLiquido = dijit.byId("valorFaturar").get('value');
        if (!isNaN(valorLiquido) && valorLiquido != "") {
            valorLiquido = unmaskFixed(valorLiquido, 2);
        } else valorLiquido = 0;

        var divida = dijit.byId("divida").get('value');
        if (!isNaN(divida) && divida != "") {
            divida = unmaskFixed(divida, 2);
        } else divida = 0;

        //Verifica o percentual máximo permidito pela escola:
        if ((!dijit.byId("tipoContratoAdto").aditamento) || (!hasValue(dijit.byId("tipoAditamento").value)))
            if (dijit.byId('ckProfissional').checked)
                getValoresParaDesconto(null, null, null, false);
            else
                getValoresParaDescontoCurso(null, null, null, false);


        //Verifica a consistência do desconto máximo e desconto na primeira parcela.
        if (!consistirDescontoMaximo(valorDesconto, valorCurso)) {
            hideCarregando();
            throw new Exception();
            return false;
        }

        //Calcula valor de desconto da primeira parcela:
        var valorDescontoParcela = calcularDescontoPrimeiraParcela(valorCurso);

        if (!consistirDescontosParcela(valorParcela, valorDescontoParcela)) {
            hideCarregando();
            throw new Exception();
            return false;
        }
        else {
            //Calcula o valor liquido:
            var valorLiquido = calcularERetornarValorFaturar(valorCurso, nroParcelas, valorDesconto);

            dijit.byId('valorFaturar')._onChangeActive = false;
            dijit.byId('valorFaturar').set('value', unmaskFixed(valorLiquido, 2));
            dijit.byId('valorFaturar').old_value = valorFaturar;
            dijit.byId('valorFaturar')._onChangeActive = true;
        }
    }
    if (!consistirValorCurso(valorLiquido, valorCurso, divida)) {
        hideCarregando();
        throw new Exception();
    }
}

function ValidarAditamento(){
    if (!validarAditamentoCrud(dojo.window) ) {
        hideCarregando();
        var tabs = dijit.byId("tabContainerMatricula");
        var pane = dijit.byId("tagAditivo");
        tabs.selectChild(pane);
        throw new Exception();
        return false;
    }

    if (hasValue(dijit.byId("tipoContratoAdto")) &&
        hasValue(dijit.byId("tipoContratoAdto").item) &&
        hasValue(dijit.byId("tipoContratoAdto").item.id_motivo_aditamento == true)) {

        if ((dijit.byId('dtInicioAditamento').value == 'Invalid Date' ||
                dijit.byId('dtInicioAditamento').value == "" ||
                dijit.byId('dtInicioAditamento').value == null) &&
            dijit.byId('dtInicioAditamento').disabled == true) {

            var tabs = dijit.byId("tabContainerMatricula");
            var pane = dijit.byId("tagAditivo");
            tabs.selectChild(pane);

            hideCarregando();
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "A Data do Aditamento é obrigatória. Clique em Novo para habilitar o campo.");
            apresentaMensagem("apresentadorMensagemMat", mensagensWeb);
            throw new Exception();
            return false;
        }
    }

    if (hasValue(dijit.byId("tipoAditamento")) &&
        hasValue(dijit.byId("tipoAditamento").item) &&
        hasValue(dijit.byId("tipoAditamento").item.id_motivo_aditamento == true)) {

        if ((dijit.byId('tipoAditamento').value == "" ||
            dijit.byId('tipoAditamento').value == null) &&
            dijit.byId('tipoAditamento').disabled == true) {

            var tabs = dijit.byId("tabContainerMatricula");
            var pane = dijit.byId("tagAditivo");
            tabs.selectChild(pane);
            hideCarregando();

            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "O Aditamento é obrigatório. Clique em Novo para habilitar o campo.");
            apresentaMensagem("apresentadorMensagemMat", mensagensWeb);
            throw new Exception();
            return false;
        }
    }

    require(["dojo/window"],
	    function(windowUtils) {
            if (!validarMatriculaCadastro(windowUtils)) {
                hideCarregando();
			    setarTabCadMatricula(true);
			    throw new Exception();
			    return false;
		    }
	    });

}

function atualizarDocumentoDigitalizadoEdicaoMatricula(xhr, ref, pFuncao) {
    try {
        var files = dijit.byId("uploaderContratoDigitalizado")._files;
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (i = 0; i < files.length; i++) {
                data.append("file" + i, files[i]);
            }
            $.ajax({
                type: "POST",
                url: Endereco() + "/secretaria/UploadDocumentoDigitalizado",
                ansy: false,
                headers: { Authorization: Token() },
                contentType: false,
                processData: false,
                data: data,
                success: function (results) {
                    try {
                        if (hasValue(results) && hasValue(results.indexOf) && results.indexOf('<') >= 0) {
                            hideCarregando();
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgSessaoExpirada3);
                            apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
                            throw new Exception();
                            return false;
                        }
                        if (hasValue(results) && !hasValue(results.erro)) {

                            var contrato = new montarObjArquivoDigitalizado(results);
                            xhr.post({
                                url: Endereco() + "/api/escola/PostAtualizarDocumentoDigitalizado",
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                postData: ref.toJson(contrato)
                            }).then(function (data) {
                                try {
                                    var dataAlterado = jQuery.parseJSON(data);
                                    hideCarregando();
                                    if (dataAlterado != null && hasValue(dataAlterado) && !hasValue(dataAlterado.erro)) {
                                        if ((ALTEROU_VENDA_PACOTE || ALTEROU_LIBERAR_CERTIFICADO || ALTEROU_LIBERAR_CERTIFICADO_C) && hasValue(pFuncao))
                                            pFuncao.call(xhr, ref)
                                        else {
                                            var itemAlterado = dataAlterado.retorno;
                                            var gridName = 'gridMatricula';
                                            var grid = dijit.byId(gridName);
                                            dojo.byId("descApresMsg").value = !hasValue(dojo.byId("descApresMsg").value) ? 'apresentadorMensagem' : dojo.byId("descApresMsg").value;
                                            apresentaMensagem(dojo.byId("descApresMsg").value, data);
                                            dijit.byId("cadMatricula").hide();
                                            if (hasValue(grid)) {
                                                if (!hasValue(grid.itensSelecionados))
                                                    grid.itensSelecionados = [];
                                            }
                                        }
                                    } else {
                                        hideCarregando();
                                        apresentaMensagem('apresentadorMensagemMat', data);
                                        throw new Exception();
                                    }
                                } catch (e) {
                                    hideCarregando();
                                    postGerarLog(e);
                                }
                            },
                            function (error) {
                                hideCarregando();
                                apresentaMensagem('apresentadorMensagemMat', error);
                                throw new Exception();
                            });

                        } else {
                            hideCarregando();
                            apresentaMensagem('apresentadorMensagemMat', results);
                            throw new Exception();
                        }
                    }
                    catch (e) {
                        hideCarregando();
                        postGerarLog(e);
                    }
                },
                error: function (error) {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemMat', error);
                    throw new Exception();
                    return false;
                }
            });
        } else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Não foi possível carregar o arquivo para fazer o upload. Tente novamente.");
            apresentaMensagem('apresentadorMensagemNoCont', mensagensWeb);
            throw new Exception();
            return false;
        }
    } catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function atualizarPacoteCerttificado(xhr, ref) {
    try {
        var contrato = new montarObjPacoteCertificado();
        xhr.post({
            url: Endereco() + "/api/escola/PostAtualizarPacoteCertificado",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(contrato)
        }).then(function (data) {
            try {
                var dataAlterado = jQuery.parseJSON(data);
                if (dataAlterado != null && hasValue(dataAlterado) && !hasValue(dataAlterado.erro)) {
                    var itemAlterado = dataAlterado.retorno;
                    var gridName = 'gridMatricula';
                    var grid = dijit.byId(gridName);
                    dojo.byId("descApresMsg").value = !hasValue(dojo.byId("descApresMsg").value) ? 'apresentadorMensagem' : dojo.byId("descApresMsg").value;
                    apresentaMensagem(dojo.byId("descApresMsg").value, data);
                    dijit.byId("cadMatricula").hide();
                    if (hasValue(grid)) {
                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];
                    }
                } else {
                    apresentaMensagem('apresentadorMensagemMat', data);
                    throw new Exception();
                    //showCarregando();
                }
            } catch (e) {
                throw new Exception();
                //showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemMat', error);
            throw new Exception();
            //showCarregando();
        });

    } catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function retornoIncluirAlterar(gridName,itemAlterado){
    var grid = dijit.byId(gridName);
    // Atualizar GridAlunos
    if (hasValue(dijit.byId("gridAlunos")) && dijit.byId("gridAlunos").listaCompletaAlunos.length > 0) {
        for (var i = 0; i <= dijit.byId("gridAlunos").listaCompletaAlunos.length; i++)
            if (hasValue(dijit.byId("gridAlunos").listaCompletaAlunos[i]) && dijit.byId("gridAlunos").listaCompletaAlunos[i].cd_aluno == dojo.byId('cd_aluno_mat').value) {
                dijit.byId("gridAlunos").listaCompletaAlunos[i].situacaoAlunoTurma = dijit.byId("tipoMatricula").value == TIPOMATRICULA ? "Ativo" : dojo.byId("tipoMatricula").value;
                dijit.byId("gridAlunos").listaCompletaAlunos[i].cd_situacao_aluno_turma = dijit.byId("tipoMatricula").value == TIPOMATRICULA ? TIPOMATRICULA : TIPOREMATRICULA;
                if (itemAlterado.AlunoTurma.length > 0)
                    for (var j = 0; j <= itemAlterado.AlunoTurma.length; j++)
                        if (hasValue(itemAlterado.AlunoTurma[j]) && (hasValue(itemAlterado.AlunoTurma[j].cd_turma) &&
                            (itemAlterado.AlunoTurma[j].cd_turma == parseInt(dojo.byId("cd_turma").value))) &&
                            itemAlterado.AlunoTurma[j].cd_aluno == parseInt(dojo.byId('cd_aluno_mat').value)) {
                            dijit.byId("gridAlunos").listaCompletaAlunos[i].cd_aluno_turma = itemAlterado.AlunoTurma[j].cd_aluno_turma;
                            if (hasValue(dijit.byId("gridAlunos")._by_idx)) {
                                $.each(dijit.byId("gridAlunos")._by_idx, function (idx, value) {
                                    if (value.idty == parseInt(dojo.byId('cd_aluno_mat').value)) {
                                        value.item = dijit.byId("gridAlunos").listaCompletaAlunos[i];
                                        return false;
                                    }
                                });
                            }
                            break;
                        }
                break;
            }
        dijit.byId("gridAlunos").update();
    }
    // Atualizar GridAlunosPPT
    if (hasValue(dijit.byId("gridAlunosPPT"))) {
        if (dijit.byId("gridAlunosPPT")._by_idx.length > 0) {
            for (var i = 0; i < dijit.byId("gridAlunosPPT")._by_idx.length; i++)
                if (dijit.byId("gridAlunosPPT")._by_idx[i].item.cd_aluno == dojo.byId('cd_aluno_mat').value) {
                    dijit.byId("gridAlunosPPT")._by_idx[i].item.situacaoAlunoTurmaFilhaPPT = dijit.byId("tipoMatricula").value == 1 ? "Ativo" : dojo.byId("tipoMatricula").value;
                    dijit.byId("gridAlunosPPT")._by_idx[i].item.alunoTurma.cd_situacao_aluno_turma =
                            dijit.byId("tipoMatricula").value == TIPOMATRICULA ? TIPOMATRICULA : TIPOREMATRICULA;
                    break;
                }
        }
        if (hasValue(dijit.byId("gridAlunosPPT").listaCompletaAlunos) && dijit.byId("gridAlunosPPT").listaCompletaAlunos.length > 0) {
            for (var i = 0; i < dijit.byId("gridAlunosPPT").listaCompletaAlunos.length; i++)
                if (dijit.byId("gridAlunosPPT").listaCompletaAlunos[i].cd_aluno == dojo.byId('cd_aluno_mat').value) {
                    dijit.byId("gridAlunosPPT").listaCompletaAlunos[i].situacaoAlunoTurmaFilhaPPT = dijit.byId("tipoMatricula").value == TIPOMATRICULA ?
                                                                                            "Ativo" : dojo.byId("tipoMatricula").value;
                    dijit.byId("gridAlunosPPT").listaCompletaAlunos[i].alunoTurma.cd_situacao_aluno_turma =
                            dijit.byId("tipoMatricula").value == TIPOMATRICULA ? TIPOMATRICULA : TIPOREMATRICULA;
                    break;
                }
        }
        dijit.byId("gridAlunosPPT").update();
    }
    dijit.byId("cadMatricula").hide();
    if (hasValue(grid)) {
        if (!hasValue(grid.itensSelecionados))
            grid.itensSelecionados = [];

        removeObjSort(grid.itensSelecionados, "cd_contrato", dojo.byId("cd_contrato").value);
        insertObjSort(grid.itensSelecionados, "cd_contrato", itemAlterado);
        buscarItensSelecionados(gridName, 'selecionadoMatricula', 'cd_contrato', 'selecionaTodosMatricula', ['pesquisarMatricula', 'relatorioMatricula'], 'todosItens');
        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
        setGridPagination(grid, itemAlterado, "cd_contrato");
    }
}

function removedecimal(valor)
{
    if (/[.]/g.test(valor) && /[,]/g.test(valor))
        return valor.replace(".", "").replace(",", ".")
    else
        if (/[,]/g.test(valor))
            return valor.replace(",", ".")
        else
            return valor
}

function getLocalTipoDocumento(cd_tipo_financeiro) {
    try {
        dojo.xhr.get({
            url: Endereco() + //"/api/financeiro/getTituloBaixaFinanComFiltrosTrocaFinanceira?cd_titulo = " + cd_titulo,
                "/api/financeiro/getLocalMovtoGeralOuCartao?cd_tipo_financeiro=" + cd_tipo_financeiro,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                apresentaMensagem("apresentadorMensagemTitulo", null);
                if (cd_tipo_financeiro < 0) cd_tipo_financeiro = -1 * cd_tipo_financeiro;
                onChangeActive(false);
                if (cd_tipo_financeiro != CARTAO)
                    loadSelectLocalMovimento(data, "bancoTitMat", 'cd_local_movto', 'nomeLocal', 'nm_tipo_local');
                else
                    loadSelectLocalMovimento(data, "bancoTitMat", 'cd_local_movto', 'no_local_movto', 'nm_tipo_local');
                onChangeActive(true);
            } catch (e) {
                postGerarLog(e);
            }
        },
            function (error) {
                apresentaMensagem("apresentadorMensagemTitulo", error);
            });

    } catch (e) {
        postGerarLog(e);
    }
}

function aplicarTaxaBancaria(cd_titulo, cd_local_movto) {
    try {
        dojo.xhr.get({
            url: Endereco() +
                "/api/financeiro/getTituloAplicadoTaxaCartao?cd_titulo=" + cd_titulo + "&cd_local_movto=" + cd_local_movto,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                apresentaMensagem("apresentadorMensagemTitulo", null);
                onChangeActive(false);
                    dijit.byId("nm_dias_cartao")._onChangeActive = false;
                    dijit.byId("pc_taxa_cartao").set("value", data.pc_taxa_cartao);
                    dijit.byId("nm_dias_cartao").set("value", data.nm_dias_cartao);
                    dijit.byId("nm_dias_cartao")._onChangeActive = true;
                    dojo.byId('vl_taxa_cartao').value = data.vlTaxaCartao;
                    dojo.byId("dtaVencTit").value = data.dt_vcto;
                onChangeActive(true);
            } catch (e) {
                postGerarLog(e);
            }
        },
            function (error) {
                apresentaMensagem("apresentadorMensagemTitulo", error);
            });

    } catch (e) {
        postGerarLog(e);
    }
}