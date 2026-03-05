# 📡 Guia de Endpoints - Upload de Imagens para Atrativos

## 🎯 Visão Geral

Endpoints para gerenciar imagens de atrativos turísticos. Todas as imagens são armazenadas em base64 no banco de dados.

---

## 🔐 Autenticação

Todos os endpoints (exceto GET) requerem autenticação JWT:

```javascript
headers: {
  'Authorization': `Bearer ${token}`
}
```

**Roles permitidas:** Admin, Prefeitura

---

## 📋 Endpoints Disponíveis

### 1. Upload de Imagens (POST)
### 2. Remover Imagem (DELETE)
### 3. Definir Imagem Principal (PUT)
### 4. Reordenar Imagens (PUT)
### 5. Listar Imagens (GET)

---

## 1️⃣ Upload de Imagens

**POST** `/api/uploads/atrativos/{atrativoId}/imagens`

### Request

```javascript
const formData = new FormData();

// Adicionar arquivos de imagem (obrigatório, mínimo 1, máximo 10)
formData.append('Imagens', file1);
formData.append('Imagens', file2);
formData.append('Imagens', file3);

// Descrições (opcional, uma para cada imagem)
formData.append('Descricoes', 'Foto principal');
formData.append('Descricoes', 'Vista lateral');
formData.append('Descricoes', 'Detalhe da cachoeira');

// Ordem customizada (opcional)
formData.append('Ordens', '1');
formData.append('Ordens', '2');
formData.append('Ordens', '3');

// ID da imagem que será marcada como principal (opcional)
formData.append('PrincipalId', 'uuid-da-imagem');

const response = await fetch(`/api/uploads/atrativos/${atrativoId}/imagens`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`
  },
  body: formData
});

const result = await response.json();
```

### Response 200 OK

```json
{
  "success": true,
  "message": "3 imagens adicionadas com sucesso",
  "data": {
    "atrativoId": "123e4567-e89b-12d3-a456-426614174000",
    "atrativoNome": "Cachoeira do Sol",
    "totalImagens": 5,
    "imagensAdicionadas": [
      {
        "id": "uuid-1",
        "url": "data:image/jpeg;base64,/9j/4AAQSkZJRg...",
        "ordem": 3,
        "principal": false,
        "descricao": "Foto principal"
      },
      {
        "id": "uuid-2",
        "url": "data:image/png;base64,iVBORw0KGg...",
        "ordem": 4,
        "principal": true,
        "descricao": "Vista lateral"
      },
      {
        "id": "uuid-3",
        "url": "data:image/jpeg;base64,/9j/4BBB...",
        "ordem": 5,
        "principal": false,
        "descricao": "Detalhe da cachoeira"
      }
    ]
  }
}
```

### Validações

- ✅ Formatos aceitos: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`
- ✅ Tamanho máximo: **5MB por imagem**
- ✅ Mínimo: **1 imagem**
- ✅ Máximo: **10 imagens por upload**
- ✅ Total máximo no atrativo: **20 imagens**

### Erros Comuns

```json
// 400 - Limite excedido
{
  "success": false,
  "errorMessage": "Limite de 20 imagens por atrativo excedido"
}

// 400 - Arquivo muito grande
{
  "errors": {
    "Imagens": ["Imagem não pode ter mais de 5MB"]
  }
}

// 404 - Atrativo não encontrado
{
  "success": false,
  "errorMessage": "Atrativo não encontrado"
}
```

---

## 2️⃣ Remover Imagem

**DELETE** `/api/uploads/atrativos/{atrativoId}/imagens/{imagemId}`

### Request

```javascript
const atrativoId = '123e4567-e89b-12d3-a456-426614174000';
const imagemId = 'uuid-da-imagem';

const response = await fetch(
  `/api/uploads/atrativos/${atrativoId}/imagens/${imagemId}`,
  {
    method: 'DELETE',
    headers: {
      'Authorization': `Bearer ${token}`
    }
  }
);

const result = await response.json();
```

### Response 200 OK

```json
{
  "success": true,
  "message": "Imagem removida com sucesso",
  "data": {
    "atrativoId": "123e4567-e89b-12d3-a456-426614174000",
    "imagemRemovidaId": "uuid-da-imagem",
    "totalImagensRestantes": 4
  }
}
```

### Comportamento

- Se remover a imagem principal, a próxima (menor ordem) vira principal automaticamente
- Se remover a última imagem, campo fica como array vazio `[]`

### Erros Comuns

```json
// 404 - Imagem não encontrada
{
  "success": false,
  "errorMessage": "Imagem não encontrada"
}
```

---

## 3️⃣ Definir Imagem Principal

**PUT** `/api/uploads/atrativos/{atrativoId}/imagens/{imagemId}/principal`

### Request

```javascript
const atrativoId = '123e4567-e89b-12d3-a456-426614174000';
const imagemId = 'uuid-da-imagem';

const response = await fetch(
  `/api/uploads/atrativos/${atrativoId}/imagens/${imagemId}/principal`,
  {
    method: 'PUT',
    headers: {
      'Authorization': `Bearer ${token}`
    }
  }
);

const result = await response.json();
```

