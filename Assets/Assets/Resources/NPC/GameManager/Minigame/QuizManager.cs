﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    [RequireComponent(typeof(ConservationManager))]
    public class QuizManager : MonoBehaviour
    {
        GameObject targetNPC;
        QuizConservationSO quizData;
        ConservationManager conservationManager;
        List<QuestionAndAnswer> temporaryQnas;
        QuestionAndAnswer curQNA;
        int currentQuestion = 0;
        int correctAnswers = 0;
        int wrongAnswers = 0;

        private void Awake()
        {
            conservationManager = GetComponent<ConservationManager>();
        }

        public void InitAndStartQuizData(GameObject targetNPC, QuizConservationSO quiz)
        {
            ResetQuizMission();
            quizData = quiz;
            this.targetNPC = targetNPC;
            temporaryQnas = new List<QuestionAndAnswer>(quizData.qnas);
            conservationManager.ChangeTargetNPC(gameObject);

            //intro dialog
            DialogConservation intro = quizData.quizIntro;
            intro.possibleResponses[0].executedFunction = DialogExecuteFunction.NextQuiz;
            StartCoroutine(conservationManager.UpdateConservation(intro));
        }

        public void ResetQuizMission()
        {
            currentQuestion = 0;
            correctAnswers = 0;
            wrongAnswers = 0;
        }

        public void NextQuiz()
        {
            if(currentQuestion >= quizData.numberOfQuestion || wrongAnswers > quizData.numberOfMaxWrong)
            {
                FinishQuiz();
                return;
            }
            
            //init next question
            curQNA = GetRandomQuestion();
            DialogConservation quiz = new DialogConservation();
            quiz.message = curQNA.Question;

            for (int i = 0; i < curQNA.Answers.Length; i++)
            {
                DialogResponse response = new DialogResponse();
                response.message = curQNA.Answers[i];
                response.executedFunction = (curQNA.CorrectAnswer - 1 == i) ? DialogExecuteFunction.AnswerCorrect : DialogExecuteFunction.AnswerWrong;
                quiz.possibleResponses.Add(response);
            }
            StartCoroutine(conservationManager.UpdateConservation(quiz));
            currentQuestion++;
        }

        public void FinishQuiz()
        {
            //change target NPC to original NPC
            conservationManager.ChangeTargetNPC(targetNPC);
            
            //create dialog
            DialogConservation correctDialog = new DialogConservation();
            DialogResponse response = new DialogResponse();
            
            if (wrongAnswers > quizData.numberOfMaxWrong)
            {
                correctDialog.message = "Tiếc quá, em đã trả lời <color=#FF6100>sai quá "+ quizData.numberOfMaxWrong +" câu</color> rồi. Thầy nghĩ là em cần thêm thời gian để tìm hiểu về trường. Hãy quay lại đây <color=#FF6100>sau 5 phút</color> nữa nhé.";
                response.executedFunction = DialogExecuteFunction.OnQuestMinigameFail;
            } else
            {
                correctDialog.message = "Thật tuyệt vời, em đã trả lời đúng <color=#06FFE6>" + correctAnswers + "/" + 8 + " câu hỏi</color> của thầy. Thầy tin là sau cuộc trò chuyện này em đã có thêm nhiều hiểu biết về trường mình.";
                response.executedFunction = DialogExecuteFunction.OnQuestMinigameSuccess;
            }
            
            response.message = "Vâng ạ";
            correctDialog.possibleResponses.Add(response);
            //
            StartCoroutine(conservationManager.UpdateConservation(correctDialog));
        }

        public void AnswerCorrect()
        {
            correctAnswers++;
            DialogConservation correctDialog = new DialogConservation();

            correctDialog.message = "Đáp án <color=#06FFE6>chính xác</color>. " + curQNA.Explaination; 
            if(currentQuestion < quizData.numberOfQuestion) 
            {
                correctDialog.message += " Sau đây là câu hỏi tiếp theo.";
            }
            
            DialogResponse response = new DialogResponse();
            response.message = "Vâng ạ";
            response.executedFunction = DialogExecuteFunction.NextQuiz;
            correctDialog.possibleResponses.Add(response);
            //
            StartCoroutine(conservationManager.UpdateConservation(correctDialog));
        }

        public void AnswerWrong()
        {
            wrongAnswers++;
            bool isFail = (wrongAnswers > quizData.numberOfMaxWrong);
            if (isFail)
            {
                FinishQuiz();
                return;
            }

            bool isAboutToFail = (wrongAnswers > quizData.numberOfMaxWrong - 1);

            DialogConservation wrongDialog = new DialogConservation();
            wrongDialog.message = "Đáp án <color=#FF6100>không chính xác</color>. Em đã trả lời <color=#FF6100>sai "
                + wrongAnswers + " câu</color> rồi." 
                + ((isAboutToFail) ? " Em chỉ <color=#FF6100>còn 1 cơ hội</color> nữa thôi. Hãy thật cẩn thận nhé." : " Hãy thật bình tĩnh trong câu hỏi tiếp theo nhé.");
            
            DialogResponse response = new DialogResponse();
            response.message = "Vâng ạ";
            response.executedFunction = DialogExecuteFunction.NextQuiz;
            wrongDialog.possibleResponses.Add(response);
            //
            StartCoroutine(conservationManager.UpdateConservation(wrongDialog));
            
        }

        QuestionAndAnswer GetRandomQuestion()
        {
            int random = Random.Range(0, temporaryQnas.Count);
            QuestionAndAnswer qna = temporaryQnas[random];
            temporaryQnas.RemoveAt(random);
            return qna;
        }
    }
}

