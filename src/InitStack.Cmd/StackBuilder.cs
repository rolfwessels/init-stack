using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using InitStack.Cmd.Sources;

namespace InitStack.Cmd;

public class StackBuilder
{
  private readonly string _solutionName;
  private readonly string _newFolder;
  private readonly string _fromPath;
  private readonly string _newSolutionName;
  private readonly ReplaceItAll _replaceIt;
  private readonly GitHistory _gitHistory;

  public StackBuilder(StackSourceFile source, string outputFolder, string newSolutionName)
  {
    _newFolder = outputFolder;
    _solutionName = source.Name;
    _newSolutionName = newSolutionName;
    _fromPath = source.Folder;
    _replaceIt = new ReplaceItAll(_solutionName, _newSolutionName);
    _gitHistory = new GitHistory();
  }

  public string OutputFolder => ReplaceFileInfo(_fromPath);

  public string[] Build()
  {
    var mainPath = new DirectoryInfo(_fromPath);
    return CopyFolder(mainPath).ToArray();
  }


  private IEnumerable<string> CopyFolder(DirectoryInfo fromDir)
  {
    foreach (var p in fromDir.GetDirectories())
    {
      if (IsIgnored(p.FullName)) continue;
      var toDir = new DirectoryInfo(ReplaceFileInfo(p.FullName));
      if (!toDir.Exists) toDir.Create();
      foreach (var s in CopyFolder(p)) yield return s;
    }

    foreach (var fromFile in fromDir.GetFiles())
    {
      if (IsIgnored(fromFile.FullName)) continue;
      var toFile = new FileInfo(ReplaceFileInfo(fromFile.FullName));
      if (toFile.Name.EndsWith("logo.png"))
        DownloadRandomImage(ReplaceFileContent(_solutionName), toFile.FullName).Wait();
      else
        CopyFile(fromFile, toFile);
      yield return toFile.FullName;
    }
  }


