# Mapeamento: Permissions → Roles

## Regras de Autorização por Recurso

### Banners
| Ação | Permission | Roles Permitidas | Política |
|------|------------|-----------------|----------|
| Create | banners:create | Admin, Prefeitura | AdminOrPrefeituraPolicy |
| Read | banners:read | Público (AllowAnonymous) | AllowAnonymous |
| Update | banners:update | Admin, Prefeitura | AdminOrPrefeituraPolicy |
| Delete | banners:delete | Admin | AdminPolicy |
| Reorder | banners:reorder | Admin, Prefeitura | AdminOrPrefeituraPolicy |

### Atrativos
| Ação | Permission | Roles Permitidas | Política |
|------|------------|-----------------|----------|
| Create | atrativos:create | Admin, Prefeitura | AdminOrPrefeituraPolicy |
| Read | atrativos:read | Todos (AllowAnonymous) | AllowAnonymous |
| Update | atrativos:update | Admin, Prefeitura | AdminOrPrefeituraPolicy |
| Delete | atrativos:delete | Admin | AdminPolicy |

### Quiosques
| Ação | Permission | Roles Permitidas | Política |
|------|------------|-----------------|----------|
| Create | quiosques:create | Admin, Prefeitura | AdminOrPrefeituraPolicy |
| Read | quiosques:read | Todos (AllowAnonymous) | AllowAnonymous |
| Update | quiosques:update | Admin, Prefeitura | AdminOrPrefeituraPolicy |
| Delete | quiosques:delete | Admin | AdminPolicy |

### Reservas
| Ação | Permission | Roles Permitidas | Política |
|------|------------|-----------------|----------|
| Create | reservas:create | Todos (AllowAnonymous) | AllowAnonymous |
| Read | reservas:read | Admin, Prefeitura, Balneario | AnyAuthenticatedPolicy |
| Update | reservas:update | Admin, Prefeitura | AdminOrPrefeituraPolicy |
| Delete | reservas:delete | Admin | AdminPolicy |
| Validate | reservas:validate | Admin, Balneario | AdminOrBalnearioPolicy |

### Usuários (Profiles)
| Ação | Permission | Roles Permitidas | Política |
|------|------------|-----------------|----------|
| Create | profiles:create | Admin | AdminPolicy |
| Read | profiles:read | Admin, Prefeitura | AdminOrPrefeituraPolicy |
| Update | profiles:update | Admin | AdminPolicy |
| Delete | profiles:delete | Admin | AdminPolicy |
| Me | - | Qualquer autenticado | AnyAuthenticatedPolicy |

### Configurações
| Ação | Permission | Roles Permitidas | Política |
|------|------------|-----------------|----------|
| Read | configuracoes:read | Admin, Prefeitura | AdminOrPrefeituraPolicy |
| Update | configuracoes:update | Admin | AdminPolicy |

### Municípios
| Ação | Permission | Roles Permitidas | Política |
|------|------------|-----------------|----------|
| Read | municipios:read | Todos (AllowAnonymous) | AllowAnonymous |

## Resumo das Políticas por Endpoint

| Endpoint | Método | Política |
|----------|--------|----------|
| /api/banners | POST | AdminOrPrefeituraPolicy |
| /api/banners | GET | AllowAnonymous |
| /api/banners/{id} | GET | AllowAnonymous |
| /api/banners/{id} | PUT | AdminOrPrefeituraPolicy |
| /api/banners/{id} | DELETE | AdminPolicy |
| /api/banners/reorder | POST | AdminOrPrefeituraPolicy |
| /api/atrativos | GET | AllowAnonymous |
| /api/atrativos/{id} | GET | AllowAnonymous |
| /api/quiosques | POST | AdminOrPrefeituraPolicy |
| /api/quiosques | GET | AllowAnonymous |
| /api/quiosques/{id} | GET | AllowAnonymous |
| /api/quiosques/{id} | PUT | AdminOrPrefeituraPolicy |
| /api/quiosques/{id} | DELETE | AdminPolicy |
| /api/reservas | POST | AllowAnonymous |
| /api/reservas | GET | AnyAuthenticatedPolicy |
| /api/reservas/{id} | GET | AnyAuthenticatedPolicy |
| /api/reservas/{id}/status | PUT | AdminOrPrefeituraPolicy |
| /api/validacoes/validar | POST | AdminOrBalnearioPolicy |
| /api/usuarios | GET | AdminOrPrefeituraPolicy |
| /api/usuarios | POST | AdminPolicy |
| /api/usuarios/{id} | GET | AdminOrPrefeituraPolicy |
| /api/usuarios/{id} | PUT | AdminPolicy |
| /api/usuarios/{id} | DELETE | AdminPolicy |
| /api/profiles/me | GET | AnyAuthenticatedPolicy |
| /api/configuracoes | GET | AdminOrPrefeituraPolicy |
| /api/configuracoes | PUT | AdminPolicy |
| /api/municipios | GET | AllowAnonymous |
