from pynput import keyboard
import time
import _thread

def on_press(key):
    raise SystemExit


def startListener():
    # Collect events until released
    with keyboard.Listener(
        on_press=on_press) as listener:
        listener.join()

startListener()