### Response 200 OK

```json
{
  "success": true,
  "message": "Imagem definida como principal",
  "data": {
    "imagemId": "uuid-da-imagem",
    "atrativoId": "123e4567-e89b-12d3-a456-426614174000"
  }
}
```

### Comportamento

- Todas as outras imagens ficam com `principal: false`
- Apenas uma imagem pode ser principal por vez

---

## 4️⃣ Reordenar Imagens

**PUT** `/api/uploads/atrativos/{atrativoId}/imagens/reordenar`

### Request

```javascript
const atrativoId = '123e4567-e89b-12d3-a456-426614174000';

const response = await fetch(
  `/api/uploads/atrativos/${atrativoId}/imagens/reordenar`,
  {
    method: 'PUT',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      imagens: [
        { id: 'uuid-1', ordem: 1 },
        { id: 'uuid-2', ordem: 2 },
        { id: 'uuid-3', ordem: 3 },
        { id: 'uuid-4', ordem: 4 }
      ]
    })
  }
);

const result = await response.json();
```

### Response 200 OK

```json
{
  "success": true,
  "message": "Imagens reordenadas com sucesso",
  "data": {
    "atrativoId": "123e4567-e89b-12d3-a456-426614174000",
    "totalImagens": 4
  }
}
```

### Comportamento

- Ordem define a sequência de exibição (1, 2, 3, 4...)
- Todos os IDs fornecidos devem existir no atrativo

---

## 5️⃣ Listar Imagens do Atrativo

**GET** `/api/atrativos/{atrativoId}`

### Request

```javascript
const atrativoId = '123e4567-e89b-12d3-a456-426614174000';

const response = await fetch(`/api/atrativos/${atrativoId}`);
const atrativo = await response.json();
```

### Response 200 OK

```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "nome": "Cachoeira do Sol",
  "tipo": "cachoeira",
  "descricao": "Linda cachoeira...",
  "imagens": [
    {
      "id": "uuid-1",
      "url": "data:image/jpeg;base64,/9j/4AAQ...",
      "ordem": 1,
      "principal": true,
      "descricao": "Foto principal"
    },
    {
      "id": "uuid-2",
      "url": "data:image/png;base64,iVBORw0KGg...",
      "ordem": 2,
      "principal": false,
      "descricao": "Vista lateral"
    }
  ],
  "capacidadeMaxima": 100,
  "ocupacaoAtual": 25
}
```

### Comportamento

- Público (não requer autenticação)
- Imagens vêm ordenadas por `ordem`
- Se não houver imagens, `imagens: []`

---

## 💻 Exemplos Práticos

### React - Upload com Preview

```tsx
import React, { useState } from 'react';

function UploadImagensAtrativo({ atrativoId, onSuccess }) {
  const [files, setFiles] = useState([]);
  const [descricoes, setDescricoes] = useState({});
  const [loading, setLoading] = useState(false);

  const handleFileChange = (e) => {
    const selectedFiles = Array.from(e.target.files);
    
    // Validar tamanho
    const invalidFiles = selectedFiles.filter(f => f.size > 5 * 1024 * 1024);
    if (invalidFiles.length > 0) {
      alert('Algumas imagens excedem 5MB!');
      return;
    }

    // Máximo 10 imagens
    if (selectedFiles.length > 10) {
      alert('Máximo 10 imagens por upload!');
      return;
    }

    setFiles(selectedFiles);
  };

  const handleUpload = async () => {
    if (files.length === 0) return;

    setLoading(true);

    try {
      const formData = new FormData();
      
      files.forEach(file => {
        formData.append('Imagens', file);
      });

      // Adicionar descrições se informadas
      files.forEach((file, index) => {
        const desc = descricoes[index] || '';
        formData.append('Descricoes', desc);
      });

      const response = await fetch(
        `/api/uploads/atrativos/${atrativoId}/imagens`,
        {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          },
          body: formData
        }
      );

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.errorMessage);
      }

      const result = await response.json();
      alert(result.message);
      onSuccess?.();
    } catch (error) {
      alert(error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <input
        type="file"
        multiple
        accept="image/jpeg,image/png,image/gif,image/webp"
        onChange={handleFileChange}
        disabled={loading}
      />
      
      {files.length > 0 && (
        <div>
          <h3>{files.length} imagem(ns) selecionada(s)</h3>
          {files.map((file, index) => (
            <div key={index}>
              <img 
                src={URL.createObjectURL(file)} 
                alt={`Preview ${index}`}
                style={{ width: '100px' }}
              />
              <input
                type="text"
                placeholder="Descrição (opcional)"
                value={descricoes[index] || ''}
                onChange={(e) => setDescricoes({
                  ...descricoes,
                  [index]: e.target.value
                })}
              />
            </div>
          ))}
          <button onClick={handleUpload} disabled={loading}>
            {loading ? 'Enviando...' : 'Upload'}
          </button>
        </div>
      )}
    </div>
  );
}
```

