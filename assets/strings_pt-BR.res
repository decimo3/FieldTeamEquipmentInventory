# Application general strings
APPLICATION_TITLE=Inventário de Equipamentos de Equipes de Campo
APPLICATION_ARGUMENT_MISS=Argumento nulo ou vazio!
# Resources class strings
RESOURCE_NOT_FOUND=Não foi possível obter o texto pela chave {0}!
# Configuration class strings
CONFIGURATION_NOT_FOUND=Não foi possível obter o valor pela chave {0}!
CONFIGURATION_TYPE_ERROR=A configuração não pode ser convertida para {0}!
CONFIGURATION_PARSE_ERROR=Formato de par inválido em '{0}'!
# Fingerprint class strings
FINGERPRINT_DEV_NOT_FOUND=Não foi encontrado nenhum dispositivo!
FINGERPRINT_DEV_FAIL_OPEN=Falha ao comunicar com o dispositivo {0}!
FINGERPRINT_CAPTURE_WAIT=Aguardando registro da biometria...
FINGERPRINT_CAPTURE_OK=Captura realizada.
FINGERPRINT_COMPARISON_FAIL=Falha ao comparar os registros!
FINGERPRINT_CAPTURE_SAMPLE=Captura adicionada! Captura {0} de {1}.
FINGERPRINT_CAPTURE_FAIL=Falha na captura da biometria! {0}
FINGERPRINT_QUALITY_FAIL=A qualidade da captura não é satisfatória! {0}
FINGERPRINT_EXTRACTION_FAIL=Falha na extração das informações da captura! {0}
# Screens strings
GLOBAL_SCREEN_BACK_BTN=Voltar
GLOBAL_SCREEN_SAVE_BTN=Salvar
GLOBAL_SCREEN_EXPORT_BTN=Exportar
GLOBAL_SCREEN_EXPORTED_TXT=Exportado para {0}!

HELP_SCREEN_HEAD_TXT=Ajuda
HELP_SCREEN_HELP_TXT=Esse é um programa para controle de equipamentos de equipes de campo, onde é possível cadastrar os funcionários, equipamentos, registrar as transações e gerar relatórios.\n\nUm kit pode conter 1 a 5 itens, com apenas com um equipamento de cada tipo.\n\nAs transações principais são CHECK-IN e CHECKOUT\n\nO equipamento só pode ser transacionado para CHECK-IN se ele estiver na base (IDLE, CHECKOUT ou FROMREPAIR), e só pode ser transacionado para CHECKOUT se estiver em campo (CHECK-IN).\n\nA transação IDLE é só usada para cadastrar o dispositivo, TOREPAIR para indicar um equipamento retirado para manutenção, e só pode ser transacionado para FROMREPAIR, indicando o retorno da manutenção.\n\nA transação REMOVED é usada para desabilitar permanentemente o equipamento, indicando que o equipamento foi devolvido para a Light.
HELP_SCREEN_BACK_BTN=Voltar

AUTH_SCREEN_TITLE_LBL=Autenticar usuário
AUTH_SCREEN_FINGER_BTN=Capturar biometria
AUTH_SCREEN_FINGER_WAIT_LBL=Aguardando registro da biometria...
AUTH_SCREEN_FINGER_SUCCESS_LBL=Reconhecimento realizado com sucesso!
AUTH_SCREEN_FINGER_FAILURE_LBL=Não foi possível reconhecer a biometria!
AUTH_SCREEN_FINGER_CANCELED_LBL=Captura de biometria cancelada!

MAIN_SCREEN_HEAD_TXT=Inventário de Equipamentos de Equipes de Campo
MAIN_SCREEN_LOGOUT_BTN=Sair
MAIN_SCREEN_HELP_BTN=Exibir informações de ajuda
MAIN_SCREEN_ENROLL_BTN=Cadastrar funcionário
MAIN_SCREEN_EQUIP_BTN=Cadastrar equipamento
MAIN_SCREEN_ENTRY_BTN=Cadastrar transação
MAIN_SCREEN_REPORT_BTN=Relatório transações


ENROLL_SCREEN_TITLE_LBL=Cadastrar funcionário
ENROLL_SCREEN_FULLNAME_PLACEHOLDER=Digite o nome completo
ENROLL_SCREEN_REGISTRY_PLACEHOLDER=Digite a matrícula
ENROLL_SCREEN_FINGER_BTN=Capturar biometria
ENROLL_SCREEN_FINGER_WAIT_LBL=Aguardando cadastro da biometria...
ENROLL_SCREEN_FINGER_SUCCESS_LBL=Captura realizada com sucesso!
ENROLL_SCREEN_SAVE_BTN=Salvar
ENROLL_SCREEN_BACK_BTN=Voltar

