<script setup>
import { onMounted, ref, computed } from 'vue';
// BACKEND API
const tasks = ref([]);
const loading = ref(false);
const error = ref(null);
const apiEndpoint = '/api/TodoItems'

// FORM
const newTitle = ref('');
const newDescription = ref('');
const isTaskCompleted = ref(false);

function getTomorrow(){
  const date = new Date();
  date.setDate(date.getDate() + 1);
  return date.toISOString().slice(0, 10);
}
const newDueDate = ref(getTomorrow());
const isAddTaskFormVisible = ref(false);
const isEditingTask = ref(false);
const editingTaskId = ref(null);

// UPCOMING TASKS
const upcomingTasksVisible = ref(true);

// FILTERS
const selectedDate = ref('');

// FILTER TASKS BY SELECTED DATE
const filterTasksByDate = computed(() => {
  if (!selectedDate.value) {
    return tasks.value;
  }
  return tasks.value.filter(task => {
    if (!task.todoItemDueDate) return false;
    return task.todoItemDueDate.slice(0, 10) === selectedDate.value;
  });
});

// FILTER TASKS BY UPCOMING (TODAY AND TOMORROW)
const upcomingTasks = computed(() => {
  const today = new Date();
  const tomorrow = new Date();
  tomorrow.setDate(today.getDate() + 1);

  return tasks.value.filter(task => {
    if (!task.todoItemDueDate) return false;
    if (task.todoItemIsDone) return false;
    const taskDate = new Date(task.todoItemDueDate);
    return taskDate.toDateString() === today.toDateString() || taskDate.toDateString() === tomorrow.toDateString();
  });
});

async function loadTasks () {
  loading.value = true;
  error.value = null;
  try{
    const res = await fetch(apiEndpoint);
    const data = await res.json();
    tasks.value = data;
  } catch (e){
    error.value = 'Nie udało się załadować zadań.';
  } finally {
    loading.value = false;
  }
}

async function addTask (title, description, isDone, dueDate) {
  if (loading.value)
    return;

  loading.value = true;
  error.value = null;
  const newTask = {
    todoItemTitle: title,
    todoItemDescription: description,
    todoItemIsDone: isDone,
    todoItemDueDate: dueDate || null
  };

  try {
    const res = await fetch(apiEndpoint, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newTask)
    });

    if (!res.ok) {
      throw new Error('Nie udało się dodać tego zadania.');
    }

    const createdTask = await res.json();
    tasks.value.push(createdTask);

    await loadTasks();
  } catch (e) {
    error.value = e.message;
    loading.value = false;
  } finally {
    loading.value = false;
  }
}

async function deleteTask (id) {
  try {
    const res = await fetch(`${apiEndpoint}/${id}`, {
      method: 'DELETE'
    });

    if (!res.ok) {
      throw new Error('Nie udało się usunąć tego zadania.');
    }

    tasks.value = tasks.value.filter(task => task.todoItemId !== id);

    if (isEditingTask.value && editingTaskId.value === id) {
      await toggleTaskEditAdd(false);
    }

    await loadTasks();
  } catch (e) {
    error.value = e.message;
  }
}