### JavaScript Vanilla - Galeria com Ações

```javascript
// Carregar imagens
async function carregarImagens(atrativoId) {
  const response = await fetch(`/api/atrativos/${atrativoId}`);
  const atrativo = await response.json();
  
  renderizarGaleria(atrativo.imagens);
}

// Renderizar galeria
function renderizarGaleria(imagens) {
  const container = document.getElementById('galeria');
  
  container.innerHTML = imagens.map(img => `
    <div class="imagem-card">
      <img src="${img.url}" alt="${img.descricao || 'Imagem'}">
      <p>${img.descricao || ''}</p>
      ${img.principal ? '<span class="badge">Principal</span>' : ''}
      <button onclick="definirPrincipal('${img.id}')">
        Marcar como Principal
      </button>
      <button onclick="removerImagem('${img.id}')">
        Remover
      </button>
    </div>
  `).join('');
}

// Definir imagem principal
async function definirPrincipal(imagemId) {
  const response = await fetch(
    `/api/uploads/atrativos/${atrativoId}/imagens/${imagemId}/principal`,
    {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    }
  );
  
  const result = await response.json();
  if (result.success) {
    alert(result.message);
    carregarImagens(atrativoId); // Recarregar
  }
}

// Remover imagem
async function removerImagem(imagemId) {
  if (!confirm('Tem certeza que deseja remover esta imagem?')) return;
  
  const response = await fetch(
    `/api/uploads/atrativos/${atrativoId}/imagens/${imagemId}`,
    {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    }
  );
  
  const result = await response.json();
  if (result.success) {
    alert(result.message);
    carregarImagens(atrativoId); // Recarregar
  }
}
```

### Vue.js - Drag and Drop para Reordenar

```vue
<template>
  <div class="galeria-reordenar">
    <draggable 
      v-model="imagens" 
      @end="salvarOrdem"
      class="imagens-list"
    >
      <div 
        v-for="(imagem, index) in imagens" 
        :key="imagem.id"
        class="imagem-item"
      >
        <img :src="imagem.url" :alt="imagem.descricao">
        <span>Ordem: {{ index + 1 }}</span>
      </div>
    </draggable>
  </div>
</template>

<script>
import draggable from 'vuedraggable';

export default {
  components: { draggable },
  data() {
    return {
      imagens: []
    };
  },
  methods: {
    async salvarOrdem() {
      const payload = {
        imagens: this.imagens.map((img, index) => ({
          id: img.id,
          ordem: index + 1
        }))
      };

      const response = await fetch(
        `/api/uploads/atrativos/${this.atrativoId}/imagens/reordenar`,
        {
          method: 'PUT',
          headers: {
            'Authorization': `Bearer ${this.token}`,
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(payload)
        }
      );

      const result = await response.json();
      if (result.success) {
        this.$toast.success(result.message);
      }
    }
  }
};
</script>
```

---

## 📊 Fluxo Completo

```
1. Usuário seleciona múltiplas imagens
   ↓
2. Frontend valida (formato, tamanho)
   ↓
3. Cria FormData com arquivos + descrições
   ↓
4. POST /api/uploads/atrativos/{id}/imagens
   ↓
5. Backend converte para base64
   ↓
6. Retorna imagens salvas com IDs
   ↓
7. Frontend exibe galeria
   ↓
8. Usuário pode:
   - Marcar outra como principal (PUT)
   - Remover imagem (DELETE)
   - Reordenar (PUT)
```

---

## 🎨 Limites e Restrições

| Limite | Valor |
|--------|-------|
| Tamanho máximo por imagem | 5MB |
| Imagens por upload | 10 |
| Total de imagens por atrativo | 20 |
| Formatos aceitos | JPG, PNG, GIF, WebP |
| Imagens principais | 1 por atrativo |

---

## 🆘 Troubleshooting

### Erro: "Limite de 20 imagens excedido"
**Solução:** Remova imagens antigas antes de adicionar novas

### Erro: "Arquivo muito grande"
**Solução:** Comprima as imagens antes do upload ou use bibliotecas como `browser-image-compression`

### Erro: 401 Unauthorized
**Solução:** Verifique se o token JWT está válido e sendo enviado corretamente

### Imagem não aparece
**Solução:** Verifique se a resposta contém `url` em base64 começando com `data:image/`

---

## ✅ Checklist Frontend

- [ ] Validar formato de arquivo antes do upload
- [ ] Validar tamanho (máx 5MB por imagem)
- [ ] Limitar a 10 imagens por upload
- [ ] Mostrar preview antes do upload
- [ ] Exibir progresso de upload
- [ ] Exibir imagem principal com destaque
- [ ] Permitir reordenação (drag & drop)
- [ ] Confirmar antes de remover
- [ ] Tratar erros da API
- [ ] Recarregar galeria após operações

---

**📱 Endpoints prontos para uso no frontend!**
