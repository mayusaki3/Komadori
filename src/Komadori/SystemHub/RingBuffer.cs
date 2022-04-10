using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XControls
{
    /// <summary>
    /// ���L��������Ƀ����O�o�b�t�@���쐬���邽�߂̃N���X�ł��B
    /// </summary>
    public partial class RingBuffer
    {
        #region �\�z�E�j��

        /// <summary>
        /// XControls.XRingBuffer �N���X�̐V�����C���X�^���X�����������܂��B
        /// </summary>
        /// <param name="xsmem">�����O�o�b�t�@�i�[�p�� XSharedMemory �N���X�C���X�^���X</param>
        public RingBuffer(SharedMemory xsmem)
        {
            this.xsmem = xsmem;
        }

        #endregion

        #region �ϐ�

        /// <summary>
        /// ���L�������ł���肷�郊���O�o�b�t�@�f�[�^�ł��B
        /// </summary>
        private BufferDataBag databuf = new BufferDataBag();

        /// <summary>
        /// �����O�o�b�t�@�i�[�p�� XSharedMemory �N���X�C���X�^���X�ł��B
        /// </summary>
        private SharedMemory xsmem = null;

        #endregion

        #region �v���p�e�B

        #region IsShutDown�v���p�e�B

        /// <summary>
        /// �V���b�g�_�E�������ǂ������擾�܂��͐ݒ肵�܂��Btrue �ɂ���ƃo�b�t�@�������݂𖳎����܂��B
        /// </summary>
        public bool IsShutDown
        {
            set
            {
                LoadSMem();
                databuf.IsShutDown = value;
                SaveSMem();
            }
            get
            {
                LoadSMem();
                return databuf.IsShutDown;
            }
        }

        #endregion

        #region IsUpdate�v���p�e�B

        /// <summary>
        /// �����O�o�b�t�@�̓��e���X�V���ꂽ���ǂ������擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public bool IsUpdate
        {
            set
            {
                LoadSMem();
                databuf.IsUpdate = value;
                SaveSMem();
            }
            get
            {
                LoadSMem();
                return databuf.IsUpdate;
            }
        }

        #endregion

        #endregion

        #region ���\�b�h

        #region �I�u�W�F�N�g�i�[ (SetObject)

        /// <summary>
        /// �I�u�W�F�N�g���o�b�t�@�Ɋi�[���܂��B
        /// </summary>
        /// <param name="obj">�i�[����I�u�W�F�N�g</param>
        /// <returns>����(true=����, false=�o�b�t�@�t��)</returns>
        public bool SetObject(object obj)
        {
            bool rt = false;

            LoadSMem();
            rt = databuf.SetObject(obj);
            SaveSMem();

            return rt;
        }

        #endregion

        #region �I�u�W�F�N�g���o�� (GetObject)

        /// <summary>
        /// �o�b�t�@����I�u�W�F�N�g�����o���܂��B
        /// </summary>
        /// <returns>�I�u�W�F�N�g(null=�I�u�W�F�N�g�Ȃ�)</returns>
        public object GetObject()
        {
            UInt16 rptr;
            return GetObject(out rptr);
        }

        /// <summary>
        /// �o�b�t�@����I�u�W�F�N�g�����o���܂��B
        /// </summary>
        /// <param name="rptr">�V�[�P���X</param>
        /// <returns>�I�u�W�F�N�g(null=�I�u�W�F�N�g�Ȃ�)</returns>
        public object GetObject(out UInt16 rptr)
        {
            object obj = null;

            LoadSMem();
            obj = databuf.GetObject(out rptr);

            return obj;
        }

        #endregion

        #region ���e���e�L�X�g�Ŏ擾 (ToString)

        /// <summary>
        /// ���e���e�L�X�g�ŕԂ��܂��B
        /// </summary>
        /// <returns>���e</returns>
        public override string ToString()
        {
            LoadSMem();
            return databuf.ToString();
        }

        #endregion

        #endregion

        #region ��������

        #region �I�u�W�F�N�g�ǂݍ��� (LoadSMem)

        /// <summary>
        /// �I�u�W�F�N�g�����L����������ǂݍ��݂܂��B
        /// </summary>
        public void LoadSMem()
        {
            try
            {
                databuf = xsmem.GetObject(0) as BufferDataBag;
            }
            catch
            {
            }
        }

        #endregion

        #region �I�u�W�F�N�g�������� (SaveSMem)

        /// <summary>
        /// �I�u�W�F�N�g�����L�������ɏ����o���܂��B
        /// </summary>
        public void SaveSMem()
        {
            try
            {
                xsmem.PutObject(databuf, 0);
            }
            catch
            {
            }
        }

        #endregion

        #endregion
    }
}