  private async Task DownloadRandomImage(string name, string toFile)
  {
    var guids =
      "d08cc2c6-800a-4e5c-9e35-3fdeb6edf333|aeeebcb7-5600-4d7e-9019-6526370160d7|cad0a1d6-32fd-4671-93d0-fe5bbace9db7|37783696-f0b2-4cdb-9bbb-99acb449a710|d8e9a873-9f23-46a8-ba8a-7078ad9ba8b8|b5d4696d-fae6-43f7-946b-a3b0c28eebf0|5c4a326f-85fe-40c7-b6bf-d8b8e39273fe|1ac5da90-f236-4676-b862-d4122fb8e981|6d08fa20-5055-4b16-839b-03cc828ebbeb|015c6a1f-66c0-4d93-b6f9-cbb700e17877|476f4549-6085-49a9-9cdd-6063767bf86d|d945197a-3635-4d83-b567-3a13f29dc197|19321e9a-ea16-4c9c-8bf3-6448e1316793|e424c725-6ac9-4e20-b8b5-19ae50f8875c|70feb60a-d2b3-4a67-9804-147c4f861b53|6e8d34e2-ba56-4344-89bd-da941c517ad2|a566e8b8-e2ab-437e-ac6c-ab7d72c503a4|57588c72-22f5-4c06-ba4b-4c11398c4fb3|6ba3467d-8f26-4790-bb8d-7d5e13b94a4d|ce260f92-c4ac-4c7d-809c-740ad7045f81|0c59848f-27dd-4ac4-8441-ca277e0c8d83|3fa4296a-cae6-4c2c-bab9-8776eafaf95f|026ca15e-75dc-43d0-bece-874312ba83f4|136e8aa0-fcfc-49b4-9dea-439bdcb8dfc5|a02349f0-8bb9-4779-a225-e0b644913509|80e71bd7-44e0-42ff-9aa1-8c476e02a8a2|2403d90c-a635-45f7-8aeb-993adeab87a8|fcd9dbd4-cde9-4ca8-b1fd-b203c271e2fc|1f852516-a5ca-45a4-9868-58de74b485b3|57953eef-789b-4613-81d1-06532bc6ea3e|7f326be2-b31b-4928-b0aa-39347181ab6a|be288a7c-3be5-40e6-ae11-d82f392ecbd0|becd902e-5ea1-4da7-b707-ad84dba68d68|3f25186c-ac3a-4d93-af6b-123a813d1b0c|568cd543-eb78-4321-8693-662ffe1ece7c|311409fc-f9c7-4714-aeb6-cd623cb65544|a03d458e-9f86-4784-adf4-2ff729da632b|a01b96d2-0c26-45b1-ab7e-e11fbb100f50|e6ff540e-cfe3-4506-bd8e-1531a3272a54|be87b516-4f4a-483d-be7e-a0695b4a9e78|83485893-1246-4fb2-a778-e1da5a9ba27c|7920b912-290a-4a1e-b064-0a607e229270|f15b519a-091b-4c11-b067-ae872c3495f2|0c7cb86e-7fbf-41a5-b631-94bf29a51179|14b53644-5bc5-4f0a-8c20-b397379e54d5|6704bb4d-1082-49d0-b75b-e07a0484c4c1|f8c0c3c5-855d-4b7e-a1c5-86ec2081b69a|b069bfb7-8287-41de-b9a3-2262c922ddac|e4df143f-88f7-4b95-beaf-23ed32164909|cd21b512-bcb4-46c2-9a9b-ac9d34a3f6b6|643c710a-d2a9-48d2-91ae-d7e0bf1a2c14|765c0ed7-a7fe-45d8-bf51-323586c9a676|254ac076-ec56-469c-95ca-5c814d97d10b|f6472ef7-297e-4bc3-b23b-8313eb49a6ee|690408a8-79da-42de-8162-7587c91d8a34|a87cf65d-c589-44f1-b4cd-169b9b4a887f|2e6e6ea2-8f82-490a-949a-cd6163eeec0b|1650b152-5e69-4244-8915-5709fcd5eb8e|ff1a5b97-4e9d-4351-bae6-a97af879bdc3|445f320c-9b68-46be-ad25-a7e6bdb25854|bdc04577-02c7-45ec-b98d-b337bb826082|c87784e9-5f1a-4c9c-bf10-98da289b7aa9|f8b9c38d-8ed0-4c3f-9447-d79439ac53db|0105c67c-8c57-4c43-bd4d-9fd55fdf5fe9|a88f377c-a932-488e-9876-b43a38636f6e|bf3318f2-6c66-4288-ac89-e4dc08a6e9ab|7a1f1702-ad44-49b6-9166-c4e7e8191761|4a44fdfc-fd24-40f2-8195-bb7af5f7f641|20372dd2-f449-4171-952e-00b664f4ce1e|32f31202-ffcf-4ed9-a2ba-93fbeb94daa1|f1ba8e70-8d49-4880-8256-e887ca1a10df|84f2ed5e-afbd-4be9-befe-d19e3eb995bf|9005add8-38ab-480c-8f0b-2540548dac94|a6a4e7ab-a434-454f-a3ac-335c7cb97a13|b2ca9d42-0467-4062-9ed7-e97a530db448|c3afe61c-b16c-4e73-9cf6-1ef649b900fd|49b31330-88ad-4d4e-b568-083e6d895393|ebb80c48-a3f2-4240-9bbb-3f5dbce6524c|efb4c64d-b81b-4407-ae05-198b9f3e206a|dff8de5a-c74e-4d53-9d0b-ba9237d6fe1b|00a7f569-8c1c-4795-8182-04521658a3b8|7684c993-ab8e-4729-99b3-5974f6651163|b3f54863-5776-4e17-ba19-880f24e796d8|5ed9a098-b851-43e2-ae62-80c1647e512a|99e0a0e4-3ae1-4686-8b14-12ab028afddc|f4e43bcc-a2a8-4831-93fe-13b37d1fe16a|67b90e64-f9e2-4365-806c-ea8aeb8b8954|9c3b2e29-6781-49dd-ae91-395c2a23e754|fe4e7c72-5462-4a6b-9289-f85acab0661f|1bb415e8-6ff7-4469-97f8-52dcf2716dc4|b241776f-9112-44ff-868d-f28d51e21e49|9e94f8b1-9b26-4c50-a928-9bc513a84de7|bdf99456-978d-4124-bbfd-657350b54663|1ea7608e-b659-4df1-b564-08e3d294281b|19e87cd2-6b46-4771-9567-a91d24a14f76|16070d71-fe34-4a33-974b-392bc4bb38fb|086a1435-eb08-42e1-b1b6-b51fcb6b07de|38f429fb-cab8-4d1e-ae17-0bddffc6cd20|649d543e-7587-46ce-8eb5-850e7b65465b|136facc4-6d79-43e8-b6d1-99ce3501b3c3|d2aec8df-e66f-4db8-9cb8-1965deb038fd|8629a778-26c1-43a7-bf0d-d653c03cb15a|d175390f-1925-4705-b9df-99950e8e94d2|9daa5191-4a56-496b-b353-12617452170d|e8efc65a-bc6a-4f4d-9d08-6867fa07763d|05d5dddc-cba7-4ec4-9ec4-fff132e3c677|8d03f9cb-3f68-47f7-8b51-e6cacced4427|d7d2427f-8548-4964-a6eb-f068bbc700de|7a2dca3f-e337-47fc-aba0-469c4fabeb63|51f1220b-4fcb-4a23-bccf-51ead1f1e3df|87a1eb51-579d-461f-a837-8095655a4a0b|71e6fab3-9a34-4ba1-9e46-fdae4b0e5a08|f428a294-c0c5-449e-a940-3c1f2cc5a775|b5501c6a-e47e-4006-981d-88fa76bd6426|fa5caf7d-a5b5-453c-99fb-47ac71acef67|3326fb2c-cb42-487f-a30d-a85536294f84|e544e720-ea63-42a9-b67a-ac1f7ea0e367|43e442b9-4fb7-4f28-90ce-059c3e622c0b|da37f465-a60e-4a51-b7a5-dab52b8f12ef|169888b0-791c-406a-bb3c-597bb8d13a5e|6e2def72-3986-4789-a866-17b6cb8210c9|722fd0fc-bb13-4996-a28c-602e0d65ea53|19adf525-3ac0-4c50-9f22-6395ae396638|f6be5697-b22f-442e-8580-3c6b0df87e75|e03af077-6b45-4ba7-bca1-88cfb48ae4c6|3368d2e6-5df1-45c4-b974-797db879ebb2|cb5ebb05-253e-47f8-9569-095bf5958369|a152701c-bfb8-4166-81c1-d5b054778c9a"
        .Split('|').ToArray();
    var key = guids[new Random().Next(0, guids.Length)];
    var url = "https://dynamic.brandcrowd.com/asset/logo/{key}/logo-search-grid-1x?&text={text}".Replace("{key}", key)
      .Replace("{text}", HttpUtility.UrlEncode(name));
    var client = new HttpClient();
    var response = await client.GetAsync(url);
    await File.WriteAllBytesAsync(toFile, await response.Content.ReadAsByteArrayAsync());
  }

