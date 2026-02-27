-- =====================================================
-- Script de Inserção de Dados de Exemplo - Supabase
-- EcoTurismo - Sistema de Gestão de Atrativos Turísticos
-- =====================================================
-- 
-- Este script insere:
-- - 1 Município (se necessário)
-- - 1 Atrativo Turístico
-- - 1 Configuração do Sistema
-- - 2 Quiosques vinculados ao atrativo
-- - 3 Reservas de exemplo
--
-- Execute no SQL Editor do Supabase
-- =====================================================

-- Variáveis (ajuste conforme necessário)
DO $$
DECLARE
    v_municipio_id uuid;
    v_atrativo_id uuid;
    v_quiosque1_id uuid;
    v_quiosque2_id uuid;
    v_now timestamptz := now();
BEGIN

-- =====================================================
-- 1. VERIFICAR/CRIAR MUNICÍPIO
-- =====================================================
SELECT "Id" INTO v_municipio_id
FROM "Municipios"
WHERE "Nome" = 'Rio Verde de Mato Grosso'
LIMIT 1;

IF v_municipio_id IS NULL THEN
    v_municipio_id := gen_random_uuid();
    
    INSERT INTO "Municipios" (
        "Id",
        "Nome",
        "Uf",
        "Logo",
        "CreatedAt"
    ) VALUES (
        v_municipio_id,
        'Rio Verde de Mato Grosso',
        'MS',
        NULL,
        v_now
    );
    
    RAISE NOTICE 'Município criado: %', v_municipio_id;
ELSE
    RAISE NOTICE 'Município já existe: %', v_municipio_id;
END IF;

-- =====================================================
-- 2. CRIAR ATRATIVO
-- =====================================================
v_atrativo_id := gen_random_uuid();

INSERT INTO "Atrativos" (
    "Id",
    "MunicipioId",
    "Nome",
    "Tipo",
    "Descricao",
    "Imagem",
    "CapacidadeMaxima",
    "OcupacaoAtual",
    "Status",
    "CreatedAt",
    "UpdatedAt"
) VALUES (
    v_atrativo_id,
    v_municipio_id,
    'Balneário das Águas Claras',
    'balneario',
    'Complexo turístico com piscinas naturais de águas cristalinas, rodeado pela natureza do cerrado. Oferece estrutura completa com churrasqueiras, quadras esportivas, parquinho infantil e área de camping. Perfeito para passar o dia em família!',
    'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800&h=600&fit=crop',
    500,
    0,
    'ativo',
    v_now,
    v_now
);

RAISE NOTICE 'Atrativo criado: % - %', v_atrativo_id, 'Balneário das Águas Claras';

-- =====================================================
-- 3. CRIAR CONFIGURAÇÃO DO SISTEMA
-- =====================================================
INSERT INTO "Configuracoes" (
    "Id",
    "Chave",
    "Valor",
    "Descricao",
    "UpdatedAt"
) VALUES (
    gen_random_uuid(),
    'capacidade_maxima_balneario',
    '500',
    'Capacidade máxima de visitantes simultâneos no balneário',
    v_now
) ON CONFLICT ("Chave") DO UPDATE
SET 
    "Valor" = EXCLUDED."Valor",
    "Descricao" = EXCLUDED."Descricao",
    "UpdatedAt" = v_now;

RAISE NOTICE 'Configuração criada/atualizada: capacidade_maxima_balneario';

-- =====================================================
-- 4. CRIAR QUIOSQUES
-- =====================================================

-- Quiosque 1
v_quiosque1_id := gen_random_uuid();

INSERT INTO "Quiosques" (
    "Id",
    "AtrativoId",
    "Numero",
    "TemChurrasqueira",
    "Status",
    "PosicaoX",
    "PosicaoY",
    "CreatedAt",
    "UpdatedAt"
) VALUES (
    v_quiosque1_id,
    v_atrativo_id,
    1,
    true,
    'disponivel',
    10,
    20,
    v_now,
    v_now
);

RAISE NOTICE 'Quiosque 1 criado: %', v_quiosque1_id;

-- Quiosque 2
v_quiosque2_id := gen_random_uuid();

INSERT INTO "Quiosques" (
    "Id",
    "AtrativoId",
    "Numero",
    "TemChurrasqueira",
    "Status",
    "PosicaoX",
    "PosicaoY",
    "CreatedAt",
    "UpdatedAt"
) VALUES (
    v_quiosque2_id,
    v_atrativo_id,
    2,
    true,
    'disponivel',
    30,
    20,
    v_now,
    v_now
);

RAISE NOTICE 'Quiosque 2 criado: %', v_quiosque2_id;

-- =====================================================
-- 5. CRIAR RESERVAS DE EXEMPLO
-- =====================================================