async function editTask(id, title, description, isDone, dueDate) {
  const updatedTask = {
    todoItemId: id,
    todoItemTitle: title,
    todoItemDescription: description,
    todoItemIsDone: isDone,
    todoItemDueDate: dueDate || null
  };

  try {
    const res = await fetch(`${apiEndpoint}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updatedTask)
    });

    if (!res.ok) {
      throw new Error('Nie udało się zaktualizować tego zadania.');
    }

    const index = tasks.value.findIndex(task => task.todoItemId === id);
    if (index !== -1) {
      tasks.value[index] = updatedTask;
    }

    await loadTasks();
    
  } catch (e) {
    error.value = e.message;
  }
}

async function openEditTaskMenu(id){
  isEditingTask.value = true;
  editingTaskId.value = id;

  const taskToEdit = tasks.value.find(task => task.todoItemId === id);
  if (!taskToEdit){
    error.value = 'Nie udało się znaleźć zadania do edycji.';
    return;
  }

  newTitle.value = taskToEdit.todoItemTitle;
  newDescription.value = taskToEdit.todoItemDescription;
  newDueDate.value = taskToEdit.todoItemDueDate ? taskToEdit.todoItemDueDate.slice(0,10) : getTomorrow();
  isTaskCompleted.value = taskToEdit.todoItemIsDone;
  isAddTaskFormVisible.value = true;
}

async function toggleTaskEditAdd(state){
  isAddTaskFormVisible.value = state
  isTaskCompleted.value = false;
  isEditingTask.value = false;
  editingTaskId.value = null;
  
  newDueDate.value = getTomorrow();

  newTitle.value = '';
  newDescription.value = '';

  error.value = null;
}

async function handleSubmit(){
  if (!newTitle.value.trim()){
    error.value = 'Tytuł zadania nie może być pusty.';
    return
  }

  if (newTitle.value.length > 30){
    error.value = 'Tytuł zadania nie może przekraczać 30 znaków.';
    return
  }

  if (newDescription.value.length > 500){
    error.value = 'Opis zadania nie może przekraczać 500 znaków.';
    return
  }

  if (newDueDate.value){
    const today = new Date();
    const dueDate = new Date(newDueDate.value);
    today.setHours(0,0,0,0);
    dueDate.setHours(0,0,0,0);
    if (dueDate < today){
      error.value = 'Data terminu nie może być z przeszłości.';
      return
    }
  }

    if (isEditingTask.value && editingTaskId.value != null){
      await editTask(editingTaskId.value, newTitle.value, newDescription.value, isTaskCompleted.value, newDueDate.value)
    }else{
      await addTask(newTitle.value, newDescription.value, isTaskCompleted.value, newDueDate.value)
    }
    await toggleTaskEditAdd(false);
}

onMounted(() => {
  loadTasks();
})

</script>

<template>
  <v-app>
    <v-main>
      <v-container max-width="700">
        <!-- UPCOMING TASKS -->
        <v-list v-if="upcomingTasksVisible && upcomingTasks.length > 0" class="my-4 py-4 d-flex flex-column position-relative">

          <v-btn
          icon="mdi-close"
          size="small"
          variant="text"
          class="close-x-btn"
          @click="upcomingTasksVisible = false"
          />

          <div class="d-flex align-center justify-center">
            <v-icon left class="text-h5 mr-2">mdi-calendar-clock</v-icon>
            <h1 class="text-h5">
              Wkrótce kończące się zadania
            </h1>
          </div>

          <v-list-item v-for="task in upcomingTasks" :key="task.todoItemId" class="my-8 bg-grey-darken-3">
            <v-list-item-title class="pa-2 text-h5">
              <v-icon left v-if="task.todoItemIsDone">mdi-checkbox-marked-circle-outline</v-icon>
              <b>{{ task.todoItemTitle }}</b>
            </v-list-item-title>
            <v-list-item-subtitle class="pa-2">
              <b>Status:</b> <span :class="task.todoItemIsDone ? 'text-green' : 'text-red'">{{ task.todoItemIsDone ? 'Zakończono' : 'W trakcie' }}</span>
            </v-list-item-subtitle>
            <v-list-item-subtitle class="pa-2">
              <b>Termin:</b> {{ task.todoItemDueDate ? new Date(task.todoItemDueDate).toLocaleDateString() : 'Brak' }}
            </v-list-item-subtitle>
          </v-list-item>

          <v-list-item v-if="!upcomingTasks.length && !loading">
            <v-list-item-title class="pa-2">Brak wkrótce kończących się zadań (poniżej 2 dni).</v-list-item-title>
          </v-list-item>
        </v-list>

          <v-text-field
            v-model="selectedDate"
            label="Pokaż zadania z terminem do"
            type="date">
          </v-text-field>

        <!-- ALL TASKS -->
        <v-list max-height="600">
          <v-list-item v-for="task in filterTasksByDate" :key="task.todoItemId">
            <!-- <v-list-item-title class="pa-2 font-weight-bold">
              ID: {{ task.todoItemId }}
            </v-list-item-title> -->

            <v-list-item-title class="d-flex text-wrap pa-2 ga-4 text-h5 my-4">
              <v-icon left v-if="task.todoItemIsDone" class="text-green">mdi-checkbox-marked-circle-outline</v-icon>
              <v-icon left v-else class="text-gray-darken-2">mdi-checkbox-blank-circle-outline</v-icon>
              <b>{{ task.todoItemTitle }}</b>
            </v-list-item-title>

            <div class="pa-2 text-medium-emphasis description-wrap">
              {{ task.todoItemDescription }}
            </div>

            <v-list-item-subtitle class="pa-2">
              <b>Status:</b> <span :class="task.todoItemIsDone ? 'text-green' : 'text-red'">{{ task.todoItemIsDone ? 'Zakończono' : 'W trakcie' }}</span>
            </v-list-item-subtitle>
            <v-list-item-subtitle class="pa-2">
              <b>Termin:</b> {{ task.todoItemDueDate ? new Date(task.todoItemDueDate).toLocaleDateString() : 'Brak' }}
            </v-list-item-subtitle>
            <template #append>
              <v-container class="d-flex flex-column">
                <v-btn :disabled="loading || editingTaskId == task.todoItemId" variant="text" color="blue" class="text-button" @click="openEditTaskMenu(task.todoItemId)">
                  EDYTUJ
                </v-btn>
                <v-btn :disabled="loading" variant="text" color="red" class="text-button" @click="deleteTask(task.todoItemId)">
                  USUŃ
                </v-btn>
              </v-container>
            </template>
          </v-list-item>

          <v-list-item v-if="!filterTasksByDate.length && !loading">
            <v-list-item-title class="pa-2">Brak zadań.</v-list-item-title>
          </v-list-item>
        </v-list>
      </v-container>

      <v-container max-width="700">
        <p v-if="error" class="text-error my-4 mx-2">Błąd: {{ error }}</p>
        <v-btn color="gray" @click="toggleTaskEditAdd(!isAddTaskFormVisible)" class="mb-4">
          {{ isAddTaskFormVisible ? 'Anuluj' : '+ Nowe zadanie' }}
        </v-btn>
        <v-form v-if="isAddTaskFormVisible" @submit.prevent="handleSubmit">
          <v-text-field
            v-model="newTitle"
            label="Tytuł zadania (max 30 znaków) *"
            required
          ></v-text-field>

          <v-textarea
            v-model="newDescription"
            label="Opis zadania (max 500 znaków)"
            rows="3"
            auto-grow
          ></v-textarea>

          <v-text-field
            v-model="newDueDate"
            label="Termin wykonania"
            type="date"
          ></v-text-field>

          <v-checkbox
            v-model="isTaskCompleted"
            label="Zadanie ukończone"
          ></v-checkbox>

          <v-btn v-if="!isEditingTask" type="submit" :color="loading ? 'gray' : 'green'">
            Dodaj zadanie
          </v-btn>
          <v-btn v-else type="submit" color="orange" @submit.prevent="editTask(newTitle, newDescription, newDueDate)">
            Zapisz zmiany
          </v-btn>
        </v-form>
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
.description-wrap {
  white-space: normal;
  overflow: visible;
  text-overflow: clip;
  word-wrap: break-word;
}

.close-x-btn {
  position: absolute !important;
  top: 25px;
  right: 25px;
  transform: translate(50%, -50%);
  background-color: rgba(0, 0, 0, 0);
  backdrop-filter: blur(5px);
}
</style>