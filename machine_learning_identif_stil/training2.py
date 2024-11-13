import os
import numpy as np
import tensorflow as tf
from tensorflow.keras import layers, models
from tensorflow.keras.preprocessing.image import ImageDataGenerator
from tensorflow.keras.callbacks import EarlyStopping, ReduceLROnPlateau
from tensorflow.keras import backend as K
from sklearn.utils import class_weight

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

# Calculate class weights to handle class imbalance
class_weights = class_weight.compute_class_weight(
    'balanced',
    classes=np.unique(train_generator.classes),
    y=train_generator.classes
)

# Initialize the model with transfer learning (ResNet50 pre-trained)
base_model = tf.keras.applications.ResNet50(weights='imagenet', include_top=False, input_shape=(150, 150, 3))
base_model.trainable = False  # Freeze the base model

# Add custom layers on top of the base model
model = models.Sequential([
    base_model,
    layers.GlobalAveragePooling2D(),
    layers.Dropout(0.5),  # Add dropout layer for regularization
    layers.Dense(64, activation='relu'),
    layers.Dense(len(train_generator.class_indices), activation='softmax')  # Output layer
])

# Compile the model
model.compile(optimizer='adam',
              loss='categorical_crossentropy',
              metrics=['accuracy'])

# Directory for saving models after each class
checkpoint_dir = './checkpoints'
os.makedirs(checkpoint_dir, exist_ok=True)

# Callbacks: Early stopping and learning rate scheduler
early_stopping = EarlyStopping(monitor='val_accuracy', patience=5, restore_best_weights=True)
lr_scheduler = ReduceLROnPlateau(monitor='val_loss', patience=3, factor=0.5, min_lr=1e-6)

# Train the model with class weights
history = model.fit(
    train_generator,
    steps_per_epoch=train_generator.samples // train_generator.batch_size,
    validation_data=validation_generator,
    validation_steps=validation_generator.samples // validation_generator.batch_size,
    epochs=30,  # Max number of epochs before early stopping kicks in
    class_weight=dict(enumerate(class_weights)),  # Apply class weights
    callbacks=[early_stopping, lr_scheduler]
)

# Save the final trained model
model.save('trained_model_final2.h5')

# Clear the session to free memory
K.clear_session()