  private void CopyFile(FileInfo fromFile, FileInfo toFile)
  {
    //straight bin copy
    if (!IsContentReplaceAble(fromFile.Name))
    {
      if (toFile.Exists)
      {
        Console.WriteLine($"Skipping {toFile.FullName}");
      }
      else
      {
        if (toFile.Exists) toFile.Delete();
        using (var newFile = toFile.Create())
        {
          using (var stream = fromFile.OpenRead())
          {
            stream.CopyTo(newFile);
            newFile.Flush();
          }
        }
      }
    }
    else
    {
      using (var newFile = toFile.Create())
      {
        using (var stream = fromFile.OpenRead())
        {
          var sr = new StreamReader(stream);
          var sw = new StreamWriter(newFile);
          var inputText = sr.ReadToEnd();
          inputText = _replaceIt.ReplaceIt(inputText);
          sw.Write(inputText);
          sw.Flush();
        }
      }
    }
  }

  string ReplaceFileInfo(string oldLocation)
  {
    var newLocation = oldLocation.Replace(Path.Combine(_fromPath), Path.Combine(_newFolder, _newSolutionName));
    newLocation = _replaceIt.ReplaceIt(newLocation);
    return newLocation;
  }

  private string ReplaceFileContent(string content)
  {
    return _replaceIt.ReplaceIt(content);
  }

  private bool IsIgnored(string path)
  {
    return GlobalSettings.Default.IgnoreFiles.Any(path.EndsWith);
  }

  private bool IsContentReplaceAble(string path)
  {
    return GlobalSettings.Default.ValidSourceFiles.Any(x =>
      path.EndsWith(x, StringComparison.InvariantCultureIgnoreCase));
  }
}
