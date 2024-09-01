<template>
  <div class="calendar-container">
    <FullCalendar :options="calendarOptions" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import FullCalendar from '@fullcalendar/vue3'
import dayGridPlugin from '@fullcalendar/daygrid'
import timeGridPlugin from '@fullcalendar/timegrid'
import interactionPlugin from '@fullcalendar/interaction'

const calendarOptions = ref({
  plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
  initialView: 'timeGridWeek',
  events: [],
  editable: true,
  selectable: true,
  dateClick: handleDateClick,
  eventClick: handleEventClick,
  eventDrop: handleEventDrop,
  eventResize: handleEventResize,
  weekends: false,
  allDaySlot: false,
  locale: 'pt-br',
  slotLabelFormat: {
    hour: '2-digit',
    minute: '2-digit',
    omitZeroMinute: false,
    hour12: false
  },
  eventTimeFormat: {
    hour: '2-digit',
    minute: '2-digit',
    omitZeroMinute: false,
    hour12: false
  },
  slotMinTime: '08:00:00',
  slotMaxTime: '18:00:00',
})

const userId = localStorage.getItem('userId')

async function fetchEvents() {
  try {
    const response = await fetch('http://195.200.2.145:5000/api/Events')
    const data = await response.json()

    // Verifique se o data é um array e adicione os eventos ao calendário
    if (Array.isArray(data)) {
      calendarOptions.value.events = data.map(event => ({
        id: event.id,
        title: event.title,
        start: event.startTime,
        end: event.endTime,
        editable: event.userId === parseInt(userId), // Apenas eventos do usuário logado são editáveis
        className: event.userId === parseInt(userId) ? 'user-event' : 'booked-event', // Diferenciar eventos
        extendedProps: { userId: event.userId }  // Adicionando o userId aos extendedProps
      }))
    }
  } catch (error) {
    console.error('Erro ao buscar eventos:', error)
  }
}

function isTimeSlotAvailable(startTime, endTime) {
  const start = new Date(startTime)
  const end = new Date(endTime)

  return !calendarOptions.value.events.some(event => {
    const eventStart = new Date(event.start)
    const eventEnd = new Date(event.end)

    return (
      (start < eventEnd && end > eventStart)
    )
  })
}

function handleDateClick(info) {
  const startTime = info.dateStr
  const endTime = new Date(info.date)
  endTime.setMinutes(endTime.getMinutes() + 30) // Defina a duração do evento

  if (isTimeSlotAvailable(startTime, endTime.toISOString())) {
    const title = prompt('Digite o título do evento:')
    if (title) {
      const newEvent = {
        title,
        start: startTime,
        end: endTime.toISOString(),
        userId, // associar ao usuário logado
      }
      createEvent(newEvent)
    }
  } else {
    alert('O horário selecionado já está ocupado.')
  }
}

// Função para lidar com o clique em um evento
function handleEventClick(info) {
  const eventUserId = info.event.extendedProps.userId
  console.log(eventUserId, userId)

  if (eventUserId === parseInt(userId)) {
    const confirmed = confirm(`Deseja deletar o evento: ${info.event.title}?`)
    if (confirmed) {
      deleteEvent(info.event.id)
    }
  } else {
    alert('Você não tem permissão para editar este evento.')
  }
}

// Função para criar um novo evento no backend
async function createEvent(event) {
  try {
    event.startTime = new Date(event.start).toISOString()
    event.endTime = new Date(event.end).toISOString()

    const response = await fetch('http://195.200.2.145:5000/api/Events', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(event),
    })

    const createdEvent = await response.json()

    calendarOptions.value.events = data.map(event => ({
      id: event.id,
      title: event.title,
      start: event.startTime,
      end: event.endTime,
      editable: event.userId === parseInt(userId), // Apenas eventos do usuário logado são editáveis
      className: event.userId === parseInt(userId) ? 'user-event' : 'booked-event', // Diferenciar eventos
      extendedProps: { userId: event.userId }  // Adicionando o userId aos extendedProps
    }))


    if (response.ok) {
      await fetchEvents()
    }
  } catch (error) {
    console.error('Erro ao criar evento:', error)
  }
}

// Função para deletar um evento do backend
async function deleteEvent(eventId) {
  try {
    const response = await fetch(`http://195.200.2.145:5000/api/Events/${eventId}`, {
      method: 'DELETE',
    })

    if (response.ok) {
      await fetchEvents()
    } else {
      console.error('Erro ao deletar evento:', await response.text())
    }
  } catch (error) {
    console.error('Erro ao deletar evento:', error)
  }
}

// Função para lidar com o movimento de um evento
async function handleEventDrop(info) {
  const updatedEvent = {
    id: info.event.id,
    title: info.event.title,
    start: info.event.start.toISOString(),
    end: info.event.end ? info.event.end.toISOString() : info.event.start.toISOString(),
  }
  updateEvent(updatedEvent)
}

// Função para lidar com o redimensionamento de um evento
async function handleEventResize(info) {
  const updatedEvent = {
    id: info.event.id,
    title: info.event.title,
    start: info.event.start.toISOString(),
    end: info.event.end.toISOString(),
  }
  updateEvent(updatedEvent)
}

// Função para atualizar um evento no backend
async function updateEvent(event) {
  try {
    const response = await fetch(`http://195.200.2.145:5000/api/Events/${event.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(event),
    })

    if (!response.ok) {
      console.error('Erro ao atualizar evento:', await response.text())
    }
  } catch (error) {
    console.error('Erro ao atualizar evento:', error)
  }
}

function setEventsColors() {
  document.querySelectorAll('.fc-event.booked-event').forEach(event => {
    event.style.backgroundColor = '#da0000'
    event.style.borderColor = '#da0000'
    event.style.cursor = 'not-allowed'
    event.style.opacity = 0.5
  })

  document.querySelectorAll('.fc-event.user-event').forEach(event => {
    event.style.backgroundColor = '#00ac00'
    event.style.borderColor = '#00ac00'
  })
}

// Chama a função para buscar os eventos quando o componente é montado
onMounted(() => {
  fetchEvents()
  setTimeout(() => {
    setEventsColors()
  }, 100)
})
</script>

<style scoped>
.calendar-container {
  max-width: 900px;
  margin: auto;
  padding: 20px;
  background-color: #f5f5f5;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.fc {
  height: 571px;
  margin: 50px;
}
</style>
