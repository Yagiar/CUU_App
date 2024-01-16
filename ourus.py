import cv2
import os
import torch
import numpy as np
import matplotlib.pyplot as plt
import time
import uuid
5
model = torch.hub.load('ultralytics/yolov5', 'custom', path='C:/Users/shabu/Desktop/best.pt', force_reload=True)
objecties = ['No helmet','No vest','Person','helmet','vest']

fig, ax = plt.subplots()

def non_max_suppression(boxes, scores, threshold=0.5):
    if len(boxes) == 0:
        return []

    boxes = np.array(boxes)
    x1 = boxes[:, 0]
    y1 = boxes[:, 1]
    x2 = x1 + boxes[:, 2]
    y2 = y1 + boxes[:, 3]

    areas = boxes[:, 2] * boxes[:, 3]
    scores = np.array(scores)
    order = scores.argsort()[::-1]

    picked_indices = []

    while order.size > 0:
        i = order[0]
        picked_indices.append(i)

        xx1 = np.maximum(x1[i], x1[order[1:]])
        yy1 = np.maximum(y1[i], y1[order[1:]])
        xx2 = np.minimum(x2[i], x2[order[1:]])
        yy2 = np.minimum(y2[i], y2[order[1:]])
        w = np.maximum(0, xx2 - xx1 + 1)
        h = np.maximum(0, yy2 - yy1 + 1)
        overlap = (w * h) / (areas[i] + areas[order[1:]] - (w * h))

        inds = np.where(overlap <= threshold)[0]
        order = order[inds + 1]

    picked_boxes = boxes[picked_indices]
    return picked_boxes.tolist()

file_path = 'C:/Users/shabu/Desktop/33.avi'
cap = cv2.VideoCapture(file_path)
target_time = 53
cap.set(cv2.CAP_PROP_POS_FRAMES, int(target_time * cap.get(cv2.CAP_PROP_FPS)))

fr = 0
boxes = []
scores = []
selected_region = None

# Dictionary to store unique objects and their counts


while cap.isOpened():
    time.sleep(0.3)
    unique_objects = {}
    ret, frame = cap.read()
    if not ret:
        break
    boxes.clear()
    scores.clear()
    scale_factor = min(1.0, 1080.0 / max(frame.shape[0], frame.shape[1]))
    resized_frame = cv2.resize(frame, None, fx=scale_factor, fy=scale_factor)
    if selected_region is not None:
        x, y, w, h = selected_region
        cv2.rectangle(resized_frame, (x, y), (x + w, y + h), (255, 0, 0), 2)

        # Perform object detection within the selected ROI
        roi_frame = resized_frame[y:y + h, x:x + w]

        predictions = model(roi_frame)

        for pred in predictions.xyxy[0]:
            x, y, w, h,k = int(pred[0]), int(pred[1]), int(pred[2]), int(pred[3]),pred[5]
            box = [x, y, w, h,k]
            boxes.append(box)
            scores.append(pred[4])

        boxes = non_max_suppression(boxes, scores)

        for box in boxes:
            x, y, w, h,k= map(int, box)
            
            cv2.rectangle(roi_frame, (x, y), (w, h), (70, 155, 125), 2)
            if k==0 or k==1:
                file_name = str(uuid.uuid4()) + '.jpg'
                cv2.imwrite(file_name, roi_frame)

            # Update the unique_objects dictionary
            obj_key = k
            if obj_key in unique_objects:
                unique_objects[obj_key] += 1
            else:
                unique_objects[obj_key] = 1

        cv2.imshow("Video", roi_frame)

    else:
        cv2.imshow("Video", resized_frame)

    key = cv2.waitKey(1)

    if key == ord('q'):
        break
    elif key == ord('s'):
        selected_region = cv2.selectROI("Select Region", resized_frame, fromCenter=False, showCrosshair=True)
        cv2.destroyWindow("Select Region")

    # Print the unique objects and their counts
    print("Unique Objects and Counts:")
    for obj, count in unique_objects.items():
        print(f"Object {objecties[obj]}: Count {count}")

cv2.destroyAllWindows()
cap.release()
