import os
from telegram import Update
from telegram.ext import Updater, CommandHandler, MessageHandler, CallbackContext

def start(update: Update, context: CallbackContext) -> None:
    update.message.reply_text('start')

def send_image(update: Update, context: CallbackContext) -> None:
    # Получаем путь к папке с изображениями (замените "путь/к/папке" на ваш путь)
    image_folder_path = "../ObjectDetected/ObjectDetected/destruction"
    # Получаем список файлов в папке
    image_files = [f for f in os.listdir(image_folder_path) if os.path.isfile(os.path.join(image_folder_path, f))]
    update.message.reply_text('В папке {} изображений'.format(len(image_files)))
    # Выбираем первое изображение из списка (вы можете выбрать любой другой способ выбора)
    if image_files:
        for image in image_files:
            image_path = os.path.join(image_folder_path, image)
            update.message.reply_photo(open(image_path, 'rb'))
    else:
        update.message.reply_text('В папке нет изображений!')

def main() -> None:
    updater = Updater("6910916880:AAGR2qpE83PAZmMgbDHUd0S7FKKIggg2XtM", use_context=True)
    dp = updater.dispatcher

    dp.add_handler(CommandHandler("start", start))
    dp.add_handler(CommandHandler("send_destruction", send_image))  # Добавляем команду для отправки изображения

    updater.start_polling()
    updater.idle()

if __name__ == '__main__':
    main()
