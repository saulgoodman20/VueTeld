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
    <v-main class="d-flex justify-center align-center">
      <v-container>
          <v-text-field
            v-model="selectedDate"
            label="Pokaż zadania z terminem do"
            type="date"
            class="mb-4">
          </v-text-field>

        <v-list>
          <v-list-item v-for="task in filterTasksByDate" :key="task.todoItemId" class="my-8">
            <!-- <v-list-item-title class="pa-2 font-weight-bold">
              ID: {{ task.todoItemId }}
            </v-list-item-title> -->

            <v-list-item-title class="pa-2 text-h5">
              <v-icon left v-if="task.todoItemIsDone">mdi-checkbox-marked-circle-outline</v-icon>
              <b>{{ task.todoItemTitle }}</b>
            </v-list-item-title>

            <div class="pa-2 text-medium-emphasis" style="white-space: pre-wrap; line-height: 1.4;">
              {{ task.todoItemDescription }}
            </div>

            <v-list-item-subtitle class="pa-2">
              <b>Status:</b> {{ task.todoItemIsDone ? 'Zakończono' : 'W trakcie' }}
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
            <v-list-item-title class="pa-2">No tasks found.</v-list-item-title>
          </v-list-item>
        </v-list>
      </v-container>

      <v-container>
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
            label="Opis zadania"
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