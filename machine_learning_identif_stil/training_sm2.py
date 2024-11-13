import os
import numpy as np
import tensorflow as tf
from tensorflow.keras import layers, models
from tensorflow.keras.preprocessing.image import ImageDataGenerator
from tensorflow.keras.callbacks import ReduceLROnPlateau
from tensorflow.keras import backend as K
from sklearn.utils import class_weight  # Import class_weight from sklearn

# Path to the training directory
train_dir = 'C:\\Users\\radit\\source\\repos\\Proiect\\machine_learning_identif_stil\\subset'

# Image preprocessing (resizing, normalization, etc.)
train_datagen = ImageDataGenerator(
    rescale=1.0/255.0,      # Normalization
    rotation_range=40,      # Random rotation
    width_shift_range=0.2,  # Random horizontal shift
    height_shift_range=0.2, # Random vertical shift
    shear_range=0.2,        # Shear transformation
    zoom_range=0.2,         # Random zoom
    horizontal_flip=True,   # Horizontal flip
    fill_mode='nearest',    # Pixel fill mode
    validation_split=0.2    # Use 20% of data for validation
)

# Create the training generator for the directory
train_generator = train_datagen.flow_from_directory(
    train_dir,
    target_size=(150, 150),  # Image size
    batch_size=32,
    class_mode='categorical', # Categorical labels
    subset='training'        # Use subset for training
)

# Create the validation generator for the directory
validation_generator = train_datagen.flow_from_directory(
    train_dir,
    target_size=(150, 150),  # Image size
    batch_size=32,
    class_mode='categorical', # Categorical labels
    subset='validation'      # Use subset for validation
)

# Calculate class weights to handle class imbalance
class_weights = class_weight.compute_class_weight(
    'balanced',
    classes=np.unique(train_generator.classes),
    y=train_generator.classes
)

# Initialize the model
total_classes = len(train_generator.class_indices)  # Get the total number of classes
model = models.Sequential([ 
    layers.Conv2D(32, (3, 3), activation='relu', input_shape=(150, 150, 3)),
    layers.MaxPooling2D((2, 2)),
    layers.Flatten(),
    layers.Dense(64, activation='relu'),
    layers.Dense(total_classes, activation='softmax')  # Define output layer based on the number of classes
])

model.compile(optimizer='adam',
              loss='categorical_crossentropy',
              metrics=['accuracy'])

# Directory for saving models after each class
checkpoint_dir = './checkpoints'
os.makedirs(checkpoint_dir, exist_ok=True)

# Callback: Learning rate scheduler
lr_scheduler = ReduceLROnPlateau(monitor='val_loss', patience=3, factor=0.5, min_lr=1e-6)

# Train the model with class weights
history = model.fit(
    train_generator,
    steps_per_epoch=train_generator.samples // train_generator.batch_size,
    validation_data=validation_generator,
    validation_steps=validation_generator.samples // validation_generator.batch_size,
    epochs=30,  # Max number of epochs
    class_weight=dict(enumerate(class_weights)),  # Apply class weights
    callbacks=[lr_scheduler]
)

# Save the final trained model
model.save('trained_model_final_sm2.h5')

# Clear the session to free memory
K.clear_session()
