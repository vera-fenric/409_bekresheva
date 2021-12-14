﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Prediction;
using YOLOv4MLNet.DataStructures;
using System.Collections.Immutable;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private void CanChangeIFCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (prediction != null)
                e.CanExecute = !prediction.InProgress;
            else e.CanExecute = false;
        }
        private void CanChangeMPCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (prediction != null)
                e.CanExecute = !prediction.InProgress;
            else e.CanExecute = false;
        }
        private void CanStartCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (prediction != null)
                e.CanExecute = !prediction.InProgress;
            else e.CanExecute = false;
        }
        private void CanStopCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (prediction != null)
                e.CanExecute = prediction.InProgress;
            else e.CanExecute = false;
        }

        async private void ChangeIFCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
            //ViewModel.ChangeIF();
        }
        async private void ChangeMPCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg_open = new Microsoft.Win32.OpenFileDialog();
            dlg_open.FileName = "model";
            dlg_open.DefaultExt = ".onnx";
            dlg_open.Filter = "ONNX documents|*.onnx";
            if (dlg_open.ShowDialog() == true)
                model_path = dlg_open.FileName;
            else
                throw new Exception("Fatal Error!");
            ViewModel.Progress = "Идёт загрузка модели, пожалуйста, подождите!";
            prediction = new PredictionClass(model_path, ViewModel);
            prediction.Init();
            ViewModel.Progress = "Готово к работе!";
        }
        async private void StartCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

            try
            {
                await prediction.StartPredictionAsync();
                prediction.InProgress = false;
                ViewModel.Progress = "Обработка завершена";
            }
            catch (Exception ex)
            {
                ViewModel.Results = ImmutableList<YoloV4Result>.Empty;
                MessageBox.Show("Ошибка предсказания или предсказание было завершено пользователем");
            }
        }

        private void StopCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                prediction.StopPrediction();
                prediction.InProgress = false;
            }
            catch
            {
                MessageBox.Show("Ошибка остановки предсказаний");
            }
        }
        private void ClassesViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClassesView.SelectedItem == null)
            {
                ImagesResultListView.ItemsSource = null;
                return;
            }
            lock (ViewModel.ImagesOfClasses)
            {
                string className = ((KeyValuePair<string, int>)ClassesView.SelectedItem).Key;
                ImagesResultListView.ItemsSource = ViewModel.ImagesOfClasses[className];
            }
        }
    }
}