EQUIP_SCREEN_TITLE_LBL=Cadastrar equipamento
EQUIP_SCREEN_EQUIP_PLACEHOLDER=Digite o ID do equipamento
EQUIP_SCREEN_KIND_PLACEHOLDER=Selecione o tipo do equipamento
EQUIP_SCREEN_STATUS_PLACEHOLDER=Selecione o estado do equipamento
EQUIP_SCREEN_KIT_PLACEHOLDER=Digite o nome do KIT
EQUIP_SCREEN_NOTE_PLACEHOLDER=Observações (opcional)

ENTRY_SCREEN_AUTH_FAIL=Dois funcionários precisam ser identificados para acessar essa tela!
ENTRY_SCREEN_HEADER_TXT=Cadastrar transação
ENTRY_SCREEN_EQUIP_PLACEHOLDER=Digite o ID do equipamento
ENTRY_SCREEN_NOTE_TXT=Observações (opcional)
ENTRY_SCREEN_NEXT_BTN=Próximo
ENTRY_SCREEN_KIND_400=É necessário selecionar um tipo de transação!
ENTRY_SCREEN_STATUS_400=É necessário selecionar um tipo de situação!
ENTRY_SCREEN_EQUIP_400=O número do ID do equipamento é inválido! {0}
ENTRY_SCREEN_EQUIP_404=Equipamento não foi encontrado!
ENTRY_SCREEN_SAVE_EMPTY=Necessário adicionar alguma transação antes de salvar!
ENTRY_SCREEN_EQUIP_KIT=O kit do equipamento informado é diferente dos anteriores!\n\nAtual: {0}\n\nEsperado: {1}\n\nDeseja continuar a inserção mesmo assim?
ENTRY_SCREEN_DIFF_KITS=O kit informado está diferente do armazenado!\n\n{0}\n\nDeseja forçar a inserção?
ENTRY_SCREEN_NO_FORCE=Operação cancelada pelo usuário!
ENTRY_SCREEN_DIFF_EMPLOYER=O funcionário que está devolvendo é diferente do que retirou o kit!\n\nAtual: {0}\n\nEsperado: {1}\n\nDeseja continuar a inserção mesmo assim?
ENTRY_SCREEN_DIFF_TRANSACTION=O equipamento não pode ser transacionado!\n\nERRO: {0}\n\nDeseja forçar a inserção mesmo assim?
ENTRY_SCREEN_STATUS_MISS=Faltando
ENTRY_SCREEN_STATUS_FINE=Correto
ENTRY_SCREEN_STATUS_MORE=Sobrando

DIFF_DIALOG_TITLE_TXT=Confirmação
DIFF_DIALOG_HEADER_TXT=Confirmar registro de Kit
DIFF_DIALOG_ALERT_TXT=O kit informado está diferente do armazenado!
DIFF_DIALOG_FOOTER_TXT=Deseja salvar alterações?
DIFF_DIALOG_CONFIRM_BTN=Sim
DIFF_DIALOG_CANCEL_BTN=Não

REPORT_SCREEN_HEADER_TXT=Relatório transações

CHECK_SCREEN_HEADER_TXT=Busca de kits
CHECK_SCREEN_GROUP_TXT=Informe o kit
CHECK_SCREEN_SEARCH_BTN=Buscar

MODELS_EMPLOYER_REGISTRY_ARG=A matrícula `{0}` é inválida!

MODEL_TRANSACTION_NEXT_NULL=A transação para comparação é nula!
MODEL_TRANSACTION_THIS_REMOVED=O equipamento marcado como removido não pode ser transacionado!
MODEL_TRANSACTION_NEXT_IDLE=Não é permitido transacionar o equipamento para parado!
MODEL_TRANSACTION_BOTH_SAME=O equipamento já está no estado solicitado! {0}
MODEL_TRANSACTION_IDLE_TO=O equipamento não pode ser trocado para o estado {0}!\nÚnica transação permitida é o check-in!
MODEL_TRANSACTION_CHECKIN_TO=O equipamento não pode ser trocado para o estado check-in!\nNecessário realizar o checkout antes!
MODEL_TRANSACTION_CHECKOUT_TO=O equipamento não pode ser trocado para o estado checkout!\nNecessário realizar o check-in antes!
MODEL_TRANSACTION_TOREPAIR_TO=O equipamento não pode ser trocado para o estado manutenção!\nNecessário realizar o checkout antes!
MODEL_TRANSACTION_FROMREPAIR_TO=O equipamento não pode ser trocado para o estado {0}!\nNecessário retornar da manutenção antes!
MODEL_TRANSACTION_REMOVED_TO=O equipamento não pode ser trocado para o estado removido!\nNecessário realizar o checkout antes!

MODEL_TRANSACTION_EMPTY_KIT=O kits enviado está vazio!
MODEL_TRANSACTION_KIT_OVERFLOW=O kit tem mais itens que o permitido!
MODEL_TRANSACTION_MULTI_KINDS=O kit contém equipamentos de mesmo tipo!
