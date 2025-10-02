
//Verifica se o navegador  é compatí­vel com W3C:
if (typeof (HTMLFormElement) == 'undefined') {
    alert("Atenção: É necessário um navegador compatá­vel com padrão W3C.");
}

//Altera o método reset() para a nossa própria classe:
HTMLFormElement.prototype.reset = function (event) {
    // Verificamos como a função foi executada, setando a variável target:
    var target = event ? event.target : this;

    //Fazemos um laço para todos os elementos do formulário:
    var focusElement;
    var changed = false;
    for (var x = 0; x < target.elements.length; x++) {
        //Verificamos se ainda não existe elemento para foco setado, e o tipo de elemento. Se combinado, seta a variável:
        if (!focusElement && (target.elements[x].type == 'text' || target.elements[x].type == 'select' ||
            target.elements[x].type == 'password' || target.elements[x].type == 'checkbox' || target.elements[x].type == 'radio')) {
            focusElement = target.elements[x];
        }
        //Verificamos o tipo de elemento:
        switch (target.elements[x].type) {
            case 'text':
            case 'textarea':
            case 'password':
            case 'file':
                //Campos tipo text, textarea, password e file utilizam defaultValue:
                if (target.elements[x].value != target.elements[x].defaultValue) {
                    target.elements[x].value = target.elements[x].defaultValue;
                    changed = true;
                }
                break;
            case 'select':
            case 'select-one':
                //Campos tipo select são tratados de forma especial. Temos que criar outro laço em seus elementos option para utilizar defaultSelected:
                for (var y = 0; y < target.elements[x].options.length; y++) {
                    if (target.elements[x].options[y].selected != target.elements[x].options[y].defaultSelected) {
                        target.elements[x].options[y].selected = target.elements[x].options[y].defaultSelected;
                        changed = true;
                    }
                }
                break;
            case 'radio':
            case 'checkbox':
                //Campos tipo radio e checkbox utilizam defaultChecked:
                if (target.elements[x].checked != target.elements[x].defaultChecked) {
                    target.elements[x].checked = target.elements[x].defaultChecked;
                    changed = true;
                }
                break;
        }
        //Se o elemento teve o valor alterado, executa o método onchange() (se definido):
        if (changed == true && typeof (target.elements[x].onchange) == 'function') {
            target.elements[x].onchange();
            changed = false;
        }
    }
    //Antes de encerrar, caso exista um campo escolhido para foco, executa o método focus() do mesmo:
    if (focusElement) {
        focusElement.focus();
    }
}

//Captura o evento onreset de todos os formulários do documento:
window.addEventListener('reset', HTMLFormElement.prototype.reset, true);
