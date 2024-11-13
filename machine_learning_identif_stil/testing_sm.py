import tensorflow as tf
from tensorflow.keras.preprocessing.image import ImageDataGenerator
from tensorflow.keras.models import load_model
from tensorflow.keras import backend as K
import numpy as np

# Paths to test data
test_dir = 'C:\\Users\\radit\\source\\repos\\Proiect\\machine_learning_identif_stil\\test_set'

# Image preprocessing (rescaling)
test_datagen = ImageDataGenerator(rescale=1.0/255.0)

# Create the test generator
test_generator = test_datagen.flow_from_directory(
    test_dir,
    target_size=(150, 150),  # Image size
    batch_size=1,
    class_mode='categorical',  # Set class_mode to 'categorical' to get labels
    shuffle=False
)

# Load the trained model
model = load_model('trained_model_final_sm.h5')
# model = load_model('trained_model_final_sm2.h5')


# Get the class labels from the training process
class_indices = {v: k for k, v in test_generator.class_indices.items()}

# Predict classes for each file in the test set
predictions = model.predict(test_generator, steps=test_generator.samples)

# Convert predictions to class labels
predicted_classes = np.argmax(predictions, axis=1)
predicted_labels = [class_indices[idx] for idx in predicted_classes]

# True labels
true_classes = test_generator.classes  # Ground truth labels
true_labels = [class_indices[idx] for idx in true_classes]

# Calculate accuracy
accuracy = np.mean(predicted_classes == true_classes)
print(f'Accuracy on test set: {accuracy * 100:.2f}%')

# Print individual results
for filename, pred_label, true_label in zip(test_generator.filenames, predicted_labels, true_labels):
    print(f'File: {filename} - Predicted label: {pred_label} - True label: {true_label}')

# Clear the session to free memory
K.clear_session()
