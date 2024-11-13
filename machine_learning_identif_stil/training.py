import os
import numpy as np
import tensorflow as tf
from tensorflow.keras import layers, models
from tensorflow.keras.preprocessing.image import ImageDataGenerator
from tensorflow.keras.callbacks import EarlyStopping, ReduceLROnPlateau
from tensorflow.keras import backend as K

# Path to the training directory
train_dir = 'C:\\Users\\radit\\source\\repos\\Proiect\\machine_learning_identif_stil\\archive'

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

# Callbacks: Early stopping and learning rate scheduler
early_stopping = EarlyStopping(monitor='val_accuracy', patience=5, restore_best_weights=True)
lr_scheduler = ReduceLROnPlateau(monitor='val_loss', patience=3, factor=0.5, min_lr=1e-6)

# Train the model
history = model.fit(
    train_generator,
    steps_per_epoch=train_generator.samples // train_generator.batch_size,
    validation_data=validation_generator,
    validation_steps=validation_generator.samples // validation_generator.batch_size,
    epochs=30,  # Max number of epochs before early stopping kicks in
    callbacks=[early_stopping, lr_scheduler]
)

# Save the final trained model
model.save('trained_model_final.h5')

# Clear the session to free memory
K.clear_session()