-- Reserva 1 - Day Use (sem quiosque)
INSERT INTO "Reservas" (
    "Id",
    "AtrativoId",
    "QuiosqueId",
    "NomeVisitante",
    "Email",
    "Cpf",
    "CidadeOrigem",
    "UfOrigem",
    "Tipo",
    "Data",
    "DataFim",
    "QuantidadePessoas",
    "Status",
    "Token",
    "CreatedAt"
) VALUES (
    gen_random_uuid(),
    v_atrativo_id,
    NULL,
    'Maria Silva Santos',
    'maria.silva@email.com',
    '123.456.789-00',
    'Campo Grande',
    'MS',
    'day_use',
    CURRENT_DATE + INTERVAL '7 days',
    NULL,
    4,
    'confirmada',
    substring(md5(random()::text) from 1 for 8),
    v_now
);

-- Reserva 2 - Quiosque 1
INSERT INTO "Reservas" (
    "Id",
    "AtrativoId",
    "QuiosqueId",
    "NomeVisitante",
    "Email",
    "Cpf",
    "CidadeOrigem",
    "UfOrigem",
    "Tipo",
    "Data",
    "DataFim",
    "QuantidadePessoas",
    "Status",
    "Token",
    "CreatedAt"
) VALUES (
    gen_random_uuid(),
    v_atrativo_id,
    v_quiosque1_id,
    'João Pedro Oliveira',
    'joao.pedro@email.com',
    '987.654.321-00',
    'Dourados',
    'MS',
    'quiosque',
    CURRENT_DATE + INTERVAL '5 days',
    NULL,
    6,
    'confirmada',
    substring(md5(random()::text) from 1 for 8),
    v_now
);

-- Reserva 3 - Quiosque 2
INSERT INTO "Reservas" (
    "Id",
    "AtrativoId",
    "QuiosqueId",
    "NomeVisitante",
    "Email",
    "Cpf",
    "CidadeOrigem",
    "UfOrigem",
    "Tipo",
    "Data",
    "DataFim",
    "QuantidadePessoas",
    "Status",
    "Token",
    "CreatedAt"
) VALUES (
    gen_random_uuid(),
    v_atrativo_id,
    v_quiosque2_id,
    'Ana Carolina Ferreira',
    'ana.ferreira@email.com',
    '456.789.123-00',
    'Corumbá',
    'MS',
    'quiosque',
    CURRENT_DATE + INTERVAL '10 days',
    NULL,
    8,
    'confirmada',
    substring(md5(random()::text) from 1 for 8),
    v_now
);

RAISE NOTICE 'Reservas criadas: 3 reservas de exemplo';

-- =====================================================
-- RESUMO
-- =====================================================
RAISE NOTICE '============================================';
RAISE NOTICE 'Script executado com sucesso!';
RAISE NOTICE '============================================';
RAISE NOTICE 'Dados inseridos:';
RAISE NOTICE '- Município: %', v_municipio_id;
RAISE NOTICE '- Atrativo: % (Balneário das Águas Claras)', v_atrativo_id;
RAISE NOTICE '- Quiosques: 2';
RAISE NOTICE '- Reservas: 3';
RAISE NOTICE '- Configurações: 1';
RAISE NOTICE '============================================';

END $$;

-- =====================================================
-- CONSULTAS DE VERIFICAÇÃO
-- =====================================================

-- Verificar atrativo criado
SELECT 
    a."Id",
    a."Nome",
    a."Tipo",
    a."Status",
    a."CapacidadeMaxima",
    m."Nome" as Municipio
FROM "Atrativos" a
JOIN "Municipios" m ON a."MunicipioId" = m."Id"
WHERE a."Nome" = 'Balneário das Águas Claras';

-- Verificar quiosques
SELECT 
    q."Id",
    q."Numero",
    q."TemChurrasqueira",
    q."Status",
    a."Nome" as Atrativo
FROM "Quiosques" q
JOIN "Atrativos" a ON q."AtrativoId" = a."Id"
WHERE a."Nome" = 'Balneário das Águas Claras'
ORDER BY q."Numero";

-- Verificar reservas
SELECT 
    r."Id",
    r."NomeVisitante",
    r."Email",
    r."Tipo",
    r."Data",
    r."QuantidadePessoas",
    r."Status",
    r."Token",
    a."Nome" as Atrativo,
    q."Numero" as Quiosque
FROM "Reservas" r
JOIN "Atrativos" a ON r."AtrativoId" = a."Id"
LEFT JOIN "Quiosques" q ON r."QuiosqueId" = q."Id"
WHERE a."Nome" = 'Balneário das Águas Claras'
ORDER BY r."Data";

-- Verificar configuração
SELECT * FROM "Configuracoes"
WHERE "Chave" = 'capacidade_maxima_balneario';
