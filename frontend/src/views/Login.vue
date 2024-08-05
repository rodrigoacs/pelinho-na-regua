<template>
  <div class="login-container">
    <div class="content">
      <img
        src="@/assets/pets.png"
        alt="Pets"
        class="pets-image"
      >
      <div class="login-form">
        <h2>Já tem cadastro?</h2>
        <p>Faça seu login</p>
        <form @submit.prevent="handleLogin">
          <div class="input-group">
            <i class="fa fa-user"></i>
            <input
              type="text"
              placeholder="Usuário"
              v-model="username"
            >
          </div>
          <div class="input-group">
            <i class="fa fa-lock"></i>
            <input
              type="password"
              placeholder="Senha"
              v-model="password"
            >
          </div>
          <button type="submit">ENTRAR</button>
        </form>
        <div class="footer">
          <a
            @mouseover="forgetPassword($event)"
            @mouseout="forgetPassword($event)"
          >
            Esqueci a senha
          </a>
          <router-link to="/cadastro">Criar conta</router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const username = ref('')
const password = ref('')
const router = useRouter()

function forgetPassword(e) {
  e.target.innerText = e.target.innerText === 'Esqueci a senha' ? 'Foda né? Sinto muito' : 'Esqueci a senha'
}

function handleLogin() {
  fetch('http://localhost:5000/api/Users/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      username: username.value,
      password: password.value
    })
  })
    .then(response => response.json())
    .then(data => {
      if (data.error) {
        alert(data.error)
        return
      }

      alert('Login feito com sucesso')
      router.push('/')
    })
    .catch(error => {
      console.error('Error:', error)
      alert('Erro ao fazer login')
    })
};

</script>

<style scoped>
.login-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  background-image: url(@/assets/background.jpg);
  background-size: contain;
  height: 100vh;
  padding: 20px;
}

.logo {
  height: 60px;
}

.content {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  flex-grow: 1;
}

.pets-image {
  max-width: 50%;
  transform: scaleX(-1);
}

.login-form {
  background-color: #a66a52;
  padding: 20px;
  border-radius: 10px;
  color: #ffffff;
  max-width: 400px;
  width: 100%;
}

.login-form h2 {
  margin: 0 0 10px;
}

.login-form p {
  margin: 0 0 20px;
}

.input-group {
  display: flex;
  align-items: center;
  margin-bottom: 20px;
}

.input-group i {
  margin-right: 10px;
}

.input-group input {
  width: 100%;
  padding: 10px;
  border: none;
  border-radius: 5px;
}

.login-form button {
  width: 100%;
  padding: 10px;
  background-color: #854204;
  color: #ffffff;
  border: none;
  border-radius: 5px;
}

.footer {
  display: flex;
  justify-content: space-between;
  margin-top: 10px;
}

.footer a {
  color: #ffffff;
  text-decoration: none;
}
</style>
