-- =====================================================
-- Script Simplificado de Inserção - Supabase
-- EcoTurismo - Versão Direta (sem PL/pgSQL)
-- =====================================================
-- 
-- Execute este script no SQL Editor do Supabase
-- Cada INSERT pode ser executado individualmente
-- 
-- ⚠️ IMPORTANTE: Ajuste os IDs se necessário
-- =====================================================

-- =====================================================
-- 1. MUNICÍPIO (ajuste o ID se já existir)
-- =====================================================
INSERT INTO "Municipios" ("Id", "Nome", "Uf", "Logo", "CreatedAt")
VALUES (
    '11111111-1111-1111-1111-111111111111',
    'Rio Verde de Mato Grosso',
    'MS',
    NULL,
    NOW()
) ON CONFLICT ("Id") DO NOTHING;

-- =====================================================
-- 2. ATRATIVO
-- =====================================================
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
    '22222222-2222-2222-2222-222222222222',
    '11111111-1111-1111-1111-111111111111',
    'Balneário das Águas Claras',
    'balneario',
    'Complexo turístico com piscinas naturais de águas cristalinas, rodeado pela natureza do cerrado. Oferece estrutura completa com churrasqueiras, quadras esportivas, parquinho infantil e área de camping. Perfeito para passar o dia em família!',
    'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=800&h=600&fit=crop',
    500,
    0,
    'ativo',
    NOW(),
    NOW()
);

-- =====================================================
-- 3. CONFIGURAÇÃO
-- =====================================================
INSERT INTO "Configuracoes" (
    "Id",
    "Chave",
    "Valor",
    "Descricao",
    "UpdatedAt"
) VALUES (
    '33333333-3333-3333-3333-333333333333',
    'capacidade_maxima_balneario',
    '500',
    'Capacidade máxima de visitantes simultâneos no balneário',
    NOW()
) ON CONFLICT ("Chave") DO UPDATE
SET 
    "Valor" = EXCLUDED."Valor",
    "UpdatedAt" = NOW();

-- =====================================================
-- 4. QUIOSQUE 1
-- =====================================================
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
    '44444444-4444-4444-4444-444444444444',
    '22222222-2222-2222-2222-222222222222',
    1,
    true,
    'disponivel',
    10,
    20,
    NOW(),
    NOW()
);

-- =====================================================
-- 5. QUIOSQUE 2
-- =====================================================
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
    '55555555-5555-5555-5555-555555555555',
    '22222222-2222-2222-2222-222222222222',
    2,
    true,
    'disponivel',
    30,
    20,
    NOW(),
    NOW()
);

-- =====================================================
-- 6. RESERVA 1 - Day Use (sem quiosque)
-- =====================================================
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
    '66666666-6666-6666-6666-666666666666',
    '22222222-2222-2222-2222-222222222222',
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
    'ABC12345',
    NOW()
);

-- =====================================================
-- 7. RESERVA 2 - Quiosque 1
-- =====================================================
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
    '77777777-7777-7777-7777-777777777777',
    '22222222-2222-2222-2222-222222222222',
    '44444444-4444-4444-4444-444444444444',
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
    'XYZ98765',
    NOW()
);

-- =====================================================
-- 8. RESERVA 3 - Quiosque 2
-- =====================================================
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
    '88888888-8888-8888-8888-888888888888',
    '22222222-2222-2222-2222-222222222222',
    '55555555-5555-5555-5555-555555555555',
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
    'LMN45678',
    NOW()
);

-- =====================================================
-- CONSULTAS DE VERIFICAÇÃO
-- =====================================================

-- Ver atrativo criado
SELECT 
    a."Id",
    a."Nome",
    a."Tipo",
    a."Status",
    a."CapacidadeMaxima",
    m."Nome" as Municipio
FROM "Atrativos" a
JOIN "Municipios" m ON a."MunicipioId" = m."Id"
WHERE a."Id" = '22222222-2222-2222-2222-222222222222';

-- Ver quiosques
SELECT 
    q."Id",
    q."Numero",
    q."TemChurrasqueira",
    q."Status",
    a."Nome" as Atrativo
FROM "Quiosques" q
JOIN "Atrativos" a ON q."AtrativoId" = a."Id"
WHERE q."AtrativoId" = '22222222-2222-2222-2222-222222222222'
ORDER BY q."Numero";

-- Ver reservas
SELECT 
    r."Id",
    r."NomeVisitante",
    r."Email",
    r."Tipo",
    r."Data",
    r."QuantidadePessoas",
    r."Status",
    r."Token",
    CASE 
        WHEN r."QuiosqueId" IS NOT NULL 
        THEN CONCAT('Quiosque ', q."Numero")
        ELSE 'Day Use'
    END as TipoReserva
FROM "Reservas" r
LEFT JOIN "Quiosques" q ON r."QuiosqueId" = q."Id"
WHERE r."AtrativoId" = '22222222-2222-2222-2222-222222222222'
ORDER BY r."Data";

-- Ver configuração
SELECT * FROM "Configuracoes"
WHERE "Chave" = 'capacidade_maxima_balneario';

-- =====================================================
-- RESUMO DOS IDs CRIADOS
-- =====================================================
/*
Município: 11111111-1111-1111-1111-111111111111
Atrativo:  22222222-2222-2222-2222-222222222222
Config:    33333333-3333-3333-3333-333333333333
Quiosque1: 44444444-4444-4444-4444-444444444444
Quiosque2: 55555555-5555-5555-5555-555555555555
Reserva1:  66666666-6666-6666-6666-666666666666
Reserva2:  77777777-7777-7777-7777-777777777777
Reserva3:  88888888-8888-8888-8888-888888888888
*/
