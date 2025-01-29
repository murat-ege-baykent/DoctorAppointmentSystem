document.addEventListener('DOMContentLoaded', () => {
    const appointmentsList = document.getElementById('appointments-list');
    const appointmentDetail = document.getElementById('appointment-detail');
    const appointmentForm = document.getElementById('appointment-form');
  
    // Fetch appointments from the backend
    fetch('https://doctorappointmentservice-a3e5g7caccg6fwbt.germanywestcentral-01.azurewebsites.net/api/appointments') // Replace with your API URL
      .then(response => response.json())
      .then(appointments => {
        appointments.forEach(appointment => {
          const listItem = document.createElement('li');
          listItem.innerHTML = `
            <strong>Doctor: ${appointment.doctorName}</strong><br>
            Appointment Date: ${appointment.appointmentDate}<br>
            <button onclick="bookAppointment('${appointment.id}')">Book Appointment</button>
          `;
          appointmentsList.appendChild(listItem);
        });
      })
      .catch(error => console.error('Error fetching appointments:', error));
  
    // Function to handle booking appointment
    window.bookAppointment = (appointmentId) => {
      // Hide the appointments list and show the booking form
      document.getElementById('appointments').style.display = 'none';
      appointmentDetail.style.display = 'block';
  
      // Handle form submission
      appointmentForm.addEventListener('submit', (event) => {
        event.preventDefault();
  
        const patientName = document.getElementById('patientName').value;
        const comment = document.getElementById('comment').value;
  
        // Post comment to backend
        const commentData = {
          appointmentId: appointmentId,
          patientName: patientName,
          comment: comment
        };
  
        fetch('https://doctorappointmentservice-a3e5g7caccg6fwbt.germanywestcentral-01.azurewebsites.net/api/comments', { // Replace with your API URL
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(commentData),
        })
          .then(response => response.json())
          .then(data => {
            alert('Thank you for your feedback!');
            window.location.reload(); // Reload the page to see the updated appointments
          })
          .catch(error => console.error('Error posting comment:', error));
      });
    };
  });
  