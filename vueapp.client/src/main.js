import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'  

createApp(App)
  .use(router)  // Usando o router
  .mount('#app') // Montando no elemento #app

