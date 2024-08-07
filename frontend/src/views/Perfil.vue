<template>
  <div class="profile-container">
    <h1>Perfil do Usuário</h1>
    <form @submit.prevent="handleSubmit">
      <div class="form-group">
        <label for="fullName">*Nome Completo</label>
        <input
          type="text"
          id="fullName"
          v-model="user.fullName"
          required
        />
      </div>
      <div class="form-group">
        <label for="phone">Telefone de contato</label>
        <input
          type="tel"
          id="phone"
          v-model="user.phone"
        />
      </div>
      <div class="address-group">
        <div class="form-group">
          <label for="street">*Rua, Avenida...</label>
          <input
            type="text"
            id="street"
            v-model="user.street"
            required
          />
        </div>
        <div class="form-group">
          <label for="number">Número</label>
          <input
            type="text"
            id="number"
            v-model="user.number"
          />
        </div>
        <div class="form-group">
          <label for="complement">Complemento</label>
          <input
            type="text"
            id="complement"
            v-model="user.complement"
          />
        </div>
        <div class="form-group">
          <label for="neighborhood">Bairro</label>
          <input
            type="text"
            id="neighborhood"
            v-model="user.neighborhood"
          />
        </div>
        <div class="form-group">
          <label for="city">Cidade</label>
          <input
            type="text"
            id="city"
            v-model="user.city"
          />
        </div>
      </div>
      <button type="submit">Salvar</button>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const user = ref({
  fullName: '',
  phone: '',
  street: '',
  neighborhood: '',
  city: '',
  number: '',
  complement: '',
})

const userId = localStorage.getItem('userId')

async function fetchUserProfile() {
  try {
    const response = await fetch(`http://195.200.2.145:5000/api/UserProfiles/${userId}`)
    const data = await response.json()
    if (response.ok) {
      user.value = data
    } else if (response.status === 404) {
      console.log('Perfil do usuário não encontrado, criando novo perfil...')
    } else {
      throw new Error('Erro ao obter perfil do usuário')
    }
  } catch (error) {
    console.error('Erro ao obter perfil do usuário:', error)
  }
}

onMounted(() => {
  fetchUserProfile()
})

async function handleSubmit() {
  try {
    const response = await fetch(`http://195.200.2.145:5000/api/UserProfiles/${userId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(user.value)
    })

    if (response.status !== 200) {
      await createUserProfile()
    } else if (response.ok) {
      alert('Informações salvas com sucesso!')
    } else {
      alert('Erro ao salvar informações')
    }
  } catch (error) {
    console.error('Erro ao salvar perfil do usuário:', error)
  }
}

async function createUserProfile() {
  try {
    const response = await fetch('http://195.200.2.145:5000/api/UserProfiles', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        userId,
        ...user.value
      })
    })

    if (response.ok) {
      alert('Perfil criado com sucesso!')
    } else {
      alert('Erro ao criar perfil')
    }
  } catch (error) {
    console.error('Erro ao criar perfil do usuário:', error)
  }
}
</script>

<style scoped>
.profile-container {
  background-color: #f5f5f5;
  padding: 20px;
  border-radius: 10px;
  max-width: 800px;
  margin: auto;
}

h1 {
  text-align: center;
  color: #b5651d;
}

form {
  display: flex;
  flex-direction: column;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  color: #b5651d;
  margin-bottom: 5px;
}

.form-group input {
  width: 100%;
  padding: 8px;
  border-radius: 5px;
  border: 1px solid #ccc;
}

.address-group {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.address-group .form-group {
  flex: 1;
  min-width: calc(33% - 10px);
}

button {
  background-color: #b5651d;
  color: white;
  border: none;
  padding: 10px;
  border-radius: 5px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button:hover {
  background-color: #94410b;
}
</style>
